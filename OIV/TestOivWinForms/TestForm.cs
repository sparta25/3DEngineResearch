﻿using System;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;
using OIV.Inventor;
using OIV.Inventor.Nodes;
using OIV.Inventor.Win;
using OIV.Inventor.Actions;
using OIVCommon;
using TestFramework;
using OIV.Inventor.Win.Viewers;

namespace TestOivWinForms
{
    public partial class TestForm : Form, ITestable
    {
        private readonly float _radius;
        private readonly SoWinRenderArea _renderArea;
        // Create a viewer
        private SoWinExaminerViewer myViewer;

        private readonly SoSeparator _root;
        private readonly ConvexSettings _scene;
        private readonly SbVec3f _sceneCenter;
        private readonly TestHelper _testHelper;
        private float _angle;
        private SoPerspectiveCamera _camera;

        public TestForm()
        {
            InitializeComponent();
            
            //_renderArea = new SoWinRenderArea(_panelView);
            myViewer = new SoWinExaminerViewer(this, "", true, SoWinFullViewer.BuildFlags.BUILD_ALL, SoWinViewer.Types.BROWSER);
            _root = new SoSeparator();
            _scene = GetSceneSettings();
            
            _sceneCenter = new SbVec3f(_scene.BoundaryBox.Length/2, _scene.BoundaryBox.Width/2,
                                       _scene.BoundaryBox.Height/2);
            _radius = new[] {_scene.BoundaryBox.Length, _scene.BoundaryBox.Width, _scene.BoundaryBox.Height}.Max()*2;

            CreateCamera();
            CreateLights();

            _testHelper = new TestHelper(this);
            _testHelper.CreateScene();
        }

        #region ITestable

        public void CreateScene()
        {
            SetupScene();
            //SoOutput output = new SoOutput();
            //output.OpenFile("output.iv");
            //SoWriteAction writeAction = new SoWriteAction(output);
            //writeAction.Apply(_root);
            //output.CloseFile();
        }

        public void Render()
        {
            const float step = 0.01f;
            _camera.position.Value = GetCameraPosition(_angle, _radius, _sceneCenter);
            _camera.PointAt(_sceneCenter);
            _renderArea.Render();
            _angle += step;
        }

        #endregion

        #region private methods

        private static ConvexSettings GetSceneSettings()
        {
            string settingsFile = ConfigurationManager.AppSettings["SettingsFile"];

            return settingsFile != null
                       ? SerializationHelper.LoadFromXml<ConvexSettings>(settingsFile)
                       : SerializationHelper.LoadFromXml<ConvexSettings>(Console.In);
        }

        private void CreateFaceSets()
        {
            var shapeHints = new SoShapeHints();
            shapeHints.vertexOrdering.Value = SoShapeHints.VertexOrderings.COUNTERCLOCKWISE;
            shapeHints.shapeType.Value = SoShapeHints.ShapeTypes.SOLID;
            _root.AddChild(shapeHints);
            
            // Add a rotor node to spin the vanes
            SoRotor myRotor = new SoRotor();
            myRotor.rotation.SetValue(new SbVec3f(0.0f, 1.0f, 0.0f), (float)(Math.PI / 32.0f)); // z axis
            myRotor.speed.Value = 1.0f;
            _root.AddChild(myRotor);

            int[] grid = _scene.Indices.ToArray();

            foreach (Plane plane in _scene.Planes)
            {
                // Using the new SoVertexProperty node is more efficient
                var myVertexProperty = new SoVertexProperty();

                SbVec3f[] colors = plane.Colors.ToArrayOfVec3F();

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

        private void SetupCamera(float radius, SbVec3f sceneCenter)
        {
            _camera.position.Value = GetCameraPosition(0, radius, sceneCenter);
            _camera.PointAt(sceneCenter);
        }

        private static SbVec3f GetCameraPosition(float angle, float radius, SbVec3f sceneCenter)
        {
            return new SbVec3f((float) (radius*Math.Cos(angle) + sceneCenter.X),
                               sceneCenter.Y, (float) (radius*Math.Sin(angle) + sceneCenter.Z));
        }

        private void SetupScene()
        {
            CreateFaceSets();
            SetupCamera(_radius, _sceneCenter);
            myViewer.SetSceneGraph(_root);
            myViewer.SetTitle("OIV Test");


            //_renderArea.SetSceneGraph(_root);
            //_renderArea.SetAutoRedraw(false);
            //_renderArea.Render();
        }

        private void _buttonRotate_Click(object sender, EventArgs e)
        {
            //_testHelper.Render();
        }

        #endregion
    }
}