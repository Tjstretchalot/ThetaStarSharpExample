using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ThetaStarSharpExample.Collision
{
    /// <summary>
    /// Describes a polygon as a collection of points. 
    /// </summary>
    public class Polygon2 : AbstractShape2
    {
        public static readonly Polygon2 UnitSquare = new Polygon2(new List<Vector2>
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
        });

        public readonly List<Vector2> OrderedPoints;
        public readonly List<Line2> Lines;
        public readonly List<Vector2> Normals;

        public Polygon2(List<Vector2> points)
        {
            MinX = points[0].X;
            MaxX = points[0].X;
            MinY = points[0].Y;
            MaxY = points[0].Y;

            for(int i = 1; i < points.Count; i++)
            {
                MinX = Math.Min(MinX, points[i].X);
                MaxX = Math.Max(MaxX, points[i].X);
                MinY = Math.Min(MinY, points[i].Y);
                MaxY = Math.Max(MaxY, points[i].Y);
            }

            OrderedPoints = points;
            Lines = new List<Line2>(points.Count);
            Normals = new List<Vector2>();
            for(int i = 0; i < OrderedPoints.Count; i++)
            {
                var i2 = (i == OrderedPoints.Count - 1) ? 0 : i + 1;
                var line = new Line2(points[i], points[i2]);
                Lines.Add(line);
                
                var norm = line.Normal.Value;
                norm.Normalize();
                if (!Normals.Contains(norm))
                    Normals.Add(norm);
            }
        }
        

        public override bool IntersectsWithPolygon(Polygon2 other, Vector2 myPos, Vector2 otherPos, bool strict = true)
        {
            var normalsToCheck = DecideNormals(this, other);

            foreach(var norm in normalsToCheck)
            {
                var myProj = ProjectOntoAxis(myPos, norm);
                var oProj = other.ProjectOntoAxis(otherPos, norm);

                if (!myProj.Intersects(oProj, strict))
                    return false;
            }

            return true;
        }

        public override ProjectedLine2 ProjectOntoAxis(Vector2 myPos, Vector2 axis)
        {
            var proj = Lines[0].ProjectOntoAxis(myPos, axis);
            var start = proj.Start;
            var end = proj.End;

            for(int i = 1; i < Lines.Count; i++)
            {
                proj = Lines[i].ProjectOntoAxis(myPos, axis);
                start = Math.Min(start, proj.Start);
                end = Math.Max(end, proj.End);
            }

            return new ProjectedLine2(axis, start, end);
        }

        static List<Vector2> DecideNormals(Polygon2 p1, Polygon2 p2)
        {
            var result = new List<Vector2>();
            result.AddRange(p1.Normals);

            foreach(var norm in p2.Normals)
            {
                if (!result.Contains(norm))
                    result.Add(norm);
            }

            return result;
        }
        
    }
}
