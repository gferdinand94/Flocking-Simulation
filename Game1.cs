using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace FlockingSimulation
{
    public class Game1 : Game
    {

        Texture2D tex, predTex;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D line;
        private int resolution = 60;//30;
        private int rows, cols;
        private List<List<List<Bird>>> grid;
        private Bird[] birds;
        private Predator predator;
        private Random rand = new Random();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.ApplyChanges();

            tex = Content.Load<Texture2D>("Arrow");
            predTex = Content.Load<Texture2D>("Arrow_White");

            rows = _graphics.PreferredBackBufferHeight / resolution;
            cols = _graphics.PreferredBackBufferWidth / resolution;

            // Line texture for drawing grid
            line = new Texture2D(_graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            line.SetData(new[] { Color.Gray });

            // Several Birds
            birds = new Bird[1000];

            predator = new Predator(
                rand.Next(150, _graphics.PreferredBackBufferWidth - 150),
                rand.Next(150, _graphics.PreferredBackBufferHeight - 150),
                predTex);

            for (int i = 0; i < birds.Length; i++)
            {
                birds[i] = new Bird(
                    rand.Next(150, _graphics.PreferredBackBufferWidth - 150),
                    rand.Next(150, _graphics.PreferredBackBufferHeight - 150),
                    tex);
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.W))
            {
                predator.goUp(gameTime);
            }
            if (state.IsKeyDown(Keys.S))
            {
                predator.goDown(gameTime);
            }
            if (state.IsKeyDown(Keys.D))
            {
                predator.goRight(gameTime);
            }
            if (state.IsKeyDown(Keys.A))
            {
                predator.goLeft(gameTime);
            }

            grid = new List<List<List<Bird>>>();

            for (int c = 0; c < cols; c++)
            {
                grid.Add(new List<List<Bird>>());
                for (int r = 0; r < rows; r++)
                {
                    grid[c].Add(new List<Bird>());
                }
            }

            for (int i = 0; i < birds.Length; i++)
            {
                grid[birds[i].getCol(resolution, _graphics)][birds[i].getRow(resolution, _graphics)].Add(birds[i]);
            }

            predator.Update(gameTime);
            predator.wrapEdges(_graphics);

            for (int i = 0; i < birds.Length; i++)
            {
                birds[i].Update(gameTime);
                birds[i].checkEdges(_graphics, gameTime);
                birds[i].flock(i, grid, resolution, _graphics);
                birds[i].checkForPredator(_graphics, gameTime, predator);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();


            /*
             * Code for drawing a grid
             */

            ////Vertical grid lines
            //for (int i = 0; i < _graphics.PreferredBackBufferWidth; i += resolution)
            //{
            //    _spriteBatch.Draw(line, new Rectangle(i, 1, 1, _graphics.PreferredBackBufferHeight), Color.Gray);
            //}

            //// Horizontal grid lines
            //for (int i = 0; i < _graphics.PreferredBackBufferHeight; i += resolution)
            //{
            //    _spriteBatch.Draw(line, new Rectangle(1, i, _graphics.PreferredBackBufferWidth, 1), Color.Gray);
            //}

            /*
             * Code for drawing a grid
             */


            for (int i = 0; i < birds.Length; i++)
            {
                birds[i].Draw(_spriteBatch);
            }

            predator.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
