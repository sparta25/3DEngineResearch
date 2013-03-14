using System.Collections.Generic;
using System.Linq;
using OIV.Inventor;
using TestFramework;

namespace OIVCommon
{
    public static class ColorsToVec3FConvertorExt
    {
        public static SbVec3f[] ToArrayOfVec3F(this List<Color> colors)
        {
            var vec = new SbVec3f[colors.Count()];
            for (int i = 0; i < colors.Count(); i++)
            {
                vec[i] = colors[i].ConvertToVec3F();
            }
            return vec;
        }
    }
}