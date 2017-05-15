using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThetaStarSharpExample.World;

namespace ThetaStarSharpExample
{
    public class PathAnalyzor
    {
        public static double GetHeuristicCost(SimpleGrid grid, Entity entity, Point start, Point end)
        {
            return (end - start).ToVector2().Length();
        }

        public static double? GetActualCost(SimpleGrid grid, Entity entity, Point start, Point end)
        {
            // For right now lets assume movement speeds are either 0 or 1. If you want 
            // to add slower areas, and you want to path around them when appropriate, 
            // I suggest having the speed of the entity equal to the speed of the slowest
            // tile the entity is on. 

            return GetActualCostImpl(grid, entity, start, end, false);
        }

        public static double? GetActualCostImpl(SimpleGrid grid, Entity entity, Point start, Point end, bool addOverlays)
        {
            Vector2 entityLocation = start.ToVector2();
            Vector2 endAsVector = end.ToVector2();
            Vector2 movementDir = Vector2.Normalize(endAsVector - entityLocation);
            float theta = (float)Math.Atan2(movementDir.Y, movementDir.X);
            float sinTheta = (float)Math.Abs(Math.Sin(theta));
            float cosTheta = (float)Math.Abs(Math.Cos(theta));

            float invSinTheta = 1 / sinTheta;
            float invCosTheta = 1 / cosTheta;

            foreach (var pt in entity.Collision.GridPointsIntersectedAt(entityLocation))
            {
                if (!grid.IsInGrid(pt))
                    return null;
                if (grid[pt].MovementModifier == 0)
                    return null;
            }

            if (addOverlays)
            {
                grid.AddOverlay(entityLocation, Color.Red);
            }

            float distanceLeftSquared = (endAsVector - entityLocation).LengthSquared();
            while (distanceLeftSquared > 0.01f)
            {
                float movement = 1;
                if (movement > distanceLeftSquared * distanceLeftSquared)
                    movement = (float)Math.Sqrt(distanceLeftSquared);
                if (movementDir.X != 0 && movementDir.Y != 0)
                {
                    foreach (var pt in entity.Collision.OrderedPoints)
                    {
                        var adjX = entityLocation.X + pt.X;
                        var adjY = entityLocation.Y + pt.Y;
                        float distToX = 1;
                        if (adjX != (int)adjX)
                        {
                            distToX = movementDir.X < 0 ? adjX - (int)adjX : 1 - (adjX - (int)adjX);
                        }

                        var distAlongDirToNextX = distToX * invCosTheta;
                        movement = Math.Min(movement, distAlongDirToNextX);

                        float distToY = 1;
                        if (adjY != (int)adjY)
                        {
                            distToY = movementDir.Y < 0 ? adjY - (int)adjY : 1 - (adjY - (int)adjY);
                        }

                        var distAlongDirToNextY = distToY * invSinTheta;
                        movement = Math.Min(movement, distAlongDirToNextY);
                    }
                }

                entityLocation += movementDir * movement;
                foreach (var pt in entity.Collision.GridPointsIntersectedAt(entityLocation))
                {
                    if (!grid.IsInGrid(pt))
                        return null;
                    if (grid[pt].MovementModifier == 0)
                        return null;
                }

                if (addOverlays)
                {
                    grid.AddOverlay(entityLocation, Color.Red);
                }

                distanceLeftSquared = (endAsVector - entityLocation).LengthSquared();
            }

            entityLocation = endAsVector;
            foreach (var pt in entity.Collision.GridPointsIntersectedAt(entityLocation))
            {
                if (!grid.IsInGrid(pt))
                    return null;
                if (grid[pt].MovementModifier == 0)
                    return null;
            }

            if (addOverlays)
            {
                grid.AddOverlay(entityLocation, Color.Red);
            }

            return (end - start).ToVector2().Length();
        }

    }
}
