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

            SerializationHelper.DumpToXml(Console.Out, scene);
        }
    }
}