using C3.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using ThetaStarSharp;
using ThetaStarSharpExample.Collision;
using ThetaStarSharpExample.World;

namespace ThetaStarSharpExample
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        readonly GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D passableTexture;
        Texture2D impassableTexture;
        Texture2D pathStartTexture;
        Texture2D pathEndTexture;
        SimpleGrid grid;

        Vector2 camera;
        KeyboardState keyboardLast;
        MouseState mouseLast;

        Point? pathStart;
        Point? pathEnd;
        List<Point> currentPath;
        Entity entity;

        int zoom;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            passableTexture = new Texture2D(GraphicsDevice, 1, 1);
            passableTexture.SetData(new[] { new Color(192, 192, 192) });

            impassableTexture = new Texture2D(GraphicsDevice, 1, 1);
            impassableTexture.SetData(new[] { Color.Black });

            pathStartTexture = new Texture2D(GraphicsDevice, 1, 1);
            pathStartTexture.SetData(new[] { Color.Green });

            pathEndTexture = new Texture2D(GraphicsDevice, 1, 1);
            pathEndTexture.SetData(new[] { Color.Red });

            int width = GraphicsDevice.Viewport.Width / 24;
            int height = GraphicsDevice.Viewport.Height / 24;

            Tile[] tiles = new Tile[width * height];
            for(int i = 0; i < tiles.Length; i++)
            {
                tiles[i] = new Tile(passableTexture, 1);
            }

            grid = new SimpleGrid(width, height, tiles);
            grid.Prepare(Content, GraphicsDevice);

            camera = new Vector2();
            zoom = 24;

            entity = new Entity();
            entity.Collision = Polygon2.UnitSquare;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            grid?.Dispose();
            grid = null;

            passableTexture?.Dispose();
            passableTexture = null;

            impassableTexture?.Dispose();
            impassableTexture = null;

            pathStartTexture?.Dispose();
            pathStartTexture = null;

            pathEndTexture?.Dispose();
            pathEndTexture = null;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var keyboard = Keyboard.GetState();
            var mouse = Mouse.GetState();
            int deltaMS = (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            UpdateCamera(deltaMS, keyboard);
            HandleMouse(mouse);

            keyboardLast = keyboard;
            mouseLast = mouse;

            base.Update(gameTime);
        }

        void UpdateCamera(int deltaMS, KeyboardState keyboard)
        {
            if(keyboard.IsKeyDown(Keys.Right))
            {
                camera.X += 0.03f * deltaMS;
            }
            if (keyboard.IsKeyDown(Keys.Left))
            {
                camera.X -= 0.03f * deltaMS;
            }
            if(keyboard.IsKeyDown(Keys.Up))
            {
                camera.Y -= 0.03f * deltaMS;
            }
            if(keyboard.IsKeyDown(Keys.Down))
            {
                camera.Y += 0.03f * deltaMS;
            }
            if(keyboardLast.IsKeyUp(Keys.Z) && keyboard.IsKeyDown(Keys.Z))
            {
                zoom--;
            }
            if(keyboardLast.IsKeyUp(Keys.X) && keyboard.IsKeyDown(Keys.X))
            {
                zoom++;
            }
            if(keyboardLast.IsKeyUp(Keys.R) && keyboard.IsKeyDown(Keys.R))
            {
                if(currentPath != null)
                {
                    grid.CleanOverlays();
                    currentPath = ThetaStarPathfinder<Entity, Point, SimpleGrid>.CalculatePath(grid, PathAnalyzor.GetHeuristicCost, PathAnalyzor.GetActualCost, 1, entity, pathStart.Value, pathEnd.Value);
                   /* for (int i = 1; i < currentPath.Count; i++)
                    {
                        PathAnalyzor.GetActualCostImpl(grid, entity, currentPath[i - 1], currentPath[i], true);
                    }*/
                }
            }
        }

        void HandleMouse(MouseState mouse)
        {
            var gridPos = new Point((int)(mouse.X / (float)zoom + camera.X), (int)(mouse.Y / (float)zoom + camera.Y));
            if (!grid.IsInGrid(gridPos))
                return;

            if(mouseLast.LeftButton == ButtonState.Pressed && mouse.LeftButton == ButtonState.Released)
            {
                if(pathStart.HasValue && pathEnd.HasValue)
                {
                    grid[pathStart.Value] = new Tile(passableTexture, 1);
                    grid[pathEnd.Value] = new Tile(passableTexture, 1);
                    pathStart = null;
                    pathEnd = null;
                    currentPath = null;
                    grid.CleanOverlays();
                }else if(pathStart.HasValue)
                {
                    if(gridPos == pathStart)
                    {
                        grid[pathStart.Value] = new Tile(passableTexture, 1);
                        pathStart = null;
                    }else
                    {
                        grid[gridPos] = new Tile(pathEndTexture, 1);
                        pathEnd = gridPos;

                        currentPath = ThetaStarPathfinder<Entity, Point, SimpleGrid>.CalculatePath(grid, PathAnalyzor.GetHeuristicCost, PathAnalyzor.GetActualCost, 1.1, entity, pathStart.Value, pathEnd.Value);
                        /*for(int i = 1; i < currentPath.Count; i++)
                        {
                            PathAnalyzor.GetActualCostImpl(grid, entity, currentPath[i - 1], currentPath[i], true);
                        }*/
                    }
                }else
                {
                    grid[gridPos] = new Tile(pathStartTexture, 1);
                    pathStart = gridPos;
                }
            }else if(mouseLast.RightButton == ButtonState.Pressed && mouse.RightButton == ButtonState.Released)
            {
                if(grid[gridPos].MovementModifier == 0)
                {
                    grid[gridPos] = new Tile(passableTexture, 1);
                }else
                {
                    grid[gridPos] = new Tile(impassableTexture, 0);
                }

                if(pathStart.HasValue)
                {
                    grid[pathStart.Value] = new Tile(passableTexture, 1);
                    pathStart = null;
                }
                if(pathEnd.HasValue)
                {
                    grid[pathEnd.Value] = new Tile(passableTexture, 1);
                    pathEnd = null;
                }
                currentPath = null;
                grid.CleanOverlays();
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            grid.Draw(spriteBatch, camera, GraphicsDevice.Viewport.Bounds, zoom);
            grid.DrawGridLines(spriteBatch, camera, GraphicsDevice.Viewport.Bounds, zoom, 1, 0.8f);

            if(currentPath != null)
            {
                for(int i = 1; i < currentPath.Count; i++)
                {
                    Color color = Color.Black;
                    if (i % 3 == 0)
                        color = Color.Lime;
                    else if (i % 3 == 1)
                        color = Color.Red;
                    else
                        color = Color.Blue;

                    Primitives2D.DrawLine(spriteBatch, new Vector2(currentPath[i - 1].X * zoom + (zoom / 2f) - camera.X * zoom, currentPath[i - 1].Y * zoom + (zoom / 2f) - camera.Y * zoom), new Vector2(currentPath[i].X * zoom + (zoom / 2f) - camera.X * zoom, currentPath[i].Y * zoom + (zoom / 2f) - camera.Y * zoom), color);
                }
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
