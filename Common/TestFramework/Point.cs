using System;
using System.Runtime.Serialization;

namespace TestFramework
{
    [Serializable]
    [DataContract]
    public struct Point
    {
        [DataMember]
        public float X { get; set; }

        [DataMember]
        public float Y { get; set; }

        [DataMember]
        public float Z { get; set; }
    }
}