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

namespace TestOivWinForms
{
    public partial class TestForm : Form
    {
        private SoSeparator _root;
        private SoWinRenderArea _renderArea;

        SoPerspectiveCamera _camera;
        float _phi = 0;
        float _r = 4;
        float _originX = 0;
        float _originY = 0;
        float _originZ = 0;
        SbVec3f _sceneCenter;
        float _radius;
        ConvexSettings _scene;

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
            _sceneCenter = new SbVec3f(0, 0, 0);

            _radius = new [] { _scene.BoundaryBox.Length, _scene.BoundaryBox.Width, _scene.BoundaryBox.Height }.Max() * 2;
            _radius = 3;

            var shapeHints = new SoShapeHints();
            shapeHints.vertexOrdering.Value = SoShapeHints.VertexOrderings.COUNTERCLOCKWISE;
            shapeHints.shapeType.Value = SoShapeHints.ShapeTypes.SOLID;
            _root.AddChild(shapeHints);

            var myMaterial = new SoMaterial();
            myMaterial.diffuseColor.SetValue(0.5f, 0.5f, 0.5f);
            _root.AddChild(myMaterial);

            var transform = new SoTransform();
            transform.rotation.Value = new SbRotation(new SbVec3f(1, 0, 0), (float)(Math.PI / 180) * 80f);
            _root.AddChild(transform);

            var cone = new SoCone();
            cone.height.Value = 0.5f;
            cone.bottomRadius.Value = 0.2f;
            _root.AddChild(cone);

            //for (int i = 0; i < 1; i++)
            //{
            //    //ConvexHelper.ConvexGenerator
            //    SoIndexedFaceSet faceSet = FaceSetHelper.GetRandomFaceSetsAsOneObject(100);
            //    _root.AddChild(faceSet);
            //}
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
            float angle = 0;
            float step = 0.01f;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            while (sw.ElapsedMilliseconds < 10000)
            {
                _camera.position.Value = GetCameraPosition(angle);
                _camera.PointAt(_sceneCenter);
                _renderArea.Render();

                count++;
                angle += step;
            }

            sw.Stop();

            double fps = count / sw.Elapsed.TotalSeconds;
            MessageBox.Show(fps.ToString());
        }
    }
}
