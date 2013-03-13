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

            var scene = new ConvexSettings
                {
                    BoundaryBox = new BoundaryBox
                        {
                            Height = configuration.BoundaryBoxSize.Z,
                            Length = configuration.BoundaryBoxSize.X,
                            Width = configuration.BoundaryBoxSize.Y
                        },
                    MaxFractureSize = configuration.QuadrilateralSize.Max,
                    MinFractureSize = configuration.QuadrilateralSize.Min,
                    NumberOfPlanes = configuration.QuadrilateralCount,
                    PartHeight = configuration.GridSize.Height,
                    PartWidth = configuration.GridSize.Height
                };

            scene.FillPlanes();
            scene.FillIndices();

            SerializationHelper.DumpToXml(Console.Out, scene);
        }
    }
}