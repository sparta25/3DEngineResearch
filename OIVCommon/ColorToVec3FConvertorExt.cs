using TestFramework;
using OIV.Inventor;

namespace OIVCommon
{
    public static class ColorToVec3FConvertorExt
    {
        public static SbVec3f ConvertToVec3F(this Color c)
        {
            return new SbVec3f(c.Red, c.Green, c.Blue);
        }
    }
}