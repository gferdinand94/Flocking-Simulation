using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlockingSimulation
{
    class Predator : Animal
    {
        private Random rand = new Random();

        private Vector2 right = new Vector2(800f, 0);
        private Vector2 left = new Vector2(-800f, 0);
        private Vector2 up = new Vector2(0, -800f);
        private Vector2 down = new Vector2(0, 800f);

        public Predator(float x, float y, Texture2D t)
        {
            topSpeed = 300f;
            bottomSpeed = 150f;
            mass = 1;
            position = new Vector2(x, y);
            texture = t;
            velocity = new Vector2((float)rand.Next(-150, 150), (float)rand.Next(-150, 150));
            acceleration = new Vector2(0f, 0f);
            angle = 0f;
        }

        public void Update(GameTime gameTime)

        {

            boostSpeed(gameTime);
            velocity += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            acceleration *= 0;
            limitSpeed(gameTime);

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Call heading to update the angle of the Bird
            heading();

            // Draw the texture using current position and angle, and adjust scale to 20%
            spriteBatch.Draw(
                texture,
                position,
                null,
                Color.DarkRed,
                angle,
                new Vector2(texture.Width / 2, texture.Height / 2),
                0.5f,
                SpriteEffects.None,
                0f);
        }


        // Changing the Predator's direction and speed based on keyboard input.
        public void goRight(GameTime gameTime)
        {
            acceleration += right;
        }

        public void goLeft(GameTime gameTime)
        {
            acceleration += left;
        }

        public void goUp(GameTime gameTime)
        {
            acceleration += up;
        }

        public void goDown(GameTime gameTime)
        {
            acceleration += down;
        }

        // Wraps edges so that if the Predator object goes off screen it comes back on the opposite side.
        public void wrapEdges(GraphicsDeviceManager _graphics)
        {
            if (position.X < 0)
                position.X = _graphics.PreferredBackBufferWidth;
            if (position.X > _graphics.PreferredBackBufferWidth)
                position.X = 0;
            if (position.Y < 0)
                position.Y = _graphics.PreferredBackBufferHeight;
            if (position.Y > _graphics.PreferredBackBufferHeight)
                position.Y = 0;
        }
    }
}
