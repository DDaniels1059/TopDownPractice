using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RPGPractice
{
    class Controller
    {
        public static double timer = 0.00001f;
        public static double maxTime = 0.5D;
        static Random rand = new Random();

        public Controller() { }


        public static void Update(GameTime gameTime, Texture2D spriteSheet)
        {
            timer -= gameTime.ElapsedGameTime.TotalSeconds;

            if (timer <= 0)
            {
                int side = rand.Next(2);

                switch (side)
                {
                    //case 0:
                    //    Enemy.enemies.Add(new Enemy(new Vector2(1190, 2444), spriteSheet));
                    //    break;
                    //case 1:
                    //    Enemy.enemies.Add(new Enemy(new Vector2(2509, 875), spriteSheet));
                    //    break;
                    //case 2:
                    //    Enemy.enemies.Add(new Enemy(new Vector2(rand.Next(4300, 7000), 1700), spriteSheet));
                    //    break;
                    //case 3:
                    //    Enemy.enemies.Add(new Enemy(new Vector2(rand.Next(4300, 7000), 6500), spriteSheet));
                    //    break;



                }
                timer = maxTime;

            }
        }
    }
}
