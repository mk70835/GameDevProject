using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// TODO: kolizje
// jak zrobic texture z bronia? gracz to dwie tekstury nalozone na siebie - nogi i korpus sterowane klawiatura rece glowa i bron sterowane myszka


namespace GameDevProject
{
    public class Player : Microsoft.Xna.Framework.DrawableGameComponent
    {

        private  const int framew = 58;
        private  const int frameh = 58;
        private  const int framehcount = 7;
        private  const int framevcount = 2;

        public Texture2D PlayerTexture;                                 // tekstura gracza
        public Rectangle DrawRectangle;                                 // kwadrat tekstury gracza do wyswietlenia //TODO: update DrawRectangle
        private Point currFrame;                                        // coord aktualnej klatki
        private Point frameSize;                                        // rozmiar klatki
        private Point sheetSize;                                        // ilosc klatek
        private int sinceLastFrame;                                     // ile czasu od ostatniej zmiany klatki
        private const int msPerFrame = 100;                             // ile czasu ma trwac jedna klatka
        
        public Rectangle CollisionRect;                                 // kwadrat kolizji

        public Vector2 PlayerPosition;                                  // pozycja gracza

        private Vector2 PlayerVelocity;                                 // ruch
        private int playerSpeed;                                        // predkosc dodawana do vektora ruchu

        public Player(Game game)
            :base (game)
        {

        }

        public Player(Game game, Texture2D texture, Vector2 position)
            : base(game)
        {
            PlayerTexture = texture;
            PlayerPosition = position;
            PlayerVelocity = Vector2.Zero;
            playerSpeed = 5;
            
            frameSize = new Point(framew, frameh);
            sheetSize = new Point(framehcount, framevcount);
            currFrame = new Point(0, 0);
            sinceLastFrame = 0;

            DrawRectangle = new Rectangle(currFrame.X * frameSize.X, currFrame.Y * frameSize.Y, frameSize.X, frameSize.Y);
            CollisionRect = new Rectangle((int)position.X, (int)position.Y, frameSize.X, frameSize.Y);
            Visible = true;
            
        }

        //public void Initialize()
        //{
        //    
        //}

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();

            if(keyState.IsKeyDown(Keys.A))
            {
                PlayerVelocity.X -= playerSpeed;
            }
            else if (keyState.IsKeyDown(Keys.D))
            {
                PlayerVelocity.X += playerSpeed;
            }

            
            if (PlayerVelocity.X < 0)
                currFrame.Y = 0;
            if (PlayerVelocity.X > 0)
                currFrame.Y = 1;

            sinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (keyState.IsKeyUp(Keys.D) && keyState.IsKeyUp(Keys.A))
            {
                currFrame.X = 0;
                if (msPerFrame < sinceLastFrame)
                {
                    sinceLastFrame -= msPerFrame;
                }
            }
            else if (msPerFrame < sinceLastFrame)
            {
                sinceLastFrame -= msPerFrame;
                ++currFrame.X;
                if (currFrame.X >= sheetSize.X)
                {
                    currFrame.X = 1;
                }
            }

            PlayerPosition += PlayerVelocity;

            DrawRectangle = new Rectangle(currFrame.X * frameSize.X, currFrame.Y * frameSize.Y, frameSize.X, frameSize.Y);
            CollisionRect = new Rectangle((int)PlayerPosition.X, (int)PlayerPosition.Y, frameSize.X, frameSize.Y);

            PlayerVelocity.X *= 0.35f;

        }

        public override void Draw( GameTime gameTime )
        {
            SpriteBatch sb = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            sb.Draw(PlayerTexture, PlayerPosition, DrawRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

    }
}
