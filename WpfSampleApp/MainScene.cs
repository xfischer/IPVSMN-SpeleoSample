using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using DEM.Net.Core.Imagery;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.WpfCore.MonoGameControls;

using SharpGLTF.Validation;
using PropertyTools.DataAnnotations;
using DEM.Net.Extension.VisualTopo;
using WpfSampleApp;
using System.IO;
using DEM.Net.Core;
using System.Windows.Input;

namespace MonoGameViewer
{
    public class MainScene : MonoGameViewModel
    {
        private readonly ImageryService imageryService;
        private readonly DemNetVisualTopoService demNetService;


        public MainScene(ImageryService imageryService, DemNetVisualTopoService demNetService) : base()
        {
            this.imageryService = imageryService;
            this.demNetService = demNetService;

            _ImageryProviders = imageryService.GetRegisteredProviders().ToArray();
            ImageryProvider = _ImageryProviders.First();
        }

        #region data

        bool _IsAssimp;
        SharpGLTF.Schema2.ModelRoot _Model;
        DeviceModelCollection _ModelTemplate;
        BoundingSphere _ModelSphere;

        private bool _UseClassicEffects;

        private ModelInstance _ModelInstance;

        private Quaternion _Rotation = Quaternion.Identity;
        private Vector3 _Translation = Vector3.Zero;
        private float _Zoom = 2.5f;

        private readonly GlobalLight _GlobalLight = new GlobalLight();
        private readonly DEMDataSet DefaultDataset = DEMDataSet.AW3D30;
        private readonly PunctualLight[] _PunctualLights = new PunctualLight[] { PunctualLight.CreateDefault(0), PunctualLight.CreateDefault(1), PunctualLight.CreateDefault(2) };
        private ImageryProvider[] _ImageryProviders;



        #endregion

        #region properties

        private string _visualTopoFile;
        private float _zFactor = 2f;

        [InputFilePath(".tro")]
        [DisplayName("Fichier VisualTopo")]
        [FilterProperty("OpenFilePathFilter")]
        [AutoUpdateText]
        public string OpenFilePath
        {
            get => _visualTopoFile;
            set
            {
                _visualTopoFile = value;
                OpenFile();
            }
        }


        [Browsable(false)]
        public string OpenFilePathFilter => "VisualTopo files (*.tro)|*.tro";


        [Browsable(false)]
        public GlobalLight GlobalLight => _GlobalLight;

        [Browsable(false)]
        public PunctualLight[] PunctualLights => _PunctualLights;


        [Browsable(false)]
        public ImageryProvider[] ImageryProviders => _ImageryProviders;

        [Browsable(true)]
        [ItemsSourceProperty(nameof(ImageryProviders))]
        [DisplayMemberPath("Name")]
        public ImageryProvider ImageryProvider { get; set; }

        [Browsable(true)]
        public bool DrawOnTexture { get; set; } = true;

        public float MarginAroundModel { get; set; } = 200f;

        #endregion

        #region API
        private void OpenFile()
        {
            try
            {
                if (!File.Exists(_visualTopoFile)) return;

                VisualTopoModel visualTopoModel = demNetService.CreateVisualTopoModelFromFile(_visualTopoFile, DefaultDataset, _zFactor);

                string outputFile = Path.GetFullPath($"{visualTopoModel.Name}_{DateTime.Now:yyyy MM dd - HH mm ss}.glb");

                _Model = demNetService.GenerateVisualTopoGltfModel(visualTopoModel, DefaultDataset, outputFile
                                                    , imageryProvider: this.ImageryProvider
                                                    , drawOnTexture: DrawOnTexture
                                                    , marginMeters: MarginAroundModel
                                                    , zFactor: _zFactor);
                _ProcessModel();

                //StringBuilder sb = new StringBuilder();
                //sb.AppendLine($"{visualTopoModel.Name} - Auteur: {visualTopoModel.Author}");
                //sb.AppendLine($"Projection {visualTopoModel.EntryPointProjectionCode}");
                //sb.AppendLine($"{visualTopoModel.Sets.Count} section(s), {visualTopoModel.Sets.Sum(s => s.Data.Count)} lignes");
                //sb.AppendLine($"Profondeur max : {visualTopoModel.Graph.AllNodes.Max(n => n.Model.Depth):N2} m");
                //sb.AppendLine($"Distance max : {visualTopoModel.Graph.AllNodes.Max(n => n.Model.DistanceFromEntry):N2} m");
                //sb.AppendLine($"Fichier 3D exporté vers {outputFile}");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
        public void LoadModel(string filePath)
        {
            try
            {
                var fp = filePath.ToLower();

                if (fp.EndsWith(".zip"))
                {
                    _Model = SharpGLTF.IO.ZipReader.LoadSchema2(filePath, ValidationMode.TryFix);
                    _IsAssimp = false;
                }
                else if (fp.EndsWith(".glb") || fp.EndsWith(".gltf"))
                {
                    _Model = SharpGLTF.Schema2.ModelRoot.Load(filePath, ValidationMode.TryFix);
                    _IsAssimp = false;
                }
                else if (fp.EndsWith(".obj") || fp.EndsWith(".blend") || fp.EndsWith(".fbx"))
                {
                    _IsAssimp = true;
                    _ModelTemplate = Microsoft.Xna.Framework.Content.Runtime.Graphics.FormatAssimp.LoadModel(filePath, GraphicsDevice, _UseClassicEffects);
                    _ModelSphere = _ModelTemplate.DefaultModel.ModelBounds;
                    _ModelInstance = null;
                }
            }
            catch (Exception ex) { System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error); return; }

            _ProcessModel();
        }

        internal void TranslateModel(float x, float y)
        {
            _Translation += new Vector3(x, -y, 0);
        }

        private void _ProcessModel()
        {
            if (_IsAssimp) return;
            if (_Model == null) return;
            if (_ModelTemplate != null) { _ModelTemplate.Dispose(); _ModelTemplate = null; }

            _ModelTemplate = Microsoft.Xna.Framework.Content.Runtime.Graphics.FormatGLTF.ReadModel(_Model, GraphicsDevice, _UseClassicEffects);
            _ModelSphere = _ModelTemplate.DefaultModel.ModelBounds;
            _ModelInstance = null;

            GC.Collect();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();

            _ModelTemplate?.Dispose(); _ModelTemplate = null;
        }

        public void RotateModel(float x, float y)
        {
            _Rotation = Quaternion.CreateFromYawPitchRoll(x, y, 0) * _Rotation;
            _Rotation.Normalize();
        }

        public void ZoomModel(float z)
        {
            _Zoom += z * 0.001f;
            if (_Zoom < 0.0001f) _Zoom = 0.0001f;
        }

        public override void Update(GameTime gameTime)
        {
            if (_ModelInstance == null && _ModelTemplate != null)
            {
                _ModelInstance = _ModelTemplate.DefaultModel.CreateInstance();

                _Rotation = Quaternion.Identity;
                _Zoom = 2.5f;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (_ModelInstance == null) return;

            _ModelInstance.Armature.SetAnimationFrame(0, (float)gameTime.TotalGameTime.TotalSeconds);

            var lookAt = _ModelSphere.Center;
            var camPos = _ModelSphere.Center + new Vector3(0, 0, _ModelSphere.Radius * _Zoom);

            var camera = Matrix.CreateWorld(camPos, lookAt - camPos, Vector3.UnitY);

            var env = new PBREnvironment();
            env.SetExposure((float)_GlobalLight.Exposure / 10.0f);
            env.SetAmbientLight(_GlobalLight.ToXna());

            if (_PunctualLights != null)
                for (int i = 0; i < _PunctualLights.Length; ++i)
                {
                    env.SetDirectLight(i, _PunctualLights[i].Direction, _PunctualLights[i].XnaColor, _PunctualLights[i].Intensity / 10.0f);
                }

            if (_ModelInstance != null)
            {
                _ModelInstance.WorldMatrix = Matrix.CreateFromQuaternion(_Rotation) * Matrix.CreateTranslation(_Translation);

                var ctx = new ModelDrawingContext(this.GraphicsDevice);

                ctx.NearPlane = Math.Min(1, _ModelSphere.Radius);

                ctx.SetCamera(camera);
                ctx.DrawModelInstance(env, _ModelInstance);
            }
        }

        #endregion
    }


    public class GlobalLight
    {
        [Slidable(0, 100)]
        [WideProperty]
        public int Exposure { get; set; } = 25;

        public System.Windows.Media.Color AmbientColor { get; set; } = System.Windows.Media.Colors.Black;

        public Vector3 ToXna() { return new Vector3(AmbientColor.ScR, AmbientColor.ScG, AmbientColor.ScB); }
    }

    public class PunctualLight
    {
        public static PunctualLight CreateDefault(int idx)
        {
            var l = new PunctualLight();

            l.Intensity = 15;
            l.Color = System.Windows.Media.Colors.White;

            switch (idx)
            {
                case 0:
                    l.DirectionAngle = 60;
                    l.ElevationAngle = 30;
                    l.Intensity = 40;
                    break;

                case 1:
                    l.DirectionAngle = -70;
                    l.ElevationAngle = 60;
                    l.Color = System.Windows.Media.Colors.DeepSkyBlue;
                    break;

                case 2:
                    l.DirectionAngle = 20;
                    l.ElevationAngle = -50;
                    l.Color = System.Windows.Media.Colors.OrangeRed;
                    break;
            }

            return l;
        }

        [Category("Source")]
        [Slidable(-180, 180)]
        // [WideProperty]
        [DisplayName("Direction")]
        public int DirectionAngle { get; set; }

        [Category("Source")]
        [Slidable(-90, 90)]
        //[WideProperty]
        [DisplayName("Elevation")]
        public int ElevationAngle { get; set; }

        [Category("Properties")]
        public System.Windows.Media.Color Color { get; set; }

        [Category("Properties")]
        [Slidable(0, 100)]
        [WideProperty]
        public int Intensity { get; set; }

        [Browsable(false)]
        public Vector3 Direction
        {
            get
            {
                float yaw = (float)(DirectionAngle * Math.PI) / 180.0f;
                float pitch = (float)(ElevationAngle * Math.PI) / 180.0f;
                var xform = Matrix.CreateFromYawPitchRoll(yaw + 3.141592f, pitch, 0);
                return Vector3.Transform(Vector3.UnitZ, xform);
            }
        }

        [Browsable(false)]
        public Color XnaColor => new Color(Color.ScR, Color.ScG, Color.ScB);
    }
}
