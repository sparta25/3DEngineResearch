using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OIV.Inventor.Nodes;
using OIV.Inventor;
using OIV.Inventor.Win;
using ConvexHelper;
using OIVCommon;
using System.Configuration;

namespace TestOivWinForms
{
    public partial class TestForm : Form, ITestable
    {
        private readonly SoSeparator _root;
        private readonly SoWinRenderArea _renderArea;
        private readonly TestHelper _testHelper;
        SoPerspectiveCamera _camera;
        SbVec3f _sceneCenter;
        float _radius;
        ConvexSettings _scene;
        float _angle = 0;

        public TestForm()
        {
            InitializeComponent();

            _renderArea = new SoWinRenderArea(_panelView);
            _root = new SoSeparator();
            
            CreateCamera();
            CreateLights();
            
            _testHelper = new TestHelper(this);
            _testHelper.CreateScene();
            
        }

        private ConvexSettings GetSceneSettings(TextReader reader)
        {
            return SerializationProvider.LoadFromXml<ConvexSettings>(reader);
        }

        private ConvexSettings GetSceneSettings()
        {
            var settingsFile = ConfigurationManager.AppSettings["SettingsFile"];
            return SerializationProvider.LoadFromXml<ConvexSettings>(settingsFile);
        }
        
        private void CreateFaceSets()
        {
            var shapeHints = new SoShapeHints();
            shapeHints.vertexOrdering.Value = SoShapeHints.VertexOrderings.COUNTERCLOCKWISE;
            shapeHints.shapeType.Value = SoShapeHints.ShapeTypes.SOLID;
            _root.AddChild(shapeHints);

            int[] grid = _scene.Indices.ToArray();

            foreach (var plane in _scene.Planes)
            {
                // Using the new SoVertexProperty node is more efficient
                var myVertexProperty = new SoVertexProperty();

                var colors = plane.Colors.ToArrayOfVec3F();

                // Define colors for the faces
                for (int k = 0; k < colors.Length; k++)
                    myVertexProperty.orderedRGBA[k] = new SbColor(colors[k]).GetPackedValue();
                

                myVertexProperty.materialBinding.Value = SoVertexProperty.Bindings.PER_FACE;

                // Define coordinates for vertices
                myVertexProperty.vertex.SetValues(0, plane.Vertices.ToArrayOfVec3F());

                // Define the IndexedFaceSet, with indices into
                // the vertices:
                var myFaceSet = new SoIndexedFaceSet();
                myFaceSet.coordIndex.SetValues(0, grid);
                myFaceSet.vertexProperty.Value = myVertexProperty;

                _root.AddChild(myFaceSet);
            }
        }

        private void CreateLights()
        {
            var light1 = new SoDirectionalLight();
            light1.color.Value = new SbColor(1, 1, 1);
            light1.direction.Value = new SbVec3f(1, 0, 0);
            _root.AddChild(light1);

            var light2 = new SoDirectionalLight();
            light2.color.Value = new SbColor(1, 1, 1);
            light2.direction.Value = new SbVec3f(-1, 0, 0);
            _root.AddChild(light2);
        }

        private void CreateCamera()
        {
            _camera = new SoPerspectiveCamera();
            _root.AddChild(_camera);
        }

        private void SetupCamera()
        {
            _camera.position.Value = GetCameraPosition(0);
            _camera.PointAt(_sceneCenter);
        }

        private SbVec3f GetCameraPosition(float angle)
        {
            return new SbVec3f((float)(_radius * Math.Cos(angle) + _sceneCenter.X),
                _sceneCenter.Y, (float)(_radius * Math.Sin(angle) + _sceneCenter.Z));
        }

        private void SetupScene()
        {
            _scene = GetSceneSettings();
            //_scene = GetScene(Console.In);

            _sceneCenter = new SbVec3f(_scene.BoundaryBox.Length / 2, _scene.BoundaryBox.Width / 2, _scene.BoundaryBox.Height / 2);

            _radius = new[] { _scene.BoundaryBox.Length, _scene.BoundaryBox.Width, _scene.BoundaryBox.Height }.Max() * 2;
            
            
            CreateFaceSets();
            SetupCamera();

            _renderArea.SetSceneGraph(_root);
            _renderArea.SetAutoRedraw(false);
            _renderArea.Render();
        }

        private void _buttonRotate_Click(object sender, EventArgs e)
        {
            _testHelper.Rotate();
        }

        #region ITestable

        public void CreateScene()
        {
            SetupScene();
        }

        public void Rotate()
        {
            const float step = 0.01f;
            _camera.position.Value = GetCameraPosition(_angle);
            _camera.PointAt(_sceneCenter);
            _renderArea.Render();
            _angle += step;
        }
        
        #endregion

        
    }
}
