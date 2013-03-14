using System;

namespace TestFramework
{
    [Serializable]
    public struct BoundaryBox
    {
        /// <summary>
        ///     Z dimension
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        ///     Y dimension
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        ///     X dimension
        /// </summary>
        public float Length { get; set; }
    }
}