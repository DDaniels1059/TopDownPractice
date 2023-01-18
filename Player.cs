using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Comora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RPGPractice
{
    class Player
    {
        //Texture2D playerSprite = new <Texture2D>("Player/player");
        private Vector2 position;
        private Dir direction = Dir.Down;
        private int speed = 500;
        private bool isMoving = false;
        public bool isColliding = false;
        public bool dead = false;
        private KeyboardState kStateOld = Keyboard.GetState();
        private MouseState mStateOld = Mouse.GetState();
        private Rectangle playerCollisionBox = new Rectangle();
        private Rectangle playerHitBox = new Rectangle();
        private int health = 200;
        public SpriteAnimation anim;       



        public SpriteAnimation[] animations = new SpriteAnimation[4];
        public Vector2 Position { get { return position; } set { position = value;} }
        public Rectangle PlayerCollisionBox { get { return playerCollisionBox; } }
        public Rectangle PlayerHitBox { get { return playerHitBox; } }
        public int Health 
        { 
            get { return health; } 
            set { health = value; } 
        }


        public Vector2 playerSpawn()
        {
            Vector2[] spawns = {
                new Vector2(576, 480),
                new Vector2(826, 2213),
                new Vector2(1193, 2522),
                new Vector2(634, 2622)
            };

            Random rng = new Random();
            int spawnindex = rng.Next(0, 4);

            return spawns[spawnindex];
        }

        public void playerRestart()
        {
            health = 200;
            dead = false;
            position = playerSpawn();
        }

        public void Update(GameTime gameTime) 
        {
            KeyboardState kState = Keyboard.GetState();
            MouseState mState= Mouse.GetState();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            isMoving = false;

            if (kState.IsKeyDown(Keys.D))
            {
                direction = Dir.Right;
                isMoving = true;
            }
            if (kState.IsKeyDown(Keys.A))
            {
                direction = Dir.Left;
                isMoving = true;
            }
            if (kState.IsKeyDown(Keys.W))
            {
                direction = Dir.Up;
                isMoving = true;
            }
            if (kState.IsKeyDown(Keys.S))
            {
                direction = Dir.Down;
                isMoving = true;
            }

            //if(kState.IsKeyDown(Keys.Space))
            //    isMoving = false;

            if (isMoving && !dead)
            {
                switch (direction)
                {
                    case Dir.Right:
                        //if(position.X < 1275)
                            position.X += speed * dt; 
                        break;
                    case Dir.Left:
                        //if (position.X > 225)
                            position.X -= speed * dt; 
                        break;
                    case Dir.Up:
                        if (position.Y > 50)
                            position.Y -= speed * dt; 
                        break;
                    case Dir.Down:
                        //if (position.Y < 1250)
                            position.Y += speed * dt; 
                        break;
                }
            }

            anim = animations[(int)direction];

            anim.Position = new Vector2(position.X - 48, position.Y - 48);
            if (kState.IsKeyDown(Keys.Space))
                anim.setFrame(0);
            else if (isMoving)
                anim.Update(gameTime);
            else
                anim.setFrame(1);



            //var mstate = Mouse.GetState();
            //Vector2 mpos = new Vector2(mstate.X, mstate.Y);
            //mpos = Vector2.Transform(mpos, camera.ViewportOffset.Absolute);

            //// Update the mouse position
            //mousePosition = new Vector2(mouseState.X, mouseState.Y);

            //if (kState.IsKeyDown(Keys.Space) && kStateOld.IsKeyUp(Keys.Space))
            //    Projectile.projectiles.Add(new Projectile(mousePosition, new Vector2(position.X, position.Y), projectileSpeed));

            playerCollisionBox = new Rectangle((int)position.X - 35, (int)position.Y + 30, 70, 25);
            playerHitBox = new Rectangle((int)position.X - 35, (int)position.Y - 30, 70, 85);

            mStateOld = mState;
            kStateOld = kState;
        }
    }
}
