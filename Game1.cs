using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Comora;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace RPGPractice
{

    enum Dir { Down, Up, Left, Right }

    public static class Sounds
    {
        public static Song Main;
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private KeyboardState kStateOld = Keyboard.GetState();
        MouseState mStateOld = Mouse.GetState();
        private bool showDebug = false;
        private bool showDebugCollider = false;
        private List<Rectangle> collisionObjects;

        //Texture2D playerSprite;

        Texture2D walkDown;
        Texture2D walkUp;
        Texture2D walkRight;
        Texture2D walkLeft;

        Texture2D ball;
        Texture2D skull;
        Texture2D _texture;
        SpriteFont timerFont;
        Texture2D pixel;

        Texture2D healthTexture;
        Rectangle healthRectangle;

        TileMapManagerSharp mapManager = new TileMapManagerSharp();
        Player player = new Player();
        Camera camera;
        FpsMonitor FPSM = new FpsMonitor();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            //_graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //_graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            Window.AllowUserResizing = false;
            Window.IsBorderless = false;
            Window.Title = "HellRise";
            this.camera = new Camera(_graphics.GraphicsDevice);
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Sounds.Main = Content.Load<Song>("Sounds/main");
            //MediaPlayer.Play(Sounds.Main);
            //MediaPlayer.IsRepeating = true;

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            
            pixel = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });



            walkDown = Content.Load<Texture2D>("NewPlayer/walkDown");
            walkUp = Content.Load<Texture2D>("NewPlayer/walkUp");
            walkRight = Content.Load<Texture2D>("NewPlayer/walkRight");
            walkLeft = Content.Load<Texture2D>("NewPlayer/walkLeft");
            collisionObjects = new List<Rectangle>();

            ball = Content.Load<Texture2D>("Misc/ball");
            skull = Content.Load<Texture2D>("Misc/skull");

            _texture = new Texture2D(GraphicsDevice, 1, 1);
            Color color = Color.White;
            _texture.SetData(new Color[] { color });

            player.animations[0] = new SpriteAnimation(walkDown, 4, 8);
            player.animations[1] = new SpriteAnimation(walkUp, 4, 8);
            player.animations[2] = new SpriteAnimation(walkLeft, 4, 8);
            player.animations[3] = new SpriteAnimation(walkRight, 4, 8);

            player.anim = player.animations[0];
            timerFont = Content.Load<SpriteFont>("Misc/timerFont");
            healthTexture = Content.Load<Texture2D>("Health");

            player.Position = player.playerSpawn();


            //Enemy.enemies.Add(new Enemy(new Vector2(500, 950), skull));

            mapManager.LoadContent(Content, collisionObjects);


            _spriteBatch = new SpriteBatch(GraphicsDevice); 
        }

        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                FPSM.Update();
                Vector2 enemyinitpos = Vector2.Zero;
                foreach (Enemy enemy in Enemy.enemies)
                {
                    enemyinitpos = enemy.Position;
                }
                var initpos = player.Position;
                KeyboardState kState = Keyboard.GetState();
                MouseState mState = Mouse.GetState();

                if (kState.IsKeyDown(Keys.Escape) && kStateOld.IsKeyUp(Keys.Escape))
                {
                    if (_graphics.PreferredBackBufferWidth == GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                    {
                        _graphics.PreferredBackBufferWidth = 1280;
                        _graphics.PreferredBackBufferHeight = 720;
                        Window.IsBorderless = false;
                    }
                    else
                    {
                        _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                        _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                       Window.IsBorderless = true;
                        //camera.Height = 1;
                    }
                    _graphics.ApplyChanges();
                }

                if (kState.IsKeyDown(Keys.D1) && kStateOld.IsKeyUp(Keys.D1))
                {
                    if (showDebug)
                    {
                        showDebug = false;
                    }
                    else if (!showDebug)
                    {
                        showDebug = true;
                    }
                }

                if (kState.IsKeyDown(Keys.D2) && kStateOld.IsKeyUp(Keys.D2))
                {
                    if (showDebugCollider)
                    {
                        showDebugCollider = false;
                    }
                    else if (!showDebugCollider)
                    {
                        showDebugCollider = true;
                    }
                }
             
                if (!player.dead && mState.LeftButton == ButtonState.Pressed && mStateOld.LeftButton == ButtonState.Released)
                {
                    Projectile.projectiles.Add(new Projectile(GetMousePos(mState), this.camera.Position, 400f, 20));
                    //Projectile.projectiles.Add(new Projectile(GetMousePos(mState), new Vector2(this.camera.Position.X, this.camera.Position.Y - 65), 400f, 20));
                }

                player.Update(gameTime);
                var playerPos = player.Position;


                if (mState.RightButton == ButtonState.Pressed && mStateOld.RightButton == ButtonState.Released)
                {
                    Enemy.enemies.Add(new Enemy(GetMousePos(mState), skull));              
                }
            

                if (!player.dead)
                {
                    Controller.Update(gameTime, skull);
                    healthRectangle = new Rectangle(25, 25, player.Health * 2, 30);
                }
                else if (player.dead)
                {
                    //MediaPlayer.Pause();
                    if (kState.IsKeyDown(Keys.Space) && kStateOld.IsKeyUp(Keys.Space))
                    {
                        player.playerRestart();
                        Enemy.enemies.RemoveAll(Enemy.enemies.Remove);
                        //MediaPlayer.Resume();
                    }
                }

                this.camera.Position = player.Position;
                this.camera.Update(gameTime);

                foreach (Projectile proj in Projectile.projectiles)
                {
                    proj.Update(gameTime);
                }

                //TileSheet Collision Detection
                foreach (var rect in collisionObjects)
                {
                    if (rect.Intersects(player.PlayerCollisionBox))
                    {
                        player.Position = initpos;
                        player.anim.setFrame(1);
                    }

                    foreach (Enemy enemy in Enemy.enemies)
                    {
                        if (rect.Intersects(enemy.HitBox))
                        {
                            enemy.Position = enemyinitpos;
                        }
                    }

                    foreach (Projectile proj in Projectile.projectiles)
                    {
                        if (rect.Intersects(proj.HitBox))
                        {
                            proj.Collided = true;
                        }
                    }
                }

                foreach (Enemy enemy in Enemy.enemies)
                {
                    if (!enemy.Dead && enemy.HitBox.Intersects(player.PlayerHitBox))
                    {

                        //enemy.Dead = true;
                        //player.Health -= 1;
                        //if(player.Health <= 0)
                        //{
                        //    player.dead = true;
                        //    healthRectangle.Width = 0;
                        //}
                    }

                    //foreach (Projectile proj in Projectile.projectiles)
                    //{
                    //    if (proj.HitBox.Intersects(enemy.HitBox) && !enemy.Dead)
                    //    {
                    //        proj.Collided = true;
                    //        enemy.Hit(proj.Damage); // call Hit function to store last hit time and set IsHit to true
                    //        if (enemy.health <= 0)
                    //        {
                    //            enemy.Dead = true;
                    //            enemy.deathTime = gameTime.TotalGameTime.TotalSeconds;
                    //        }
                    //    }
                    //}                   

                    enemy.Update(gameTime, player.Position, player.dead);
                }


                Projectile.projectiles.RemoveAll(p => p.Collided);
                Enemy.enemies.RemoveAll(e => e.Dead && gameTime.TotalGameTime.TotalSeconds - e.deathTime >= 1);
                kStateOld = kState;
                mStateOld = mState;
                base.Update(gameTime);
            }
        }


        public Vector2 GetMousePos(MouseState mstate)
        {
            mstate = Mouse.GetState();
            Vector2 mpos = new Vector2(mstate.X, mstate.Y);
            return mpos = Vector2.Transform(mpos, camera.ViewportOffset.Absolute);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //For Debug Purposes Only
            float playerDepth = 0f;

            _spriteBatch.Begin(this.camera, SpriteSortMode.FrontToBack, BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);


            mapManager.Draw(_spriteBatch, _graphics, showDebugCollider, Content);
            //mapManager.Draw(gameTime, _spriteBatch, _graphics, timerFont);

            foreach (Enemy enemy in Enemy.enemies)
            {
                
                Vector2 origin = new Vector2(enemy.Position.X - 48, enemy.Position.Y - 7);
                float depth = origin.Y / _graphics.PreferredBackBufferHeight;
                depth = depth * 0.01f; // multiply the depth by a small value
                //Vector2 enemyTilePos = new Vector2((enemy.Position.X - 48) / 96, (enemy.Position.Y + 8) / 96);

                //Vector2 enemypos = new Vector2(enemy.Position.X / 96, enemy.Position.Y / 96);
                if (showDebugCollider)
                {
                    _spriteBatch.Draw(ball, origin, null, Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 1f);

                }
                //_spriteBatch.Draw(ball, origin, null, Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 0.0005f);


                if (showDebugCollider)
                    _spriteBatch.Draw(_texture, enemy.HitBox, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0004f);

                enemy.Draw(_spriteBatch, depth, enemy.pathList, pixel);
            }           

            foreach (Projectile proj in Projectile.projectiles)
            {
                _spriteBatch.Draw(ball, new Vector2(proj.Position.X - 48, proj.Position.Y - 48), null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0001f);
            }

            if (!player.dead)
            {
                Vector2 origin = new Vector2(player.Position.X - 48, player.Position.Y - 7);
                float depth = origin.Y / _graphics.PreferredBackBufferHeight;
                playerDepth = depth * 0.01f;

                if (showDebugCollider)
                    _spriteBatch.Draw(_texture, player.PlayerCollisionBox, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.004f);

                if (showDebugCollider)
                    _spriteBatch.Draw(ball, origin, null, Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 0.005f);

                player.anim.Draw(_spriteBatch, playerDepth);
            }



            _spriteBatch.End();




            _spriteBatch.Begin();

            _spriteBatch.Draw(healthTexture, healthRectangle, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1f);

            if (player.dead)
            {
                Vector2 textSize;
                Vector2 textPosition;
                string text = "Press SPACE to Restart";

                textSize = timerFont.MeasureString(text);

                // Calculate the position at which to draw the text
                textPosition = new Vector2(
                    (GraphicsDevice.Viewport.Width - textSize.X) / 2,
                    (GraphicsDevice.Viewport.Height - textSize.Y) / 2);

                _spriteBatch.DrawString(timerFont, text, textPosition, Color.White);
            }

            if (showDebug)
            {
                //All For Debug Purposes Only
                var mstate = Mouse.GetState();
                Vector2 mpos = new Vector2(mstate.X, mstate.Y);
                mpos = Vector2.Transform(mpos, camera.ViewportOffset.Absolute);

                FPSM.Draw(_spriteBatch, timerFont, new Vector2(25, 90), Color.White);
                _spriteBatch.DrawString(timerFont, "Player Depth: " + playerDepth.ToString(), new Vector2(25, 0), Color.White);
                _spriteBatch.DrawString(timerFont, "Player Pos: " + Math.Round(player.Position.X).ToString() + " " + Math.Round(player.Position.Y).ToString(), new Vector2(25, 30), Color.White);
                _spriteBatch.DrawString(timerFont, "Mouse Pos: " + Math.Round(mpos.X).ToString() + " " + Math.Round(mpos.Y).ToString(), new Vector2(25, 60), Color.White); 
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    public class FpsMonitor
    {
        public float Value { get; private set; }
        public TimeSpan Sample { get; set; }
        private Stopwatch sw;
        private int Frames;
        public FpsMonitor()
        {
            this.Sample = TimeSpan.FromSeconds(1);
            this.Value = 0;
            this.Frames = 0;
            this.sw = Stopwatch.StartNew();
        }
        public void Update()
        {
            if (sw.Elapsed > Sample)
            {
                this.Value = (float)(Frames / sw.Elapsed.TotalSeconds);
                this.sw.Reset();
                this.sw.Start();
                this.Frames = 0;
            }
        }
        public void Draw(SpriteBatch SpriteBatch, SpriteFont Font, Vector2 Location, Color Color)
        {
            this.Frames++;
            SpriteBatch.DrawString(Font, "FPS: " + this.Value.ToString(), Location, Color);
        }
    }
}