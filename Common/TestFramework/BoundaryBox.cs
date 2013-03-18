using System;
using System.Runtime.Serialization;

namespace TestFramework
{
    [Serializable]
    [DataContract]
    public struct BoundaryBox
    {
        /// <summary>
        ///     Z dimension
        /// </summary>
        [DataMember]
        public float Height { get; set; }

        /// <summary>
        ///     Y dimension
        /// </summary>
        [DataMember]
        public float Width { get; set; }

        /// <summary>
        ///     X dimension
        /// </summary>
        [DataMember]
        public float Length { get; set; }
    }
}