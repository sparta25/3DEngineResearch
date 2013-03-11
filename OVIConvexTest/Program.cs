using System;
using System.Windows.Forms;
using ConvexHelper;


namespace OVIConvexTest
{
    static class Program
    {
        private static readonly string SettingsFile = System.Configuration.ConfigurationManager.AppSettings["SettingsFile"];

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Dump();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var indexedFaceSet = new IndexedFaceSet(GetConvexSettings());
            Application.Run(indexedFaceSet);
        }
        
        private static IConvexSettings GetConvexSettings()
        {
            var settings = SerializationProvider.LoadFromXml<ConvexSettings>(SettingsFile);
            return settings;
        }

        private static void Dump()
        {
            var settings = new ConvexSettings(100, 10, 10, 5, 40)
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
    }
}
