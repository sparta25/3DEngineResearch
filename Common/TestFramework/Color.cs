using System;
using System.Runtime.Serialization;
using System.Globalization;

namespace TestFramework
{
    [Serializable]
    [DataContract]
    public struct Color
    {
        [DataMember(Name = "R")]
        public float Red { get; set; }

        [DataMember(Name = "G")]
        public float Green { get; set; }

        [DataMember(Name = "B")]
        public float Blue { get; set; }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0},{1},{2})", Red, Green, Blue);
        }
    }
}