using System.Configuration;

using PlaneGenerator.Configuration;
using ConvexHelper;
using System.Xml.Serialization;
using System;

namespace PlaneGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            PlaneGeneratorConfiguration configuration =
                (PlaneGeneratorConfiguration)ConfigurationManager.GetSection("planeGenerator");

            var scene = new ConvexSettings {
                BoundaryBox = new BoundaryBox {
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

            scene.FillVertices();
            scene.FillIndices();
            scene.FillColors();

            var serializer = new XmlSerializer(typeof(ConvexSettings));
            serializer.Serialize(Console.Out, scene);
        }
    }
}
