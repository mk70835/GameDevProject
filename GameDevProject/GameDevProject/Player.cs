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

        private SpriteBatch sb;
        //private GraphicsDevice gd;

        private Texture2D PlayerTexture;                                // tekstura spritow gracza
        private Color[] tex;                                            // tekstura spritow gracza
        public int drawTex;                                   // wskaznik na teksture do narysowania w danej klatce
        public Texture2D[] DrawTexBuf;                                 // tutaj rysowane sa tekstury
        private Color[] draw;                                           // tekstura do narysowania w danej klatce
        public Rectangle DrawLegs;                                      // kwadrat do wyswietlenia z dolna tekstura (poryszanie WSAD)
        private Color[] legs;
        public Rectangle DrawTorso;                                     // kwadrat do wyswietlania z gorna tekstura (poruszanie myszka)
        private Color[] torso;


        private Point currLegFrame;                                     // coord aktualnej dolnej klatki
        private Point prevLegFrame;
        private Point currTorsoFrame;                                   // coord aktualnej gornej klatki
        private Point prevTorsoFrame;
        private Point frameSize;                                        // rozmiar klatki (aktualnie tyle samo dla gornej i dolnej)
        private Point sheetSize;                                        // ilosc klatek (aktualnie tyle samo dla gornej i dolnej)
        private int sinceLastFrame;                                     // ile czasu od ostatniej zmiany klatki (dla dolnej)
        private const int msPerFrame = 100;                             // ile czasu ma trwac jedna klatka (dla dolnej)
        
        public Rectangle CollisionRect;                                 // kwadrat kolizji

        public Vector2 PlayerPosition;                                  // pozycja gracza
        public Vector2 PrevPlayerPosition;                              // poprzednia pozycja
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
            tex = new Color[PlayerTexture.Width * PlayerTexture.Height];
            PlayerTexture.GetData(tex);
            PlayerPosition = position;
            PrevPlayerPosition = PlayerPosition;
            PlayerVelocity = Vector2.Zero;
            playerSpeed = 5;

            sb = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            //gd = (GraphicsDevice)Game.Services.GetService(typeof(GraphicsDevice));
            
            frameSize = new Point(framew, frameh);
            sheetSize = new Point(framehcount, framevcount);
            currLegFrame = new Point(0, 1);
            prevLegFrame = new Point(-1, -1);
            currTorsoFrame = new Point(0, 0);
            prevTorsoFrame = new Point(-1, -1);
            legs = new Color[frameSize.X * frameSize.Y];
            for (int y = 0; y < frameSize.Y; ++y)
            {
                for (int x = 0; x < frameSize.X; ++x)
                {
                    legs[y * frameSize.X + x] = tex[x + (currLegFrame.X * frameSize.X) + (y + currLegFrame.Y * frameSize.Y) * PlayerTexture.Width];
                }
            }
            torso = new Color[frameSize.X * frameSize.Y];
            for (int y = 0; y < frameSize.Y; ++y)
            {
                for (int x = 0; x < frameSize.X; ++x)
                {
                    torso[y * frameSize.X + x] = tex[x + (currTorsoFrame.X * frameSize.X) + (y + currTorsoFrame.Y * frameSize.Y) * PlayerTexture.Width];
                }
            }
            draw = new Color[frameSize.X * (2 * frameSize.Y)];
            for (int y = 0; y < frameSize.Y; ++y)
            {
                for (int x = 0; x < frameSize.X; ++x)
                {
                    draw[(y + frameSize.Y) * frameSize.X + x] = legs[y * frameSize.X + x];
                    draw[y * frameSize.X + x] = torso[y * frameSize.X + x];
                }
            }
            DrawTexBuf = new Texture2D[2];
            DrawTexBuf[0] = new Texture2D(game.GraphicsDevice, frameSize.X, frameSize.Y * 2);
            DrawTexBuf[1] = new Texture2D(game.GraphicsDevice, frameSize.X, frameSize.Y * 2);
            drawTex = 0;
            DrawTexBuf[drawTex].SetData(draw);
            ++drawTex;
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

            if (keyState.IsKeyDown(Keys.A))
            {
                PlayerVelocity.X -= playerSpeed;
            }
            else if (keyState.IsKeyDown(Keys.D))
            {
                PlayerVelocity.X += playerSpeed;
            }


            if (PlayerVelocity.X < 0)
            {
                prevLegFrame.Y = currLegFrame.Y;
                currLegFrame.Y = 1;
            }
            if (PlayerVelocity.X > 0)
            {
                prevLegFrame.Y = currLegFrame.Y;
                currLegFrame.Y = 3;
            }

            sinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (keyState.IsKeyUp(Keys.D) && keyState.IsKeyUp(Keys.A))
            {
                prevLegFrame.X = currLegFrame.X;
                currLegFrame.X = 0;
                if (msPerFrame < sinceLastFrame)
                {
                    sinceLastFrame -= msPerFrame;
                }
            }
            else if (msPerFrame < sinceLastFrame)
            {
                sinceLastFrame -= msPerFrame;
                prevLegFrame.X = currLegFrame.X;
                ++currLegFrame.X;
                if (currLegFrame.X >= sheetSize.X)
                {
                    currLegFrame.X = 1;
                }
            }

            DrawLegs = new Rectangle(currLegFrame.X * frameSize.X, currLegFrame.Y * frameSize.Y, frameSize.X, frameSize.Y);

            if (currLegFrame.X != prevLegFrame.X || currLegFrame.Y != prevLegFrame.Y)
            {
                for (int y = 0; y < frameSize.Y; ++y)
                {
                    for (int x = 0; x < frameSize.X; ++x)
                    {
                        legs[y * frameSize.X + x] = tex[x + (currLegFrame.X * frameSize.X) + (y + currLegFrame.Y * frameSize.Y) * PlayerTexture.Width];
                    }
                }
            }

            // dobra klatka gorna

            MouseState mouseState = Mouse.GetState();

            Point origin = new Point((int)(PlayerPosition.X + 0.5f * frameSize.X), (int)(PlayerPosition.Y + 0.5f * frameSize.Y));
            Point mouse = new Point(mouseState.X, mouseState.Y);

            float delta = mouse.X - origin.X;
            if (delta == 0)
                delta = float.Epsilon;

            float tga = (mouse.Y - origin.Y) / delta;

            if (tga > 1.903)                                // tan(90 - (180/7))
            {
                prevTorsoFrame.X = currTorsoFrame.X;
                currTorsoFrame.X = 6;
            }
            else if (tga > 0.797)
            {
                prevTorsoFrame.X = currTorsoFrame.X;
                currTorsoFrame.X = 5;
            }
            else if (tga > 0.228)
            {
                prevTorsoFrame.X = currTorsoFrame.X;
                currTorsoFrame.X = 4;
            }
            else if (tga > -0.228)
            {
                prevTorsoFrame.X = currTorsoFrame.X;
                currTorsoFrame.X = 3;
            }
            else if (tga > -0.797)
            {
                prevTorsoFrame.X = currTorsoFrame.X;
                currTorsoFrame.X = 2;
            }
            else if (tga > -1.903)
            {
                prevTorsoFrame.X = currTorsoFrame.X;
                currTorsoFrame.X = 1;
            }
            else
            {
                prevTorsoFrame.X = currTorsoFrame.X;
                currTorsoFrame.X = 0;
            }

            if (mouse.X < origin.X)
            {
                prevTorsoFrame.Y = currTorsoFrame.Y;
                currTorsoFrame.Y = 0;
            }
            else
            {
                prevTorsoFrame.Y = currTorsoFrame.Y;
                currTorsoFrame.Y = 2;
            }

            DrawTorso = new Rectangle(currTorsoFrame.X * frameSize.X, currTorsoFrame.Y * frameSize.Y, frameSize.X, frameSize.Y);

            if (currTorsoFrame.X != prevTorsoFrame.X || currTorsoFrame.Y != prevTorsoFrame.Y)
            {
                for (int y = 0; y < frameSize.Y; ++y)
                {
                    for (int x = 0; x < frameSize.X; ++x)
                    {
                        torso[y * frameSize.X + x] = tex[x + (currTorsoFrame.X * frameSize.X) + (y + currTorsoFrame.Y * frameSize.Y) * PlayerTexture.Width];
                    }
                }
            }

            // merge tekstur (CPU)

            


            //
            PrevPlayerPosition = PlayerPosition;
            PlayerPosition += PlayerVelocity;

            CollisionRect = new Rectangle((int)PlayerPosition.X, (int)PlayerPosition.Y, frameSize.X * 2, frameSize.Y * 2);

            PlayerVelocity.X *= 0.35f;

        }

        public override void Draw( GameTime gameTime )
        {

            if (currLegFrame.X != prevLegFrame.X || currLegFrame.Y != prevLegFrame.Y || currTorsoFrame.X != prevTorsoFrame.X || currTorsoFrame.Y != prevTorsoFrame.Y)
            {
                ++drawTex;
                if (drawTex == 2)
                    drawTex = 0;
                for (int y = 0; y < frameSize.Y; ++y)
                {
                    for (int x = 0; x < frameSize.X; ++x)
                    {
                        draw[(y + frameSize.Y) * frameSize.X + x] = legs[y * frameSize.X + x];
                        draw[y * frameSize.X + x] = torso[y * frameSize.X + x];
                    }
                }
                DrawTexBuf[drawTex].SetData(draw);
            }

            sb.Draw(DrawTexBuf[drawTex], PlayerPosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            //sb.Draw(PlayerTexture, PlayerPosition, DrawTorso, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            //sb.Draw(PlayerTexture, new Vector2(PlayerPosition.X, PlayerPosition.Y + frameSize.Y), DrawLegs, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

    }
}
