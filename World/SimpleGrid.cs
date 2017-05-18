using C3.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ThetaStarSharp;

namespace ThetaStarSharpExample.World
{
    /// <summary>
    /// This describes a simple grid built of tiles.
    /// </summary>
    public class SimpleGrid : IThetaGridAdapter<Entity, Point>
    {
        /// <summary>
        /// The width of this grid in tiles
        /// </summary>
        public readonly int WidthInTiles;

        /// <summary>
        /// The height of this grid in tiles
        /// </summary>
        public readonly int HeightInTiles;

        /// <summary>
        /// The grid of tiles, such that index 0 is (0, 0), index 1 is (1, 0), etc.
        /// </summary>
        protected Tile[] Tiles;

        /// <summary>
        /// The grid line texture
        /// </summary>
        protected Texture2D GridLineTexture;

        /// <summary>
        /// The overlays that are being shown on the map.
        /// </summary>
        protected List<Tuple<Vector2, Color>> Overlays;

        /// <summary>
        /// Initializes a simple grid of tiles with the specified width, height, and actual tiles.
        /// </summary>
        /// <param name="widthInTiles">The width of the grid in tiles</param>
        /// <param name="heightInTiles">The height of the grid in tiles</param>
        /// <param name="tiles">The tiles</param>
        public SimpleGrid(int widthInTiles, int heightInTiles, Tile[] tiles)
        {
            WidthInTiles = widthInTiles;
            HeightInTiles = heightInTiles;
            Tiles = tiles;
            Overlays = new List<Tuple<Vector2, Color>>();
        }

        public Tile this[int i]
        {
            get { return Tiles[i]; }
            set { Tiles[i] = value; }
        }

        public Tile this[int x, int y]
        {
            get { return Tiles[y * WidthInTiles + x]; }
            set { Tiles[y * WidthInTiles + x] = value; }
        }

        public Tile this[Point p]
        {
            get { return this[p.X, p.Y]; }
            set { this[p.X, p.Y] = value; }
        }

        /// <summary>
        /// Prepares this simple grid for drawing
        /// </summary>
        /// <param name="content">The content manager</param>
        /// <param name="graphicsDevice">Graphics device</param>
        public void Prepare(ContentManager content, GraphicsDevice graphicsDevice)
        {
            GridLineTexture = new Texture2D(graphicsDevice, 1, 1);
            GridLineTexture.SetData(new[] { Color.Gray });
        }

        /// <summary>
        /// Draws the grid at the specified zoom level.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to use</param>
        /// <param name="camera">Where the camera is located</param>
        /// <param name="zoom">The pixel width and height of each tile.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition, Rectangle viewport, int zoom)
        {
            int tileStartX = (int)cameraPosition.X;
            int tileStartY = (int)cameraPosition.Y;
            int tileEndX = (int)Math.Ceiling(cameraPosition.X + viewport.Width);
            int tileEndY = (int)Math.Ceiling(cameraPosition.Y + viewport.Height);

            tileStartX = Math.Max(tileStartX, 0);
            tileStartY = Math.Max(tileStartY, 0);
            tileEndX = Math.Min(tileEndX, WidthInTiles - 1);
            tileEndY = Math.Min(tileEndY, HeightInTiles - 1);
            
            int offsetBetweenRows = WidthInTiles - 1 - tileEndX + tileStartX;

            int currentIndex = IndexOfTileAt(tileStartX, tileStartY);

            int startPixelX = (int)(tileStartX * zoom - cameraPosition.X * zoom);
            int startPixelY = (int)(tileStartY * zoom - cameraPosition.Y * zoom);
            
            Rectangle destRect = new Rectangle(startPixelX, startPixelY, zoom, zoom);
            var pt = new Point(0, 0);
            for(int y = tileStartY; y <= tileEndY; y++)
            {
                for(int x = tileStartX; x <= tileEndX; x++)
                {
                    var tile = Tiles[currentIndex];
                    spriteBatch.Draw(tile.Texture, destRect, Color.White);

                    currentIndex++;
                    destRect.X += zoom;
                }

                currentIndex += offsetBetweenRows;
                destRect.X = startPixelX;
                destRect.Y += zoom;
            }

            foreach(var overlay in Overlays)
            {
                destRect = new Rectangle((int)(overlay.Item1.X * zoom - cameraPosition.X * zoom), (int)(overlay.Item1.Y * zoom - cameraPosition.Y * zoom), zoom, zoom);
                spriteBatch.Draw(Tiles[0].Texture, destRect, overlay.Item2 * 0.3f);
            }
        }

        /// <summary>
        /// Draws grid lines for the grid.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch</param>
        /// <param name="cameraPosition">Where the camera is located (top-left)</param>
        /// <param name="viewport">Viewport bounds</param>
        /// <param name="zoom">Pixels per tile</param>
        /// <param name="lineWidth">Width of grid lines in pixels</param>
        public void DrawGridLines(SpriteBatch spriteBatch, Vector2 cameraPosition, Rectangle viewport, int zoom, int lineWidth, float opacity)
        {
            int tileStartX = (int)cameraPosition.X;
            int tileStartY = (int)cameraPosition.Y;
            int tileEndX = (int)Math.Ceiling(cameraPosition.X + viewport.Width);
            int tileEndY = (int)Math.Ceiling(cameraPosition.Y + viewport.Height);

            tileStartX = Math.Max(tileStartX, 0);
            tileStartY = Math.Max(tileStartY, 0);
            tileEndX = Math.Min(tileEndX + 1, WidthInTiles);
            tileEndY = Math.Min(tileEndY + 1, HeightInTiles);
            

            // Vertical lines first
            int startPixelX = (int)(tileStartX * zoom - cameraPosition.X * zoom);
            int startPixelY = (int)Math.Max(0, tileStartY * zoom - cameraPosition.Y * zoom);
            int pixelWidth = lineWidth;
            int pixelHeight = (int)Math.Min(viewport.Height, tileEndY * zoom - cameraPosition.Y * zoom) - startPixelY;
            var color = Color.White * opacity;
            Rectangle destRect = new Rectangle(startPixelX, startPixelY, pixelWidth, pixelHeight);
            for(int tileX = tileStartX; tileX <= tileEndX; tileX++)
            {
                spriteBatch.Draw(GridLineTexture, destRect, color);

                destRect.X += zoom;
            }

            // Horizontal lines
            startPixelX = (int)Math.Max(0, tileStartX * zoom - cameraPosition.X * zoom);
            startPixelY = (int)(tileStartY * zoom - cameraPosition.Y * zoom);
            pixelWidth = (int)Math.Min(viewport.Width, tileEndX * zoom - cameraPosition.X * zoom) - startPixelX;
            pixelHeight = lineWidth;
            destRect.X = startPixelX;
            destRect.Y = startPixelY;
            destRect.Width = pixelWidth;
            destRect.Height = pixelHeight;
            for(int tileY = tileStartY; tileY <= tileEndY; tileY++)
            {
                spriteBatch.Draw(GridLineTexture, destRect, color);

                destRect.Y += zoom;
            }
        }

        /// <summary>
        /// Disposes of any unmanaged resources
        /// </summary>
        public void Dispose()
        {
            GridLineTexture?.Dispose();
            GridLineTexture = null;
        }

        public void CleanOverlays()
        {
            Overlays.Clear();
        }

        public void AddOverlay(Vector2 pt, Color color)
        {
            Overlays.Add(Tuple.Create(pt, color));
        }

        /// <summary>
        /// Calculates the index of the tile at the specified position.
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns>The index in Tiles of the tile at that position</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOfTileAt(int x, int y)
        {
            return y * WidthInTiles + x;
        }

        /// <summary>
        /// Determines if the specified position is inside the grid
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns>If that position is in the grid</returns>
        public bool IsInGrid(int x, int y)
        {
            return x >= 0 && x < WidthInTiles && y >= 0 && y < HeightInTiles;
        }

        /// <summary>
        /// Determines if the specified position is inside the grid
        /// </summary>
        /// <param name="p">Position</param>
        /// <returns>If p is inside the grid</returns>
        public bool IsInGrid(Point p)
        {
            return IsInGrid(p.X, p.Y);
        }

        public IEnumerable<Point> GetNodesAdjacentTo(Entity ent, Point position)
        {
            // For the sake of simplicity lets assume entity is a 1x1 tile. 

            Point tmp;
            if (position.X > 0)
            {
                tmp = new Point(position.X - 1, position.Y);
                if (this[tmp].MovementModifier != 0) // left empty
                {
                    yield return tmp; // left

                    if (position.Y > 0)
                    {
                        tmp = new Point(position.X - 1, position.Y - 1); // upper left

                        if (this[tmp].MovementModifier != 0 // upper left empty
                            && this[position.X, position.Y - 1].MovementModifier != 0) // up empty
                            yield return tmp;
                    }

                    if (position.Y < HeightInTiles - 1)
                    {
                        tmp = new Point(position.X - 1, position.Y + 1); // lower left
                        if (this[tmp].MovementModifier != 0 // lower left empty
                            && this[position.X, position.Y + 1].MovementModifier != 0) // down empty
                            yield return tmp;
                    }
                }
            }

            if (position.Y > 0)
            {
                tmp = new Point(position.X, position.Y - 1);
                if (this[tmp].MovementModifier != 0)
                    yield return tmp; // above
            }

            if (position.Y < HeightInTiles - 1)
            {
                tmp = new Point(position.X, position.Y + 1);
                if (this[tmp].MovementModifier != 0)
                    yield return tmp; // below
            }

            if (position.X < WidthInTiles - 1)
            {
                tmp = new Point(position.X + 1, position.Y);
                if (this[tmp].MovementModifier != 0)
                { // right empty
                    yield return tmp; //right

                    if (position.Y > 0)
                    {
                        tmp = new Point(position.X + 1, position.Y - 1); // upper right
                        if (this[tmp].MovementModifier != 0 // upper right empty 
                            && this[position.X, position.Y - 1].MovementModifier != 0) // up empty
                            yield return tmp;
                    }

                    if (position.Y < HeightInTiles - 1)
                    {
                        tmp = new Point(position.X + 1, position.Y + 1); // lower right
                        if (this[tmp].MovementModifier != 0 // lower right empty
                            && this[position.X, position.Y + 1].MovementModifier != 0) // down empty
                            yield return tmp;
                    }
                }
            }
        }

        public void OnExpanding(Point p)
        {
            AddOverlay(p.ToVector2(), Color.Green);
        }
    }
}