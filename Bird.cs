using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlockingSimulation
{
    class Bird : Animal
    {

        // variables unique to Bird objects
        private float visibleRange = 150f;
        private float protectedRange = 25f;
        private float avoid = 0.04f;
        private float matchSpeed = 0.035f;
        private float centering = 0.0004f;
        private float topMargin, bottomMargin, leftMargin, rightMargin;
        private Random rand = new Random();
        int totalRows; int totalCols;


        public Bird(float x, float y, Texture2D t)
        {
            topSpeed = 450f;
            bottomSpeed = 300f;
            mass = 1;
            position = new Vector2(x, y);
            texture = t;
            velocity = new Vector2((float)rand.Next(-150, 150), (float)rand.Next(-150, 150));
            acceleration = new Vector2(0f, 0f);
            angle = 0f;
        }

        public void Update(GameTime gameTime)
        {

            accel(gameTime);
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
                Color.White,
                angle,
                new Vector2(texture.Width / 2, texture.Height / 2),
                0.2f,
                SpriteEffects.None,
                0f);
        }

        // Used for getting the current row of the Bird for spatial hashing optimization
        public int getRow(int resolution, GraphicsDeviceManager _graphics)
        {
            int row = (int)Math.Floor((double)(position.Y / resolution));
            totalRows = (int)Math.Floor((double)(_graphics.PreferredBackBufferHeight / resolution));

            if (row < 0)
            {
                return 0;
            }

            if (row >= totalRows)
            {
                return totalRows - 1;
            }

            return row;
        }

        // Used for getting the current column of the Bird for spatial hashing optimization
        public int getCol(int resolution, GraphicsDeviceManager _graphics)
        {
            int col = (int)Math.Floor((double)(position.X / resolution));
            totalCols = (int)Math.Floor((double)(_graphics.PreferredBackBufferWidth / resolution));

            if (col < 0)
            {
                return 0;
            }

            if (col >= totalCols)
            {
                return totalCols - 1;
            }

            return col;
        }

        /* Flocking algorithm, based on the 'Boids' algorithm developed by Craig Reynolds.
        *  Based on the distance to nearby "birds", each Bird object will accumulate information about avoidance, speed matching, and centering.
        *  If Bird objects are too close, they will fly away from each other. If they are within an acceptable distance, they will attempt to match
        *  each others' speeds. If they are within an acceptable distance they will attempt to come to a 'centering' position. Each Bird object
        *  takes account of the Bird objects surrounding it and averages each of these values. When every Bird object does this to every other
        *  Bird object, it generates a larger flocking behavior. The values for each of these parameters are hard coded in (near the top of the
        *  program), but can be changed to a desired effect.
        */
        public void flock(int thisBoid, List<List<List<Bird>>> grid, int resolution, GraphicsDeviceManager _graphics)
        {
            // Setting variables for determining flocking behavior
            float tooCloseX = 0f;
            float tooCloseY = 0f;
            float avgVelX = 0f;
            float avgVelY = 0f;
            float avgPosX = 0f;
            float avgPosY = 0f;
            int neighbors = 0;

            // Setting up the neighbor list so that each Bird object can look at its neighbors (within a defined distance)
            List<Bird> neighborsList = new List<Bird>();

            for (int i = -1; i < 2; i++)
            {
                int nextRow = getRow(resolution, _graphics) + i;

                for (int j = -1; j < 2; j++)
                {
                    int nextCol = getCol(resolution, _graphics) + j;

                    if ((nextRow >= 0) && (nextRow < totalRows) && (nextCol >= 0) && (nextCol < totalCols))
                    {
                        neighborsList.AddRange(grid[nextCol][nextRow]);
                    }
                }
            }

            // Look for neighbors and accumulate values for avoidance, matching, and centering behaviors
            for (int i = 0; i < neighborsList.Count; i++)
            {
                if (Vector2.Distance(position, neighborsList[i].position) < protectedRange && thisBoid != i)
                {
                    tooCloseX += position.X - neighborsList[i].position.X;
                    tooCloseY += position.Y - neighborsList[i].position.Y;
                }

                if (Vector2.Distance(position, neighborsList[i].position) < visibleRange && thisBoid != i)
                {
                    avgPosX += neighborsList[i].position.X;
                    avgPosY += neighborsList[i].position.Y;

                    avgVelX += neighborsList[i].velocity.X;
                    avgVelY += neighborsList[i].velocity.Y;

                    neighbors++;
                }

            }

            // If the Bird object has neighbors, apply the avoidance, matching, and centering behaviors
            if (neighbors > 0)
            {
                avgPosX /= neighbors;
                avgPosY /= neighbors;

                avgVelX /= neighbors;
                avgVelY /= neighbors;

                velocity.X += tooCloseX * avoid;
                velocity.Y += tooCloseY * avoid;

                velocity.X += (avgVelX - velocity.X) * matchSpeed;
                velocity.Y += (avgVelY - velocity.Y) * matchSpeed;

                velocity.X += (avgPosX - position.X) * centering;
                velocity.Y += (avgPosY - position.Y) * centering;
            }

        }

        // Checks edges of the screen and turns each Bird object back so that it stays within the screen.
        public void checkEdges(GraphicsDeviceManager _graphics, GameTime g)
        {
            bool outOfBounds = false;
            topMargin = 50f;
            bottomMargin = _graphics.PreferredBackBufferHeight - 50f;
            leftMargin = 50f;
            rightMargin = _graphics.PreferredBackBufferWidth - 50f;

            if (position.X < leftMargin)
            {
                velocity.X += 500f * (float)g.ElapsedGameTime.TotalSeconds;
                outOfBounds = true;
            }
            if (position.X > rightMargin)
            {
                velocity.X -= 500f * (float)g.ElapsedGameTime.TotalSeconds;
                outOfBounds = true;
            }
            if (position.Y > bottomMargin)
            {
                velocity.Y -= 500f * (float)g.ElapsedGameTime.TotalSeconds;
                outOfBounds = true;
            }
            if (position.Y < topMargin)
            {
                velocity.Y += 500f * (float)g.ElapsedGameTime.TotalSeconds;
                outOfBounds = true;
            }

            if (outOfBounds)
            {
                velocity *= 0.99f;
            }

        }

        // Checks for the 'predator' which is controlled by a user.
        public void checkForPredator(GraphicsDeviceManager _graphics, GameTime g, Predator predator)
        {
            bool tooClose = false;
            float distanceToPredator = Vector2.Distance(position, predator.position);

            if (distanceToPredator < 100f) 
            { 
                if (position.X < predator.position.X)
                {
                    velocity.X -= 1200f * (float)g.ElapsedGameTime.TotalSeconds;
                    tooClose = true;
                }
                if (position.X > predator.position.X)
                {
                    velocity.X += 1200f * (float)g.ElapsedGameTime.TotalSeconds;
                    tooClose = true;
                }
                if (position.Y < predator.position.Y)
                {
                    velocity.Y -= 1200f * (float)g.ElapsedGameTime.TotalSeconds;
                    tooClose = true;
                }
                if (position.Y > predator.position.Y)
                {
                    velocity.Y += 1200f * (float)g.ElapsedGameTime.TotalSeconds;
                    tooClose = true;
                }
                if (tooClose)
                {
                    velocity *= 0.99f;
                }
            }
        }

    }
}
