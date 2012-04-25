using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevProject
{
    class Player
    {

        public Texture2D PlayerTexture;
        public Rectangle DrawRectangle;
        
        public Vector2 PlayerPosition;
        
        public bool Active;
        
        public int Width
        {
            get { return PlayerTexture.Width; }
        }
        
        public int Height
        {
            get { return PlayerTexture.Height; }
        }

        public void Initialize(Texture2D texture, Vector2 position)
        {
            PlayerTexture = texture;
            PlayerPosition = position;
            DrawRectangle = new Rectangle(0, 0, PlayerTexture.Width, PlayerTexture.Height);
            Active = true;

        }

        public void Update()
        {

        }

        public void Draw( SpriteBatch sb, GameTime gameTime )
        {
            sb.Draw(PlayerTexture, PlayerPosition, DrawRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

    }
}
