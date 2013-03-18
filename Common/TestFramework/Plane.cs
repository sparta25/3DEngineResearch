using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TestFramework
{
    [Serializable]
    [DataContract]
    public class Plane
    {
        [DataMember]
        public List<Color> Colors { get; set; }

        [DataMember]
        public List<Point> Vertices { get; set; }
    }
}