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
            Vector2 movementDir = (endAsVector - entityLocation);
            movementDir.Normalize();
            float theta = (float)Math.Atan2(movementDir.Y, movementDir.X);
            float sinTheta = (float)Math.Sin(theta);
            float cosTheta = (float)Math.Cos(theta);
            
            foreach (var pt in entity.Collision.GridPointsIntersectedAt(entityLocation))
            {
                if (!grid.IsInGrid(pt))
                    return null;
                if (grid[pt].MovementModifier == 0)
                    return null;
            }
            if(addOverlays)
            {
                grid.AddOverlay(entityLocation, Color.Red);
            }

            float distanceLeftSquared = (endAsVector - entityLocation).LengthSquared();
            while (distanceLeftSquared > 1)
            {
                float movement = 1;
                /*if (movementDir.X != 0 && movementDir.Y != 0)
                {
                    float distToX = 1;
                    if (entityLocation.X != (int)entityLocation.X)
                    {
                        distToX = movementDir.X < 0 ? entityLocation.X - (int)entityLocation.X : 1 - (entityLocation.X - (int)entityLocation.X);
                    }

                    var distAlongDirToNextX = distToX / cosTheta;
                    movement = Math.Min(movement, distAlongDirToNextX);

                    float distToY = 1;
                    if (entityLocation.Y != (int)entityLocation.Y)
                    {
                        distToY = movementDir.Y < 0 ? entityLocation.Y - (int)entityLocation.Y : 1 - (entityLocation.Y - (int)entityLocation.Y);
                    }

                    var distAlongDirToNextY = distToY / sinTheta;
                    movement = Math.Min(movement, distAlongDirToNextY);
                }*/

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
