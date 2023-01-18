using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TiledSharp;
using Microsoft.Xna.Framework;
using AStar.Collections;    
using AStar.Options;
using AStar;
using AStar.Heuristics;

namespace RPGPractice
{
    class TileMapManagerSharp
    {
        private TmxMap map;
        private Texture2D tileset;
        private Texture2D _debugMarkerTexture;
        public static WorldGrid worldGrid;
        public static PathFinderOptions pathfinderOptions;

        public static int tileWidth;
        public static int tileHeight;
        private int tilesetTilesWide;
        private int tilesetTilesHigh;
        public static int mapWidth;
        public static int mapHeight;
        public static short[,] tilesmap;

        public void LoadContent(ContentManager Content, List<Rectangle> collisionObjects)
        {
            map = new TmxMap(Content.RootDirectory + "\\hellsurfaceMap.tmx");
            tileset = Content.Load<Texture2D>(map.Tilesets[0].Name.ToString());
            _debugMarkerTexture = Content.Load<Texture2D>("Misc/debugMarker");



            pathfinderOptions = new PathFinderOptions
            {
                HeuristicFormula = HeuristicFormula.Manhattan,
                PunishChangeDirection = true,
                UseDiagonals = false,
            };

            mapWidth = map.Width;
            mapHeight = map.Height;

            tileWidth = map.Tilesets[0].TileWidth;
            tileHeight = map.Tilesets[0].TileHeight;

            tilesetTilesWide = tileset.Width / tileWidth;
            tilesetTilesHigh = tileset.Height / tileHeight;


            tilesmap = new short[mapWidth, mapHeight];


            //Getting Collision Layer and Adding... Collisions
            TmxLayer layer = map.Layers[2];
            foreach (TmxLayerTile tile in layer.Tiles)
            {
                // Empty tile, do nothing
                if (tile.Gid == 0)
                {
                    //Moveable
                    int x = (int)tile.X;
                    int y = (int)tile.Y;
                    tilesmap[x, y] = 1;
                }
                else
                {
                    //Collider/Wall
                    Rectangle rect = new Rectangle((int)tile.X * tileWidth, (int)tile.Y * tileHeight, tileWidth, tileHeight);
                    collisionObjects.Add(rect);
                    int x = (int)tile.X;
                    int y = (int)tile.Y;
                    tilesmap[x, y] = 0;
                }
            }
            worldGrid = new WorldGrid(tilesmap);

        }

        public void Draw(SpriteBatch _spriteBatch, GraphicsDeviceManager _graphics, Boolean showDebugCollider, ContentManager Content)
        {

            Texture2D nodeTexture = Content.Load<Texture2D>("node");

            for (int x = 0; x < worldGrid.Width; x++)
            {
                for (int y = 0; y < worldGrid.Height; y++)
                {
                    Vector2 nodePos = new Vector2(x * tileWidth, (y * tileHeight));
                    _spriteBatch.Draw(nodeTexture, new Rectangle((int)nodePos.X, (int)nodePos.Y, tileWidth, tileHeight), null, Color.Black, 0.0f, Vector2.Zero, SpriteEffects.None, 0.9f);
                }
            }

            for (int j = 0; j < map.Layers.Count; j++)
            {
                //Dynamic Sprites
                if (j == 1)
                {
                    for (var i = 0; i < map.Layers[1].Tiles.Count; i++)
                    {
                        int gid = map.Layers[1].Tiles[i].Gid;

                        // Empty tile, do nothing
                        if (gid == 0)
                        {

                        }
                        else
                        {
                            int tileFrame = gid - 1;
                            int column = tileFrame % tilesetTilesWide;
                            int row = (int)Math.Floor((double)tileFrame / (double)tilesetTilesWide);

                            float x = (i % map.Width) * map.TileWidth;
                            float y = (float)Math.Floor(i / (double)map.Width) * map.TileHeight;

                            Rectangle tilesetRec = new Rectangle(tileWidth * column, tileHeight * row, tileWidth, tileHeight);

                            Vector2 origin = new Vector2(((int)x + tileWidth) - 96, ((int)y + tileHeight) - 48);
                            float depth = origin.Y / _graphics.PreferredBackBufferHeight;
                            depth = depth * 0.01f;


                            _spriteBatch.Draw(tileset, new Rectangle((int)x, (int)y, tileWidth, tileHeight), tilesetRec, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
                            _spriteBatch.Draw(_debugMarkerTexture, origin, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.00003f);
                        }
                    }
                }

                //Static Sprites Like Ground Tile Etc..
                if (j == 0)
                {
                    for (var i = 0; i < map.Layers[0].Tiles.Count; i++)
                    {
                        int gid = map.Layers[0].Tiles[i].Gid;

                        // Empty tile, do nothing
                        if (gid == 0)
                        {

                        }
                        else
                        {
                            int tileFrame = gid - 1;
                            int column = tileFrame % tilesetTilesWide;
                            int row = (int)Math.Floor((double)tileFrame / (double)tilesetTilesWide);

                            float x = (i % map.Width) * map.TileWidth;
                            float y = (float)Math.Floor(i / (double)map.Width) * map.TileHeight;

                            Rectangle tilesetRec = new Rectangle(tileWidth * column, tileHeight * row, tileWidth, tileHeight);
                            Vector2 origin = new Vector2(((int)x + tileWidth) - 96, ((int)y + tileHeight) - 96);

                            _spriteBatch.Draw(tileset, new Rectangle((int)x, (int)y, tileWidth, tileHeight), tilesetRec, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0f);

                            if(showDebugCollider)
                                _spriteBatch.Draw(_debugMarkerTexture, origin, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.00004f);
                        }
                    }
                }

                //if (j == 3)
                //{
                //    for (var i = 0; i < map.Layers[3].Tiles.Count; i++)
                //    {
                //        int gid = map.Layers[3].Tiles[i].Gid;

                //        // Empty tile, do nothing
                //        if (gid == 0)
                //        {

                //        }
                //        else
                //        {
                //            int tileFrame = gid - 1;
                //            int column = tileFrame % tilesetTilesWide;
                //            int row = (int)Math.Floor((double)tileFrame / (double)tilesetTilesWide);

                //            float x = (i % map.Width) * map.TileWidth;
                //            float y = (float)Math.Floor(i / (double)map.Width) * map.TileHeight;

                //            Rectangle tilesetRec = new Rectangle(tileWidth * column, tileHeight * row, tileWidth, tileHeight);

                //            Vector2 origin = new Vector2(((int)x + tileWidth) - 96, ((int)y + tileHeight) - 96);
                //            float depth = origin.Y / _graphics.PreferredBackBufferHeight;
                //            depth = depth * 0.01f;


                //            _spriteBatch.Draw(tileset, new Rectangle((int)x, (int)y, tileWidth, tileHeight), tilesetRec, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, depth);
                //            //_spriteBatch.Draw(_debugMarkerTexture, origin, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.00003f);
                //        }
                //    }
                //}
            }
        }
    }
}
