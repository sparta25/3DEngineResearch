using TestFramework;
using OIV.Inventor;

namespace OIVCommon
{
    public static class Vec3FsToPointConvertorExt
    {
        public static Point[] ToArrayOfPoint(this SbVec3f[] vecs)
        {
            var points = new Point[vecs.Length];
            for (var i = 0; i < vecs.Length; i++)
            {
                points[i] = vecs[i].ConvertToPoint();
            }
            return points;
        }
    }
}