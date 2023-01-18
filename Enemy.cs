using System.Collections.Generic;
using AStar.Options;
using AStar;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.IO;
using Comora;

namespace RPGPractice
{
    internal class Enemy
    {
        public static List<Enemy> enemies = new List<Enemy>();

        //Press Right Mouse Button to spawn enemy at Mouse Position

        private Vector2 position = new Vector2(0, 0);
        private int speed = 25;
        public SpriteAnimation anim;
        private bool dead = false;
        private Rectangle hitBox = new Rectangle();
        public int health = 150;
        private bool isHit = false;
        private float elapsedTime;
        public readonly float hitDuration = 0.09f; // duration to show enemy in red (in seconds)
        public float lastHitTime; // time at which enemy was last hit
        public double deathTime;
        public List<Vector2> pathList;
        public void Hit(int damage)
        {
            health -= damage;
            lastHitTime = elapsedTime;
            isHit = true;
        }

        public Enemy(Vector2 newPos, Texture2D spriteSheet)
        {
            isHit = false;
            lastHitTime = 0;
            position = newPos;
            anim = new SpriteAnimation(spriteSheet, 10, 6);
        }

        public Rectangle HitBox { get { return hitBox; } }
        public Vector2 Position { get { return position; } set { position = value; } }
        public bool Dead { get { return dead; } set { dead = value; } }





        public void Update(GameTime gameTime, Vector2 playerPos, bool isPlayerDead)
        {
            anim.Position = new Vector2(position.X - 48, position.Y - 48);
            anim.Update(gameTime);
            hitBox = new Rectangle((int)position.X - 35, (int)position.Y - 30, 70, 85);
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!isPlayerDead)
            {
                //Tiles Are 96x96

                var pathfinder = new PathFinder(TileMapManagerSharp.worldGrid, TileMapManagerSharp.pathfinderOptions);

                Position[] path = pathfinder.FindPath(new Position((int)position.X / 96, (int)position.Y / 96), new Position((int)playerPos.X / 96, (int)playerPos.Y / 96));
                pathList = path.Select(p => new Vector2(p.Row * 96 + 48, p.Column * 96 + 48)).ToList();
                if (pathList.Count > 0)
                {

                    MoveEnemy(pathList, gameTime);

                }
            }

            if (dead)
            {
                speed = 0;
            }

        }

        private void MoveEnemy(List<Vector2> pathList, GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;


            Vector2 moveDir;
            for (int i = 0; i < pathList.Count - 1; i++)
            {
                Vector2 centerOfTile = new Vector2((int)pathList[i].X / 96 * 96 + 48, (int)pathList[i].Y / 96 * 96 + 48);
                moveDir = new Vector2(centerOfTile.X - position.X, centerOfTile.Y - position.Y);
                moveDir.Normalize();
                position += moveDir * speed * dt;
            }
        }

        public void Draw(SpriteBatch _spriteBatch, float depth, List<Vector2> pathList, Texture2D pixel)
        {

            //Drawing the lines to show the path, atm I know the path is on the edges so I accounted for that
            for (int i = 0; i < pathList.Count - 1; i++)
            {
                float distance = Vector2.Distance(pathList[i], pathList[i + 1]);
                float angle = (float)Math.Atan2(pathList[i + 1].Y - pathList[i].Y, pathList[i + 1].X - pathList[i].X);
                _spriteBatch.Draw(pixel, pathList[i] - new Vector2(48, 48), null, Color.Red, angle, Vector2.Zero, new Vector2(distance, 3), SpriteEffects.None, 1f);
            }

            if (!dead)
            {
                if (isHit && elapsedTime - lastHitTime < hitDuration)
                {
                    anim.Draw(_spriteBatch, depth, Color.Red);
                }
                else
                {
                    anim.Draw(_spriteBatch, depth);
                    isHit = false;
                }
            }
            else
            {
                anim.Draw(_spriteBatch, depth, Color.Black);
            }
        }
    }
}
