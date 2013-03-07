using System.Collections.Generic;

namespace ConvexHelper
{
    public static class ConvexSettingsEx
    {
        public static void FillIndices(this ConvexSettings settings)
        {
            settings.Indices = new List<int>();
            settings.Indices.AddRange(ConvexGenerator.GetGridFaces(settings.PartWidth, settings.PartHeight));
        }

        public static void FillColors(this ConvexSettings settings)
        {
            settings.Colors = ConvexGenerator.GetFaceColors(settings.PartWidth, settings.PartHeight);
        }
        
        public static void FillVertices(this ConvexSettings settings)
        {
            settings.Vertices = new List<Point>();
            var quadrilaterals = ConvexGenerator.GenerateQuadrilaterals(settings);
            foreach (var quad in quadrilaterals)
            {
                var vertices = ConvexGenerator.GetGridVertices(quad, settings.PartWidth, settings.PartHeight);
                settings.Vertices.AddRange(vertices);    
            }
        }

    }
}