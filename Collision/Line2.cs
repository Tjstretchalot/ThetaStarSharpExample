using Microsoft.Xna.Framework;
using System;

namespace ThetaStarSharpExample.Collision
{
    /// <summary>
    /// A line is a collection of two points
    /// </summary>
    public class Line2
    {
        /// <summary>
        /// The start of the line
        /// </summary>
        public readonly Vector2 Start;

        /// <summary>
        /// The end of the line
        /// </summary>
        public readonly Vector2 End;
        
        /// <summary>
        /// End - Start
        /// </summary>
        public readonly Lazy<Vector2> Delta;

        /// <summary>
        /// The magnitude of this line, squared
        /// </summary>
        public readonly Lazy<float> MagnitudeSquared;

        /// <summary>
        /// The magnitude of this line
        /// </summary>
        public readonly Lazy<float> Magnitude;

        /// <summary>
        /// A normal of this line
        /// </summary>
        public readonly Lazy<Vector2> Normal;

        /// <summary>
        /// Initializes a line from start to end
        /// </summary>
        /// <param name="start">Start of the line</param>
        /// <param name="end">End of the line</param>
        public Line2(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;

            Delta = new Lazy<Vector2>(() => End - Start);
            MagnitudeSquared = new Lazy<float>(() => (End - Start).LengthSquared());
            Magnitude = new Lazy<float>(() => (float)Math.Sqrt(MagnitudeSquared.Value));
            Normal = new Lazy<Vector2>(() => new Vector2(-Delta.Value.Y, Delta.Value.X));
        }

        /// <summary>
        /// Projects the version of this line at myPos onto the specified axis.
        /// </summary>
        /// <param name="myPos">The position of this line</param>
        /// <param name="axis">The axis to project onto</param>
        /// <returns>The projected line</returns>
        public ProjectedLine2 ProjectOntoAxis(Vector2 myPos, Vector2 axis)
        {
            return new ProjectedLine2(axis, DotProduct(Start + myPos, axis), DotProduct(End + myPos, axis));
        }
        
        static float DotProduct(Vector2 v1, Vector2 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }
    }
}
