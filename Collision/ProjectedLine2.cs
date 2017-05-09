using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThetaStarSharpExample.Collision
{
    /// <summary>
    /// Describes a line by how it looks when projected onto a
    /// specific axis. Note that you do NOT typically get the same
    /// line back if you project  it onto some axis and then convert 
    /// back
    /// </summary>
    public class ProjectedLine2
    {
        /// <summary>
        /// The axis this line is projected on
        /// </summary>
        public readonly Vector2 Axis;

        /// <summary>
        /// The start of this line on the axis
        /// </summary>
        public readonly float Start;

        /// <summary>
        /// The end of this line on the axis
        /// </summary>
        public readonly float End;

        /// <summary>
        /// Converts this back into standard coordinates.
        /// </summary>
        public readonly Lazy<Line2> AsLine2;

        /// <summary>
        /// Initializes a projected line on the axis with the specified
        /// starting and stopping points
        /// </summary>
        /// <param name="axis">Axis, normalized</param>
        /// <param name="start">Start</param>
        /// <param name="end">End</param>
        public ProjectedLine2(Vector2 axis, float start, float end)
        {
            Axis = axis;
            Start = start;
            End = end;

            AsLine2 = new Lazy<Line2>(ConvertToLine2);
        }

        /// <summary>
        /// Checks if this line intersects the other line. Requires that
        /// the other line uses the same axis.
        /// </summary>
        /// <param name="other">The other line</param>
        /// <param name="strict">True if overlapping is required for intersection</param>
        /// <returns>If this line intersects the other line</returns>
        public bool Intersects(ProjectedLine2 other, bool strict = true)
        {
            if(strict)
                return (Start >= other.Start && Start < other.End) || (other.Start >= Start && other.Start < End);
            else
                return (Start >= other.Start && Start <= other.End) || (other.Start >= Start && other.Start <= End);
        }

        private Line2 ConvertToLine2()
        {
            var start = new Vector2(Axis.X * Start, Axis.Y * Start);
            var end = new Vector2(Axis.X * End, Axis.Y * End);

            return new Line2(start, end);
        }
    }
}
