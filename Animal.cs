using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlockingSimulation
{

    class Animal
    {

        protected int mass;
        public Vector2 position;
        protected Vector2 velocity;
        protected Vector2 acceleration;
        protected Texture2D texture;
        protected float topSpeed;// = 300f;
        protected float bottomSpeed;// = 150f;
        protected float angle = 0f;

        // Limits the speed of objects so that they don't exceed an acceptable velocity
        public void limitSpeed(GameTime gameTime)
        {

            // Reset the velocity to topSpeed if it goes over the topSpeed variable's value
            if (velocity.Length() > topSpeed)
            {
                velocity.Normalize();
                velocity *= topSpeed;
            }
        }

        // Boosts the speed of objects, so that nothing comes to a stop
        public void boostSpeed(GameTime gameTime)
        {

            // Reset the velocity to bottomSpeed if it goes under the bottomSpeed variable's value
            if (velocity.Length() < bottomSpeed)
            {
                velocity.Normalize();
                velocity *= bottomSpeed;
            }
        }

        // Used for objects not controlled by a user.
        public void accel(GameTime gameTime)
        {
            // Accelerate the Bird
            Vector2 accel = Vector2.Multiply(velocity, 20f);
            applyForce(accel, gameTime);
        }

        public void applyForce(Vector2 force, GameTime gameTime)
        {
            // Apply force to the Bird. Using mass but mass is currently set to 1, so it has no bearing on the result,
            // but could incorporate it for different types of users.
            Vector2 f = Vector2.Divide(force, mass);
            acceleration += f * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        // Update the angle to match the direction of movement.
        public void heading()
        {
            float heading = (float)Math.Atan2(velocity.Y, velocity.X);
            angle = heading;
        }
    }
}
