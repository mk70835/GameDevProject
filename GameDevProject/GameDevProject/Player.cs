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
        private  const int frameh = 29;
        private  const int framehcount = 7;
        private  const int framevcount = 4;

        public Texture2D PlayerTexture;                                 // tekstura gracza
        public Rectangle DrawLegs;                                      // kwadrat do wyswietlenia z dolna tekstura (poryszanie WSAD)
        public Rectangle DrawTorso;                                     // kwadrat do wyswietlania z gorna tekstura (poruszanie myszka)
        private Point currLegFrame;                                     // coord aktualnej dolnej klatki
        private Point currTorsoFrame;                                   // coord aktualnej gornej klatki
        private Point frameSize;                                        // rozmiar klatki (aktualnie tyle samo dla gornej i dolnej)
        private Point sheetSize;                                        // ilosc klatek (aktualnie tyle samo dla gornej i dolnej)
        private int sinceLastFrame;                                     // ile czasu od ostatniej zmiany klatki (dla dolnej)
        private const int msPerFrame = 100;                             // ile czasu ma trwac jedna klatka (dla dolnej)
        
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
            currLegFrame = new Point(0, 1);
            currTorsoFrame = new Point(0, 0);
            sinceLastFrame = 0;

            DrawLegs = new Rectangle(currLegFrame.X * frameSize.X, currLegFrame.Y * frameSize.Y, frameSize.X, frameSize.Y);
            DrawTorso = new Rectangle(currTorsoFrame.X * frameSize.X, currTorsoFrame.Y * frameSize.Y, frameSize.X, frameSize.Y);
            CollisionRect = new Rectangle((int)position.X, (int)position.Y, frameSize.X, frameSize.Y);
            Visible = true;
            
        }

        //public void Initialize()
        //{
        //    
        //}

        public override void Update(GameTime gameTime)
        {

            // dobra klatka dolna

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
                currLegFrame.Y = 1;
            if (PlayerVelocity.X > 0)
                currLegFrame.Y = 3;

            sinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (keyState.IsKeyUp(Keys.D) && keyState.IsKeyUp(Keys.A))
            {
                currLegFrame.X = 0;
                if (msPerFrame < sinceLastFrame)
                {
                    sinceLastFrame -= msPerFrame;
                }
            }
            else if (msPerFrame < sinceLastFrame)
            {
                sinceLastFrame -= msPerFrame;
                ++currLegFrame.X;
                if (currLegFrame.X >= sheetSize.X)
                {
                    currLegFrame.X = 1;
                }
            }

            DrawLegs = new Rectangle(currLegFrame.X * frameSize.X, currLegFrame.Y * frameSize.Y, frameSize.X, frameSize.Y);

            // dobra klatka gorna

            MouseState mouseState = Mouse.GetState();

            Point origin = new Point((int)(PlayerPosition.X + 0.5f * frameSize.X), (int)(PlayerPosition.Y + 0.5f * frameSize.Y));
            Point mouse = new Point(mouseState.X, mouseState.Y);

            float delta = mouse.X - origin.X ;
            if (delta == 0)
                delta = float.Epsilon;

            float tga = (mouse.Y - origin.Y) / delta;

            if (tga > 1.903)                                // tan(90 - (180/7))
            {
                currTorsoFrame.X = 6;
            }
            else if (tga > 0.797)
            {
                currTorsoFrame.X = 5;
            }
            else if (tga > 0.228)
            {
                currTorsoFrame.X = 4;
            }
            else if (tga > -0.228)
            {
                currTorsoFrame.X = 3;
            }
            else if (tga > -0.797)
            {
                currTorsoFrame.X = 2;
            }
            else if (tga > -1.903)
            {
                currTorsoFrame.X = 1;
            }
            else
                currTorsoFrame.X = 0;

            if (mouse.X < origin.X)
                currTorsoFrame.Y = 0;
            else
                currTorsoFrame.Y = 2;

            DrawTorso = new Rectangle(currTorsoFrame.X * frameSize.X, currTorsoFrame.Y * frameSize.Y, frameSize.X, frameSize.Y);


            //
            PlayerPosition += PlayerVelocity;

            CollisionRect = new Rectangle((int)PlayerPosition.X, (int)PlayerPosition.Y, frameSize.X * 2, frameSize.Y * 2);

            PlayerVelocity.X *= 0.35f;

        }

        public override void Draw( GameTime gameTime )
        {
            SpriteBatch sb = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            sb.Draw(PlayerTexture, PlayerPosition, DrawTorso, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            sb.Draw(PlayerTexture, new Vector2(PlayerPosition.X, PlayerPosition.Y + frameSize.Y), DrawLegs, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

    }
}
