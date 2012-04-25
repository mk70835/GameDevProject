using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// TODO: kolizje

namespace GameDevProject
{
    public class Player : Microsoft.Xna.Framework.DrawableGameComponent
    {

        public Texture2D PlayerTexture;                                 // tekstura gracza
        public Rectangle DrawRectangle;                                 // kwadrat tekstury gracza do wyswietlenia //TODO: update DrawRectangle
        
        public Vector2 PlayerPosition;                                  // pozycja gracza (lewy gorny rog)
        
        public int Width                                                // szerokosc gracza
        {
            get { return PlayerTexture.Width; }
        }
        
        public int Height                                               // wysokosc gracza
        {
            get { return PlayerTexture.Height; }
        }

        public Player(Game game)
            :base (game)
        {

        }

        public Player(Game game, Texture2D texture, Vector2 position)
            : base(game)
        {
            PlayerTexture = texture;
            PlayerPosition = position;
            DrawRectangle = new Rectangle(0, 0, PlayerTexture.Width, PlayerTexture.Height);
            Visible = true;
        }

        //public void Initialize()
        //{
        //    
        //}

        public void Update()
        {

        }

        public void Draw( SpriteBatch sb, GameTime gameTime )
        {
            sb.Draw(PlayerTexture, PlayerPosition, DrawRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

    }
}
