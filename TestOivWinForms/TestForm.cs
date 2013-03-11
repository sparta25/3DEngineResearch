using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using OIV.Inventor.Win.Viewers;
using OIV.Inventor.Nodes;
using OIV.Inventor;
using OIV.Inventor.Win;

using ConvexHelper;
using OIVCommon;

namespace TestOivWinForms
{
    public partial class TestForm : Form
    {
        private SoSeparator _root;
        private SoWinRenderArea _renderArea;

        SoPerspectiveCamera _camera;
        SbVec3f _sceneCenter;
        float _radius;
        ConvexSettings _scene;
        float _angle = 0;

        public TestForm()
        {
            InitializeComponent();

            SetupScene();
        }

        void CreateFaceSets()
        {
            _scene = SerializationProvider.LoadFromXml<ConvexSettings>(Console.In);

            _sceneCenter = new SbVec3f(_scene.BoundaryBox.Length / 2,
                _scene.BoundaryBox.Width /2 , _scene.BoundaryBox.Height / 2);

            _radius = new [] { _scene.BoundaryBox.Length, _scene.BoundaryBox.Width, _scene.BoundaryBox.Height }.Max() * 2;

            var shapeHints = new SoShapeHints();
            shapeHints.vertexOrdering.Value = SoShapeHints.VertexOrderings.COUNTERCLOCKWISE;
            shapeHints.shapeType.Value = SoShapeHints.ShapeTypes.SOLID;
            _root.AddChild(shapeHints);

            //var countOfVertices = (_scene.PartWidth + 1) * (_scene.PartHeight + 1);
            //SbVec3f[] vertices = _scene.Vertices.ToArrayOfVec3F();
            //SbVec3f[] colors = _scene.Colors.ToArrayOfVec3F();
            
            int[] grid = _scene.Indices.ToArray();

            //for (int i = 0; i < _scene.NumberOfPlanes; i++)
            foreach (var plane in _scene.Planes)
            {
                //var ver = new SbVec3f[countOfVertices];
                //Array.Copy(vertices, i * countOfVertices, ver, 0, ver.Length);

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

        void CreateLights()
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

        void CreateCamera()
        {
            _camera = new SoPerspectiveCamera();
            _root.AddChild(_camera);
        }

        void SetupCamera()
        {
            _camera.position.Value = GetCameraPosition(0);
            _camera.PointAt(_sceneCenter);
        }

        SbVec3f GetCameraPosition(float angle)
        {
            return new SbVec3f((float)(_radius * Math.Cos(angle) + _sceneCenter.X),
                _sceneCenter.Y, (float)(_radius * Math.Sin(angle) + _sceneCenter.Z));
        }

        void SetupScene()
        {
            _renderArea = new SoWinRenderArea(_panelView);
            _root = new SoSeparator();

            CreateCamera();
            CreateLights();
            CreateFaceSets();
            SetupCamera();

            _renderArea.SetSceneGraph(_root);
            _renderArea.SetAutoRedraw(false);
            _renderArea.Render();
        }

        private void _buttonRotate_Click(object sender, EventArgs e)
        {
            int count = 0;
            float step = 0.01f;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            while (sw.ElapsedMilliseconds < 10000)
            {
                _camera.position.Value = GetCameraPosition(_angle);
                _camera.PointAt(_sceneCenter);
                _renderArea.Render();

                count++;
                _angle += step;
            }

            sw.Stop();

            double fps = count / sw.Elapsed.TotalSeconds;
            MessageBox.Show(fps.ToString());
        }
    }
}
