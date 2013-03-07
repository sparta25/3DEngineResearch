using System.Configuration;

namespace PlaneGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            PlaneGeneratorConfiguration configuration = (PlaneGeneratorConfiguration)ConfigurationManager.GetSection("PlaneGenerator");
            /*for (int i = 0; i < 100; i++)
            {
                SoIndexedFaceSet faceSet = GetRandomFaceSet();
                _root.AddChild(faceSet);
            }
            _viewer.SetSceneGraph(_root);
            _viewer.ViewAll();

            _settings.Vertices = Points;
            SerializationProvider.DumpToXml(System.Configuration.ConfigurationManager.AppSettings["SettingsFile"], _settings as ConvexSettings);*/
        }
    }
}
