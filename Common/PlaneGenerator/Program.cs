using System;
using System.Configuration;
using PlaneGenerator.Configuration;
using TestFramework;

namespace PlaneGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var configuration = (PlaneGeneratorConfiguration) ConfigurationManager.GetSection("planeGenerator");

            var scene = configuration.FromConfiguration();

            scene.FillPlanes();
            scene.FillIndices();
            if (args.Length != 0 && args[0].ToLowerInvariant() == "json")
            {
                using (var outStream = Console.OpenStandardOutput())
                {
                    SerializationHelper.DumpToJson(outStream, scene);
                }
            }
            else
            {
                SerializationHelper.DumpToXml(Console.Out, scene);
            }
        }
    }
}