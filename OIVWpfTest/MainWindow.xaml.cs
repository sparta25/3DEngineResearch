using System;
using System.Windows;
using ConvexHelper;
using OIV.Inventor.Win.Viewers;
using OIV.Inventor.Nodes;
using OIVCommon;
using OIV.Inventor;
namespace OIVWpfTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ITestable
    {
        private static readonly string SettingsFile = System.Configuration.ConfigurationManager.AppSettings["SettingsFile"];

        // Open Inventor viewer object
        private SoWinExaminerViewer m_viewer = null;

        public MainWindow()
        {
            InitializeComponent();
            var t = new TestHelper(this);
            t.CreateScene();
        }

        private static IConvexSettings GetConvexSettings()
        {
            return SerializationProvider.LoadFromXml<ConvexSettings>(SettingsFile);
        }

        private static void Dump()
        {
            var settings = new ConvexSettings(1, 10, 10, 5, 40)
            {
                BoundaryBox = new BoundaryBox
                {
                    Height = 100,
                    Length = 100,
                    Width = 100
                }
            };
            
            settings.FillPlanes();
            settings.FillIndices();

            SerializationProvider.DumpToXml(SettingsFile, settings);
        }

        private SoSeparator CreateScene(IConvexSettings settings)
        {
            var _root = new SoSeparator();
            
            //int[] grid = settings.Indices.ToArray();
            

            //SoShapeHints myHints = new SoShapeHints();
            //myHints.vertexOrdering.Value = SoShapeHints.VertexOrderings.COUNTERCLOCKWISE;
            //myHints.shapeType.Value = SoShapeHints.ShapeTypes.SOLID;
            //_root.AddChild(myHints);


            //// Using the new SoVertexProperty node is more efficient
            //var myVertexProperty = new SoVertexProperty();
            
            //SbVec3f[] colors = settings.Planes.ToArrayOfVec3F();
            
            //// Define colors for the faces
            //for (int i = 0; i < colors.Length; i++)
            //    myVertexProperty.orderedRGBA[i] = new SbColor(colors[i]).GetPackedValue();

            //myVertexProperty.materialBinding.Value = SoVertexProperty.Bindings.PER_FACE;

            //// Define coordinates for vertices
            //myVertexProperty.vertex.SetValues(0, vertices);

            //// Define the IndexedFaceSet, with indices into
            //// the vertices:
            //var myFaceSet = new SoIndexedFaceSet();
            //myFaceSet.coordIndex.SetValues(0, grid);

            //myFaceSet.vertexProperty.Value = myVertexProperty;
            //_root.AddChild(myFaceSet);
            return _root;
        } 
        
        public void CreateScene()
        {
            // Create a form to contain the Open Inventor viewer
            System.Windows.Forms.Control viewerForm = new System.Windows.Forms.Control();

            // Add the form to the forms host part of the UI.
            // Our windowsFormsHost (windowsFormsHost1) is declared in the XAML.
            this.windowsFormsHost1.Child = viewerForm;

            // Create a viewer as a child of the form
            m_viewer = new SoWinExaminerViewer(viewerForm, "OIVViewer", true,
                SoWinFullViewer.BuildFlags.BUILD_NONE, SoWinViewer.Types.BROWSER);

            // Create the initial scene graph and give it to the viewer
            SoSeparator root = CreateScene(GetConvexSettings());
            m_viewer.SetSceneGraph(root);

            // Set the initial camera position so the entire scene is visible
            // (this is done automatically if the viewer created a camera for us)
            m_viewer.ViewAll();
        }

        public void Rotate()
        {
            throw new System.NotImplementedException();
        }

    }
}
