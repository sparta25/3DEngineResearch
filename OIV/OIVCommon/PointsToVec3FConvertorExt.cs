using System.Collections.Generic;
using OIV.Inventor;
using TestFramework;

namespace OIVCommon
{
    public static class PointsToVec3FConvertorExt
    {
        public static SbVec3f[] ToArrayOfVec3F(this IList<Point> points)
        {
            var vec = new SbVec3f[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                vec[i] = points[i].ConvertToVec3F();
            }
            return vec;
        }
    }
}