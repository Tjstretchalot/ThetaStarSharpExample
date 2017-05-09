using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThetaStarSharpExample.Collision;

namespace ThetaStarSharpExample.World
{
    /// <summary>
    /// A basic entity.
    /// </summary>
    public class Entity
    {
        public Texture2D Texture;
        public Vector2 Position;
        public IShape2 Collision;
    }
}
