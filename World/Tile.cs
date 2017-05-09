using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThetaStarSharpExample.World
{
    /// <summary>
    /// Describes a tile
    /// </summary>
    public class Tile
    {
        /// <summary>
        /// The texture this tile uses
        /// </summary>
        public readonly Texture2D Texture;
        
        /// <summary>
        /// The movement modifier that is applied when walking
        /// on this tile. Must be nonnegative; 0 implies the tile
        /// is not passable.
        /// </summary>
        public readonly double MovementModifier;

        /// <summary>
        /// Creates a new tile of the specified texture and movement modifier.
        /// </summary>
        /// <param name="texture">The texture for this tile</param>
        /// <param name="movementModifier">The movement modifier if touching this tile</param>
        public Tile(Texture2D texture, double movementModifier)
        {
            Texture = texture;
            MovementModifier = movementModifier;
        }
    }
}
