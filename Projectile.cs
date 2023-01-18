using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Transform;

namespace RPGPractice
{
    internal class Projectile
    {
        public static List<Projectile> projectiles = new List<Projectile>();

        private Vector2 position;
        private int speed = 800;
        private Rectangle hitBox = new Rectangle();
        private bool collided = false;
        private Vector2 direction;
        private float projectileSpeed;
        private int damage = 50;


        //public int radius = 18;
        //private Dir direction;
        //private Vector2 target;
        //private Vector2 velocity;
        //private Vector2 projectilePosition;

        //public Projectile(Vector2 newPosition, Dir newDir)
        //{
        //    position = newPosition;
        //    //direction = newDir;
        //}

        public Projectile(Vector2 mousePosition, Vector2 projectilePosition, float projectileSpeed, int damage)
        {
            this.position = projectilePosition;
            this.projectileSpeed = projectileSpeed;

            // Calculate the direction of the mouse from the projectile's starting position
            direction = Vector2.Normalize(mousePosition - projectilePosition);
        }

        public Vector2 Position { get { return position; } }

        public bool Collided { get { return collided; } set { collided = value; } }

        public int Damage { get { return damage; } set { damage = value; } }

        public Rectangle HitBox { get { return hitBox; } }

        public void Update(GameTime gameTime)
        {
            position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            hitBox = new Rectangle((int)position.X, (int)position.Y, 32, 32);

            //float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //position += velocity * dt;

            //switch (direction)
            //{
            //    case Dir.Right:
            //        position.X += speed * dt; break;
            //    case Dir.Left:
            //        position.X -= speed * dt; break;
            //    case Dir.Up:
            //        position.Y -= speed * dt; break;
            //    case Dir.Down:
            //        position.Y += speed * dt; break;

            //}
        }
    }
}
