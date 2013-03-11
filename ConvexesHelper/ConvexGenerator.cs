using System;
using System.Collections.Generic;

namespace ConvexHelper
{
    public class ConvexGenerator
    {
        private static readonly Random _random = new Random();

        private static float GetRandomFloat()
        {
            return (float)_random.NextDouble();
        }

        private static float GetRandomFloat(float max)
        {
            return GetRandomFloat() * max;
        }

        private static float GetRandomFloat(float min, float max)
        {
            return GetRandomFloat(max - min) + min;
        }

        private static float GetRandomFloatInDoubleRange(float min, float max)
        {
            return (_random.Next(2) == 0 ? -1 : 1) * GetRandomFloat(min, max);
        }

        public static IEnumerable<Quadrilateral> GenerateQuadrilaterals(IConvexSettings settings)
        {
            var list = new List<Quadrilateral>();
            for (int i = 0; i < settings.NumberOfPlanes; i++)
            {
                list.Add(GenerateRandomQuadrilateral(settings.BoundaryBox, settings.MinFractureSize, settings.MaxFractureSize));
            }

            return list;
        }

        public static Quadrilateral GenerateRandomQuadrilateral(BoundaryBox box, float minSize, float maxSize)
        {
            var sceneSize = new BoundaryBox {
                Length = box.Length - 2 * maxSize,
                Width = box.Width - 2 * maxSize,
                Height = box.Height - 2 * maxSize
            };

            var minSetSize = new Point { X = minSize, Y = minSize, Z = minSize };
            var setSize = new Point { X = maxSize, Y = maxSize, Z = maxSize };

            var topLeft = new Point {
                X = maxSize + GetRandomFloat(sceneSize.Length),
                Y = maxSize + GetRandomFloat(sceneSize.Width),
                Z = maxSize + GetRandomFloat(sceneSize.Height)
            };

            var topRight = new Point {
                X = topLeft.X + GetRandomFloatInDoubleRange(minSize, maxSize),
                Y = topLeft.Y + GetRandomFloatInDoubleRange(minSize, maxSize),
                Z = topLeft.Z + GetRandomFloatInDoubleRange(minSize, maxSize)
            };
            
            var bottomLeft = new Point {
                X = topLeft.X + GetRandomFloatInDoubleRange(minSize, maxSize),
                Y = topLeft.Y + GetRandomFloatInDoubleRange(minSize, maxSize),
                Z = topLeft.Z + GetRandomFloatInDoubleRange(minSize, maxSize)
            };

            float intersectionRatio1 = GetRandomFloat(0.2f, 0.8f);
            var intersection = new Point
            {
                X = bottomLeft.X + (topRight.X - bottomLeft.X) * intersectionRatio1,
                Y = bottomLeft.Y + (topRight.Y - bottomLeft.Y) * intersectionRatio1,
                Z = bottomLeft.Z + (topRight.Z - bottomLeft.Z) * intersectionRatio1
            };

            var delta = new Point
            {
                X = intersection.X - topLeft.X,
                Y = intersection.Y - topLeft.Y,
                Z = intersection.Z - topLeft.Z
            };

            float intersectionRatio2 = GetRandomFloat(1.2f, 2.0f);
            var bottomRight = new Point
            {
                X = topLeft.X + (intersection.X - topLeft.X) * intersectionRatio2,
                Y = topLeft.Y + (intersection.Y - topLeft.Y) * intersectionRatio2,
                Z = topLeft.Z + (intersection.Z - topLeft.Z) * intersectionRatio2
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

        private static float ConstraintToRange(float value, float min, float max)
        {
            if (value < min)
                return min;

            if (value > max)
                return max;

            return value;
        }

        private static Color GetRandomColor()
        {
            return new Color {
                Red = GetRandomFloat(),
                Green = GetRandomFloat(),
                Blue = GetRandomFloat()
            };
        }

        public static List<Color> GetFaceColors(int width, int height, bool doubleSides = true)
        {
            var sides = doubleSides ? 2 : 1;
            var colors = new List<Color>();
            int itemCount = sides * width * height;

            Color primaryColor = GetRandomColor();

            for (int i = 0; i < itemCount; i++)
                colors.Add(
                    new Color
                    {
                        Red = ConstraintToRange(primaryColor.Red + GetRandomFloat(-0.1f, 0.1f), 0, 1),
                        Green = ConstraintToRange(primaryColor.Green + GetRandomFloat(-0.1f, 0.1f), 0, 1),
                        Blue = ConstraintToRange(primaryColor.Blue + GetRandomFloat(-0.1f, 0.1f), 0, 1),
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
