using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ThetaStarSharpExample.Collision
{
    /// <summary>
    /// This class enforces that each shape can handle collision with all
    /// the other implemented shapes.
    /// </summary>
    public abstract class AbstractShape2 : IShape2
    {
        public float MinX { get; protected set; }
        public float MinY { get; protected set; }
        public float MaxX { get; protected set; }
        public float MaxY { get; protected set; }

        public bool IntersectsWith(IShape2 other, Vector2 myPos, Vector2 otherPos, bool strict = true)
        {
            if(typeof(Polygon2).IsAssignableFrom(other.GetType()))
            {
                return IntersectsWithPolygon((Polygon2)other, myPos, otherPos, strict);
            }

            throw new InvalidOperationException($"Unknown shape type {other.GetType()}");
        }

        public abstract bool IntersectsWithPolygon(Polygon2 other, Vector2 myPos, Vector2 otherPos, bool strict = true);

        /// <summary>
        /// Determines which grid points are intersected when this shape is at the
        /// specified position.
        /// </summary>
        /// <remarks>
        /// This performs poorly for large shapes, regardless of how uniform they are. If
        /// you have particularly large shapes it is recommended you create a Rectangle
        /// shape, which would not need to redo collision checks for every unit square,
        /// then use those for large uniform sections and combine the nonuniform sections
        /// with a collision mesh.
        /// </remarks>
        /// <param name="pos">Position</param>
        /// <param name="strict">If touching a tile constitutes intersection</param>
        /// <returns>The tiles intersected when this shape is at the specified position.</returns>
        public virtual IEnumerable<Point> GridPointsIntersectedAt(Vector2 pos, bool strict = true)
        {
            int iPosX = (int)pos.X;
            int iPosY = (int)pos.Y;
            
            int startX = (int)MinX - 1;
            int startY = (int)MinY - 1;
            int endX = (int)Math.Ceiling(MaxX + 1);
            int endY = (int)Math.Ceiling(MaxY + 1);

            Vector2 tmp = new Vector2();
            for (int y = startY; y <= endY; y++)
            {
                tmp.Y = y + iPosY;
                for (int x = startX; x <= endX; x++)
                {
                    tmp.X = x + iPosX;
                    if (IntersectsWithPolygon(Polygon2.UnitSquare, pos, tmp, strict))
                        yield return new Point(iPosX + x, iPosY + y);
                }
            }
        }

        public abstract ProjectedLine2 ProjectOntoAxis(Vector2 myPos, Vector2 axis);
    }
}
