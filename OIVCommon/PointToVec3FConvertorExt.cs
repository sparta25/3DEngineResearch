using ConvexHelper;
using OIV.Inventor;

namespace OIVCommon
{
    public static class PointToVec3FConvertorExt
    {
        public static SbVec3f ConvertToVec3F(this Point p )
        {
            return new SbVec3f(p.X,p.Y, p.Z);
        }
    }
}