using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConvexHelper;
using OIV.Inventor;
using OIV.Inventor.Win.Viewers;
using OIV.Inventor.Nodes;
using OIVCommon;
using OIV.Inventor.Sensors;
namespace OVIConvexTest
{
    
    public class IndexedFaceSet : Form, ITestable
	{
		#region private declarations
		private SoSeparator _root;
        private SoWinExaminerViewer _viewer;
		private Panel _parent;
		private Panel _controlPanel;
		private Panel _viewerPanel;
        private readonly IConvexSettings _settings;
        const int ChangingPersents = 80;
		private readonly System.ComponentModel.Container _components = null;
        private readonly SoRotation _myRotation = new SoRotation();
        private readonly List<SoTimerSensor> _sensors = new List<SoTimerSensor>();
		#endregion
        
		#region Constructors	
		public IndexedFaceSet(IConvexSettings settings)
		{
            _settings = settings;
			InitializeComponent();
			_parent = _viewerPanel;

            //Testing
            var testHelper = new TestHelper(this);
            testHelper.CreateScene();
            testHelper.Rotate();
		}
		#endregion

		#region Graphics components
		
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (_components != null) 
				{
					_components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		private void InitializeComponent()
		{
            this._controlPanel = new System.Windows.Forms.Panel();
            this._viewerPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // _controlPanel
            // 
            this._controlPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._controlPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this._controlPanel.Location = new System.Drawing.Point(879, 0);
            this._controlPanel.Name = "_controlPanel";
            this._controlPanel.Size = new System.Drawing.Size(0, 523);
            this._controlPanel.TabIndex = 0;
            // 
            // _viewerPanel
            // 
            this._viewerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._viewerPanel.Location = new System.Drawing.Point(0, 0);
            this._viewerPanel.Name = "_viewerPanel";
            this._viewerPanel.Size = new System.Drawing.Size(879, 523);
            this._viewerPanel.TabIndex = 1;
            // 
            // IndexedFaceSet
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.ClientSize = new System.Drawing.Size(879, 523);
            this.Controls.Add(this._viewerPanel);
            this.Controls.Add(this._controlPanel);
            this.Name = "IndexedFaceSet";
            this.Text = "IndexedFaceSet - Main Form";
            this.ResumeLayout(false);

		}
		#endregion
        
        #region Function to add to make the demo works properly
		//---------------------------------------------------------------------
        
        public bool  Init(Panel parent)
		{
			/* The demo Launcher always give a parent */
			/* If we don't want to put the application on demoLauncher, we just need to */
			/* Comment the setParent line and  uncomment the show method*/

			SetParent(parent);
			return true;

		}
						
		public void Stop()
		{
			HideViewer();
			Close();
		}

		public void SetParent(Panel p)
		{
			_parent = p;
		}

		public void HideViewer()
		{
			_viewer.Hide();
		}

		//---------------------------------------------------------------------
		#endregion

		#region Events
		//Put here the graphics events
		#endregion

		#region CallBack
		//Put here the Inventor callbacks 
		#endregion
        
        #region ITestable implementation
        
        public void CreateScene()
        {
            _viewer =
                new SoWinExaminerViewer(_parent, "Name", true,
                SoWinFullViewer.BuildFlags.BUILD_ALL,
                SoWinViewer.Types.BROWSER);

            _root = new SoSeparator();

            _root.AddChild(_myRotation);

            //ToDo: remove commented lines
            //SbVec3f[] vertices = _settings.Vertices.ToArrayOfVec3F();
            //SbVec3f[] colors = _settings.Colors.ToArrayOfVec3F();
            //var size = (_settings.PartWidth + 1) * (_settings.PartHeight + 1);
            //for (int i = 0; i < _settings.NumberOfPlanes; i++)

            int[] grid = _settings.Indices.ToArray();

            foreach (var plane in _settings.Planes)
            {
                var myHints = new SoShapeHints();
                myHints.vertexOrdering.Value = SoShapeHints.VertexOrderings.CLOCKWISE;
                myHints.faceType.Value = SoShapeHints.FaceTypes.CONVEX;
                myHints.shapeType.Value = SoShapeHints.ShapeTypes.SOLID;
                _root.AddChild(myHints);

                //var ver = new SbVec3f[size];
                //Array.Copy(vertices, i * size, ver, 0, ver.Length);

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

                var colorSensor = new SoTimerSensor();
                var helper = new CallBackHelper(myVertexProperty, ChangingPersents, _settings);
                colorSensor.Action = helper.ColorSensorCallback;
                colorSensor.SetInterval(new SbTime(1.0f)); // scheduled once per second
                colorSensor.Schedule();
                _sensors.Add(colorSensor);
            }

            _viewer.SetSceneGraph(_root);
            _viewer.ViewAll();
        }
        
        public void Rotate()
        {
            Task.Factory.StartNew(() =>
            {
                SbRotation currentRotation = _myRotation.rotation.Value;
                var sw = Stopwatch.StartNew();
                long delta = 0;
                int frameCount = 0;
                for (float angle = 1; angle < 1000; angle++)
                {
                    Application.DoEvents();
                    currentRotation = new SbRotation(new SbVec3f(0.0f, 0.0f, 1.0f), (float)(Math.PI / 180.0f)) * currentRotation;
                    _myRotation.rotation.Value = currentRotation;
                    //sw.Stop();
                    delta = sw.ElapsedMilliseconds + delta;
                    //Thread.Sleep(50);
                    Debug.WriteLine("delta = " + delta);
                    if (delta >= 1000)
                    {
                        Debug.WriteLine("{0} per {1} millisecond", frameCount, delta);
                        break;
                    }
                    frameCount++;
                    sw.Start();
                }
                sw.Stop();
            });
        }

        #endregion
    }
}