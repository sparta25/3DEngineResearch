using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConvexHelper
{
    public interface IStatistics
    {
        float Memory { get; set; }
        long Duration { get; set; }
    }
}
