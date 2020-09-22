using DEM.Net.Core;
using DEM.Net.Core.Configuration;
using DEM.Net.Core.Imagery;
using DEM.Net.Extension.VisualTopo;
using DEM.Net.glTF;
using DEM.Net.glTF.SharpglTF;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp
{
    public class DemNetVisualTopoService
    {
        private readonly ServiceProvider services;
        private readonly VisualTopoService visualTopoService;
        private readonly ElevationService elevationService;
        private readonly SharpGltfService sharpGltfService;
        private readonly MeshService meshService;
        private readonly ImageryService imageryService;

        public DemNetVisualTopoService()
        {
            var config = new ConfigurationBuilder()
                       .SetBasePath(AppContext.BaseDirectory)
                       .AddJsonFile("appsettings.json", optional: true)
                       .AddJsonFile("secrets.json", optional: true, reloadOnChange: false)
                       .Build();

            
            this.services = new ServiceCollection()
                .AddLogging(loggingBuilder => loggingBuilder
                            .AddDebug()
                            .SetMinimumLevel(LogLevel.Debug))
                 .Configure<AppSecrets>(config.GetSection(nameof(AppSecrets)))
                       .AddDemNetCore()
                       .AddDemNetglTF()
                       .AddDemNetVisualTopoExtension()
                       .BuildServiceProvider();


            this.visualTopoService = services.GetService<VisualTopoService>();
            this.elevationService = services.GetService<ElevationService>();
            this.meshService = services.GetService<MeshService>();
            this.sharpGltfService = services.GetService<SharpGltfService>();
            this.imageryService = services.GetService<ImageryService>();
        }

        public VisualTopoModel CreateVisualTopoModelFromFile(string visualTopoFileName, DEMDataSet dataSet, float zFactor)
        {

            VisualTopoModel model = this.visualTopoService.LoadFile(visualTopoFileName, Encoding.GetEncoding("ISO-8859-1")
                                                                   , decimalDegrees: true
                                                                   , ignoreRadialBeams: true
                                                                   , zFactor);

            BoundingBox bbox = model.BoundingBox // relative coords
                                        .Translate(model.EntryPoint.Longitude, model.EntryPoint.Latitude, model.EntryPoint.Elevation ?? 0) // absolute coords
                                        .Pad(50) // margin around model
                                        .ReprojectTo(model.SRID, dataSet.SRID);

            this.elevationService.DownloadMissingFiles(dataSet, bbox);

            this.visualTopoService.ComputeFullCavityElevations(model, dataSet, zFactor); // will add TerrainElevationAbove and entry elevations

            this.visualTopoService.Create3DTriangulation(model);
            return model;
        }

        public void ExportVisualTopoToExcel(VisualTopoModel model, string outputExcelFileName)
        {
            using (MemoryStream ms = visualTopoService.ExportToExcel(model, autoFitColumns: false))
            {
                File.WriteAllBytes(outputExcelFileName, ms.ToArray());
            }
        }

        internal void ExportVisualTopoToGLB(VisualTopoModel visualTopoModel, DEMDataSet dataset, string outputFile, bool drawOnTexture, float marginMeters, float zFactor)
        {
            int outputSrid = 3857;
            float lineWidth = 1.0F;
            int numTilesPerImage = 4;
            ImageryProvider imageryProvider = ImageryProvider.EsriWorldImagery;


            BoundingBox bbox = visualTopoModel.BoundingBox // relative coords
                                    .Translate(visualTopoModel.EntryPoint.Longitude, visualTopoModel.EntryPoint.Latitude, visualTopoModel.EntryPoint.Elevation ?? 0) // absolute coords
                                    .Pad(marginMeters) // margin around model
                                    .ReprojectTo(visualTopoModel.SRID, dataset.SRID); // DEM coords

            //=======================
            // Height map (get dem elevation for bbox)
            //
            // Get height map
            // Note that ref Bbox means that the bbox will be adjusted to match DEM data
            var heightMap = elevationService.GetHeightMap(ref bbox, dataset, downloadMissingFiles: true);
            var bboxTerrainSpace = bbox.ReprojectTo(dataset.SRID, outputSrid); // terrain coords

            // Model origin
            GeoPoint axisOriginWorldSpace = visualTopoModel.EntryPoint.ReprojectTo(visualTopoModel.SRID, outputSrid)
                                    .CenterOnOrigin(bboxTerrainSpace);
            Vector3 axisOriginModelSpace = visualTopoModel.EntryPoint.AsVector3();

            //await signalR.ReportGenerateProgressAsync(signalRConnectionId, "Getting GPX extent elevation...", 20);
            //=======================
            // Local transform function from model coordinates (relative to entry, in meters)
            // and global coordinates absolute in final 3D model space
            //
            IEnumerable<GeoPoint> TransformLine(IEnumerable<GeoPoint> line)
            {
                var newLine = line.Translate(visualTopoModel.EntryPoint)              // Translate to entry (=> global topo coord space)
                                    .ReprojectTo(visualTopoModel.SRID, outputSrid)    // Reproject to terrain coord space
                                    .CenterOnOrigin(bboxTerrainSpace)      // Center on terrain space origin
                                    .CenterOnOrigin(axisOriginWorldSpace);
                return newLine;
            };

            //=======================
            // 3D model
            //
            var gltfModel = sharpGltfService.CreateNewModel();

            // Add X/Y/Z axis on entry point
            var axis = meshService.CreateAxis();
            sharpGltfService.AddMesh(gltfModel, "Axis", axis, doubleSided: false);

            int i = 0;

            var triangulation = visualTopoModel.TriangulationFull3D.Clone()
                                            .Translate(axisOriginModelSpace) // already zScaled if zFactor > 1
                                            .ReprojectTo(visualTopoModel.SRID, outputSrid)
                                            .CenterOnOrigin(bboxTerrainSpace)
                                            .CenterOnOrigin(axisOriginWorldSpace.AsVector3());
            gltfModel = sharpGltfService.AddMesh(gltfModel, "Cavite3D", triangulation, VectorsExtensions.CreateColor(0, 255, 0), doubleSided: false);

            ////=======================
            //// 3D cavity model
            //foreach (var line in visualTopoModel.Topology3D) // model.Topology3D is the graph of topo paths
            //{
            //    // Add line to model
            //    gltfModel = sharpGltfService.AddLine(gltfModel
            //                                    , string.Concat("CavitySection", i++)     // name of 3D node
            //                                    , TransformLine(line)               // call transform function
            //                                    , color: VectorsExtensions.CreateColor(255, 0, 0, 128)
            //                                    , lineWidth);
            //}

            // Reproject and center height map coordinates
            heightMap = heightMap.ReprojectTo(dataset.SRID, outputSrid)
                                .CenterOnOrigin(bboxTerrainSpace)
                                .ZScale(zFactor)
                                .CenterOnOrigin(axisOriginWorldSpace);

            //=======================
            // Textures
            //
            PBRTexture pbrTexture = null;
            if (imageryProvider != null)
            {
                TileRange tiles = imageryService.DownloadTiles(bbox, imageryProvider, numTilesPerImage);
                string fileName = Path.Combine(Path.GetDirectoryName(outputFile), "Texture.jpg");

                Console.WriteLine("Construct texture...");
                TextureInfo texInfo = null;
                if (drawOnTexture)
                {
                    var topoTexture = visualTopoModel.Topology3D.SelectMany(l=>l).Translate(visualTopoModel.EntryPoint).ReprojectTo(visualTopoModel.SRID, 4326);
                    texInfo = imageryService.ConstructTextureWithGpxTrack(tiles, bbox, fileName, TextureImageFormat.image_jpeg
                        , topoTexture, drawGpxVertices: true);
                }
                else
                {
                     texInfo = imageryService.ConstructTexture(tiles, bbox, fileName, TextureImageFormat.image_jpeg);
                }
                
                

                pbrTexture = PBRTexture.Create(texInfo, null);
            }
            //
            //=======================

            gltfModel = sharpGltfService.AddTerrainMesh(gltfModel, heightMap, pbrTexture);
            gltfModel.SaveGLB(outputFile);


        }
    }
}
