using DEM.Net.Core;
using DEM.Net.Extension.VisualTopo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp
{
    public class DemNetVisualTopoService
    {
        private readonly ServiceProvider services;
        private readonly VisualTopoService visualTopoService;
        private readonly ElevationService elevationService;

        public DemNetVisualTopoService()
        {
            this.services = new ServiceCollection()
                .AddLogging(loggingBuilder => loggingBuilder
                            .AddDebug()
                            .SetMinimumLevel(LogLevel.Debug))
                       .AddDemNetCore()
                       .AddDemNetVisualTopoExtension()
                       .BuildServiceProvider();


            this.visualTopoService = services.GetService<VisualTopoService>();
            this.elevationService = services.GetService<ElevationService>();
        }

        public VisualTopoModel  ExportVisualTopoToExcel(string visualTopoFileName, string outputExcelFileName, DEMDataSet dataSet)
        {

            VisualTopoModel model = this.visualTopoService.LoadFile(visualTopoFileName, Encoding.GetEncoding("ISO-8859-1")
                                                                   , decimalDegrees: true
                                                                   , ignoreRadialBeams: true);

            BoundingBox bbox = model.BoundingBox // relative coords
                                        .Translate(model.EntryPoint.Longitude, model.EntryPoint.Latitude, model.EntryPoint.Elevation ?? 0) // absolute coords
                                        .Pad(50) // margin around model
                                        .ReprojectTo(model.SRID, dataSet.SRID);

            this.elevationService.DownloadMissingFiles(dataSet, bbox);

            this.visualTopoService.ComputeFullCavityElevations(model, dataSet); // will add TerrainElevationAbove and entry elevations
            using (MemoryStream ms = visualTopoService.ExportToExcel(model, autoFitColumns: false))
            {
                File.WriteAllBytes(outputExcelFileName, ms.ToArray());
            }

            return model;
        }
    }
}
