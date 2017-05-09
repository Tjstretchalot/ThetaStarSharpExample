using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace ThetaStarSharpExample.Collision
{
    /// <summary>
    /// Describes a shape that is capable of determining intersection with other shapes. For the sake 
    /// of this example we will not consider rotation, although it isn't particularly difficult to add.
    /// 
    /// Similiarly it would be reasonable to add MTVs here, but this project doesn't need it
    /// </summary>
    public interface IShape2
    {
        float MinX { get; }
        float MinY { get; }
        float MaxX { get; }
        float MaxY { get; }

        /// <summary>
        /// Determines if this shape intersects with the other shape.
        /// </summary>
        /// <param name="other">The other shape</param>
        /// <param name="myPos">Where this shape is located</param>
        /// <param name="otherPos">Where the other shape is located</param>
        /// <param name="strict">True if overlapping is required for intersection</param>
        /// <returns>if this shape intersects the other shape when both are at their specified positions</returns>
        bool IntersectsWith(IShape2 other, Vector2 myPos, Vector2 otherPos, bool strict = true);

        /// <summary>
        /// Determines what grid points are intersected when this shape is at the specified
        /// position. Grid points are unit squares covering the entire 2d plane with no overlap,
        /// with one unit square at top-left (0, 0)
        /// </summary>
        /// <param name="pos">Where this shape is located</param>
        /// <param name="strict">If touching a grid position constitutes intersecting it</param>
        /// <returns>The grid points intersected when this shape is at the specified position</returns>
        IEnumerable<Point> GridPointsIntersectedAt(Vector2 pos, bool strict = true);

        /// <summary>
        /// Projects this shape onto the specified axis
        /// </summary>
        /// <param name="myPos">The position of the shape</param>
        /// <param name="axis">The axis to project onto</param>
        /// <returns>This shape projected onto the specified axis</returns>
        ProjectedLine2 ProjectOntoAxis(Vector2 myPos, Vector2 axis);
    }
}