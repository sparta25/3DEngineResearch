using System;
using System.Collections.Generic;

namespace ConvexHelper
{
    public class ConvexGenerator
    {
        private static readonly Random Random = new Random();        
        
        public static IEnumerable<Quadrilateral> GenerateQuadrilaterals(IConvexSettings settings)
        {
            var list = new List<Quadrilateral>();
            for (int i = 0; i < settings.NumberOfPlanes; i++)
            {
                list.Add(GenerateRandomQuadrilateral(settings.BoundaryBox, settings.MinFractureSize, settings.MaxFractureSize));
            }

            return list;
        }

        public static Quadrilateral GenerateRandomQuadrilateral(BoundaryBox box, int minSize, int maxSize)
        {
            var sceneSize = box;
            var minSetSize = new Point { X = minSize, Y = minSize, Z = minSize };
            var setSize = new Point { X = maxSize, Y = maxSize, Z = maxSize }; 

            var topLeft = new Point
            {
                X = (float)(sceneSize.Length * Random.NextDouble()),
                Y = (float)(sceneSize.Width * Random.NextDouble()),
                Z = (float)(sceneSize.Height * Random.NextDouble())
            };

            var topRight = new Point
            {
                X = topLeft.X + BoundToAbsMin((float)(setSize.X * Random.NextDouble()), minSetSize.X),
                Y = topLeft.Y + BoundToAbsMin((float)(setSize.Y * Random.NextDouble()), minSetSize.Y),
                Z = topLeft.Z + BoundToAbsMin((float)(setSize.Z * Random.NextDouble()), minSetSize.Z)
            };
            
            var bottomLeft = new Point
            {
                X = topLeft.X + BoundToAbsMin((float)(setSize.X * Random.NextDouble()), minSetSize.X),
                Y = topLeft.Y + BoundToAbsMin((float)(setSize.Y * Random.NextDouble()), minSetSize.Y),
                Z = topLeft.Z + BoundToAbsMin((float)(setSize.Z * Random.NextDouble()), minSetSize.Z)
            };

            var middle = new Point
            {
                X = (topRight.X + bottomLeft.X) / 2f,
                Y = (topRight.Y + bottomLeft.Y) / 2f,
                Z = (topRight.Z + bottomLeft.Z) / 2f
            };

            var delta = new Point
            {
                X = middle.X - topLeft.X,
                Y = middle.Y - topLeft.Y,
                Z = middle.Z - topLeft.Z
            };

            var bottomRight = new Point
            {
                X = topLeft.X + 2 * delta.X,
                Y = topLeft.Y + 2 * delta.Y,
                Z = topLeft.Z + 2 * delta.Z
            };

            return new Quadrilateral
                {
                    TopLeft = topLeft,
                    TopRight = topRight,
                    BottomLeft = bottomLeft,
                    BottomRight = bottomRight
                };
        }
        
        public static int[] GetGridFaces(int width, int height, bool doubleSides = true)
        {
            var sides = doubleSides ? 2 : 1;
            var grid = new int[5 * sides * width * height];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int topLeft = GetVertexIndex(width, j, i);
                    int topRight = GetVertexIndex(width, j + 1, i);
                    int bottomRight = GetVertexIndex(width, j + 1, i + 1);
                    int bottomLeft = GetVertexIndex(width, j, i + 1);

                    int index = 5 * sides * (i * width + j);

                    grid[index++] = topLeft;
                    grid[index++] = topRight;
                    grid[index++] = bottomRight;
                    grid[index++] = bottomLeft;
                    grid[index++] = -1;

                    if (!doubleSides) continue;
                    grid[index++] = bottomLeft;
                    grid[index++] = bottomRight;
                    grid[index++] = topRight;
                    grid[index++] = topLeft;
                    grid[index++] = -1;
                }
            }

            return grid;
        }

        public static Point[] GetGridVertices(Quadrilateral quadrilateral, int width, int height)
        {
            var topLeft = quadrilateral.TopLeft;
            var topRight = quadrilateral.TopRight;
            var bottomLeft = quadrilateral.BottomLeft;
            var bottomRight =  quadrilateral.BottomRight;

            var vertices = new Point[(width + 1) * (height + 1)];

            for (int i = 0; i <= height; i++)
            {
                Point left = GetNthPoint(topLeft, bottomLeft, height, i);
                Point right = GetNthPoint(topRight, bottomRight, height, i);

                for (int j = 0; j <= width; j++)
                {
                    Point point = GetNthPoint(left, right, width, j);
                    vertices[i * (width + 1) + j] = point;
                }
            }

            return vertices;
        }

        public static List<Color> GetFaceColors(int width, int height, bool doubleSides = true)
        {
            var sides = doubleSides ? 2 : 1;
            var random = new Random();
            var colors = new List<Color>();
            int itemCount = sides * width * height;
            for (int i = 0; i < itemCount; i++)
                colors.Add(
                    new Color
                    {
                        Red = (float)random.NextDouble(),
                        Green = (float)random.NextDouble(),
                        Blue = (float)random.NextDouble()
                    });

            return colors;
        }
        
        private static Point GetNthPoint(Point first, Point last, int parts, int n)
        {
            var point = new Point
            {
                X = first.X + (last.X - first.X) / parts * n,
                Y = first.Y + (last.Y - first.Y) / parts * n,
                Z = first.Z + (last.Z - first.Z) / parts * n
            };
            return point;
        }

        private static int GetVertexIndex(int width, int x, int y)
        {
            return y * (width + 1) + x;
        }
        
        private static float BoundToAbsMin(float value, float min)
        {
            return value >= 0 ? value + min : value - min;
        }
        
    }
}
