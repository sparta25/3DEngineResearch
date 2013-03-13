using OIV.Inventor;
using TestFramework;

namespace OIVCommon
{
    public static class Vec3FToPointConvertorExt
    {
        public static Point ConvertToPoint(this SbVec3f v)
        {
            return new Point
                {
                    X = v.X,
                    Y = v.Y,
                    Z = v.Z
                };
        }
    }
}