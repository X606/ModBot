using UnityEngine;

namespace InternalModBot
{
    /// <summary>
    /// Describes a line in 3D space
    /// </summary>
    internal class LineInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineInfo"/> class
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="color"></param>
        /// <param name="endTime"></param>
        public LineInfo(Vector3 point1, Vector3 point2, Color color, float endTime)
        {
            Point1 = point1;
            Point2 = point2;
            Color = color;
            EndTime = endTime;
        }

        /// <summary>
        /// The start point of the line
        /// </summary>
        public Vector3 Point1;

        /// <summary>
        /// The end point of the line
        /// </summary>
        public Vector3 Point2;

        /// <summary>
        /// The <see cref="UnityEngine.Color"/> of the line
        /// </summary>
        public Color Color;

        /// <summary>
        /// The time we want to stop displaying the line
        /// </summary>
        public float EndTime;
    }
}
