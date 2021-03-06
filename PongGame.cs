﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace SinglePlayerPong
{
    public class PongGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Rectangle playAreaRectangle;
        Color playAreaColor;
        Texture2D blankTexture;
        Texture2D splashScreen;

        SpriteFont centerFont;
        SpriteFont scoreFont;

        Vector2 centreVector;
        Vector2 scoreVector;
        Vector2 subtitleVector;
        Vector2 livesVector;

        SoundEffect paddleBounce;
        SoundEffect missSound;
        SoundEffect wallBounce;
        SoundEffect startSound;

        string subtitleText;
        string scoreText;
        string centreText;
        string livesText;

        Texture2D ballTexture;
        Rectangle ballRectangle;
        Color ballColor;

        int ballXSpeed;
        int ballYSpeed;
        int paddleSpeed;

        Texture2D paddleTexture;
        Rectangle paddleRectangle;
        Color paddleColor;

        GamePadState pad1;
        GamePadState prevState;
        KeyboardState keyb;
        KeyboardState prevKeyb;

        int gameState;
        const int WAITING = 0, PLAYING = 1, PAUSED = 2, GAME_OVER = 3, SPLASH = 4;

        int scoreTimer;
        const int TICKS = 7;

        int lives;
        int score;

        public PongGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            gameState = SPLASH;

            playAreaRectangle = new Rectangle(50, 40, 700, 400);
            playAreaColor = Color.Honeydew;
            blankTexture = new Texture2D(GraphicsDevice, 1, 1);
            blankTexture.SetData(new[] { Color.White });

            ballRectangle.Width = playAreaRectangle.Width / 40;
            ballRectangle.Height = playAreaRectangle.Width / 40;
            ballRectangle.Y = playAreaRectangle.Top;
            ballRectangle.X = (GraphicsDevice.Viewport.Width / 2) - (ballRectangle.Width / 2);
            ballColor = Color.HotPink;

            ballXSpeed = 0;
            ballYSpeed = 0;
            paddleSpeed = playAreaRectangle.Width / 160;

            paddleRectangle.Width = playAreaRectangle.Width / 40 * 4;
            paddleRectangle.Height = playAreaRectangle.Width / 40;
            paddleRectangle.Y = playAreaRectangle.Bottom - paddleRectangle.Height;
            paddleRectangle.X = (GraphicsDevice.Viewport.Width / 2) - (paddleRectangle.Width / 2);
            paddleColor = Color.Black;

            lives = 3;
            score = 0;

            subtitleText = "press [space] to start";
            scoreText = "score: " + score;
            centreText = "PONG";
            livesText = "lives: " + lives;

            centreVector.X = 0;
            centreVector.Y = 0;
            scoreVector.X = 5;
            scoreVector.Y = 5;
            subtitleVector.X = 0;
            subtitleVector.Y = 0;
            livesVector.X = 0;
            livesVector.Y = 5;

            scoreTimer = TICKS;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            splashScreen = Content.Load<Texture2D>("splashscreen");

            ballTexture = Content.Load<Texture2D>("ball");
            paddleTexture = Content.Load<Texture2D>("paddle");

            centerFont = Content.Load<SpriteFont>("PongFont");
            scoreFont = Content.Load<SpriteFont>("ScoreFont");

            paddleBounce = Content.Load<SoundEffect>("Audio/PaddleBounceSound");
            wallBounce = Content.Load<SoundEffect>("Audio/WallBounceSound");
            startSound = Content.Load<SoundEffect>("Audio/StartSound");
            missSound = Content.Load<SoundEffect>("Audio/MissSound");

        }

        protected override void Update(GameTime gameTime)
        {
            keyb = Keyboard.GetState();
            GamePadCapabilities capabilities = GamePad.GetCapabilities(PlayerIndex.One);

            if (capabilities.IsConnected)
                pad1 = GamePad.GetState(PlayerIndex.One);

            if (gameState == SPLASH || gameState == GAME_OVER)
            {
                if ((keyb.IsKeyDown(Keys.Space) && prevKeyb.IsKeyUp(Keys.Space)) || (pad1.Buttons.Start == ButtonState.Pressed && prevState.Buttons.Start == ButtonState.Released))
                {
                    if (gameState == GAME_OVER)
                    {
                        ballRectangle.Y = playAreaRectangle.Top;
                        ballRectangle.X = (GraphicsDevice.Viewport.Width / 2) - (ballRectangle.Width / 2);
                        score = 0;
                        subtitleText = "press [space] to start";
                        centreText = "PONG";
                    }

                    gameState = WAITING;
                    startSound.Play(1, 0, 0);
                }
            }
            else if ((keyb.IsKeyDown(Keys.Space) && prevKeyb.IsKeyUp(Keys.Space)) || (pad1.Buttons.Start == ButtonState.Pressed && prevState.Buttons.Start == ButtonState.Released))
            {
                if (gameState != PAUSED && gameState != PLAYING)
                {
                    lives = 3;
                    livesText = "lives: " + lives;
                    startSound.Play(1, 0, 0);
                }
                
                if (gameState != PLAYING)
                {
                    ballXSpeed = playAreaRectangle.Width / 160;
                    ballYSpeed = playAreaRectangle.Width / 160;

                    gameState = PLAYING;
                    startSound.Play(1, 0, 0);
                }
            }

            if (gameState == PAUSED)
            {
                subtitleText = "press [space] to continue";
                centreText = "PAUSED";
            }

            if (gameState == PLAYING || gameState == PAUSED)
            {
                if (keyb.IsKeyDown(Keys.Left) || keyb.IsKeyDown(Keys.A) || pad1.ThumbSticks.Left.X < 0)
                {
                    paddleRectangle.X -= paddleSpeed;
                }

                if (keyb.IsKeyDown(Keys.Right) || keyb.IsKeyDown(Keys.D) || pad1.ThumbSticks.Left.X > 0)
                {
                    paddleRectangle.X += paddleSpeed;
                }

                if (paddleRectangle.Left <= playAreaRectangle.Left)
                    paddleRectangle.X = playAreaRectangle.Left;
                if (paddleRectangle.Right >= playAreaRectangle.Right)
                    paddleRectangle.X = playAreaRectangle.Right - paddleRectangle.Width;

                if (ballRectangle.Left <= playAreaRectangle.Left)
                {
                    ballRectangle.X = playAreaRectangle.Left;
                    wallBounce.Play(1, 0, 0);
                }
                    
                    
                if (ballRectangle.Right >= playAreaRectangle.Right)
                {
                    ballRectangle.X = playAreaRectangle.Right - ballRectangle.Width;
                    wallBounce.Play(1, 0, 0);
                }
                    
                if (ballRectangle.Top < playAreaRectangle.Top)
                {
                    ballRectangle.Y = playAreaRectangle.Top;
                    wallBounce.Play(1, 0, 0);
                }
            }

            if ((gameState == WAITING || gameState == GAME_OVER || gameState == SPLASH) && (keyb.IsKeyDown(Keys.Escape) || pad1.Buttons.B == ButtonState.Pressed))
                Exit();

            if (gameState == PLAYING)
            {
                score++;

                if (ballRectangle.X == playAreaRectangle.Left || ballRectangle.X == (playAreaRectangle.Right - ballRectangle.Width))
                    ballXSpeed *= -1;

                if (ballRectangle.Y == playAreaRectangle.Top)
                    ballYSpeed *= -1;

                if (ballRectangle.Intersects(paddleRectangle))
                {
                    ballYSpeed *= -1;
                    ballRectangle.Y = paddleRectangle.Top - ballRectangle.Height;
                    paddleBounce.Play();
                }

                if (ballRectangle.Bottom > playAreaRectangle.Bottom)
                {
                    if (lives > 1)
                    {
                        gameState = PAUSED;
                        ballRectangle.Y = playAreaRectangle.Top;
                        ballRectangle.X = (GraphicsDevice.Viewport.Width / 2) - (ballRectangle.Width / 2);
                        ballYSpeed = 0;
                        ballXSpeed = 0;
                    }
                    else
                    {
                        missSound.Play();
                        gameState = GAME_OVER;
                        subtitleText = "press [space] to start over";
                        centreText = "GAME OVER";
                        paddleRectangle.Y = playAreaRectangle.Bottom - paddleRectangle.Height;
                        paddleRectangle.X = (GraphicsDevice.Viewport.Width / 2) - (paddleRectangle.Width / 2);
                        ballYSpeed = 0;
                        ballXSpeed = 0;
                    }

                    lives -= 1;
                    livesText = "lives: " + lives;
                }
            }

            ballRectangle.X += ballXSpeed;
            ballRectangle.Y += ballYSpeed;

            scoreTimer--;

            if (scoreTimer == 0)
            {
                scoreTimer = TICKS;
                scoreText = "score: " + score;
            }

            centreVector.X = (GraphicsDevice.Viewport.Width / 2) - (centerFont.MeasureString(centreText).X/2);
            centreVector.Y = (GraphicsDevice.Viewport.Height / 2) - (centerFont.MeasureString(centreText).Y);
            subtitleVector.X = (GraphicsDevice.Viewport.Width / 2) - (scoreFont.MeasureString(subtitleText).X / 2);
            subtitleVector.Y = (GraphicsDevice.Viewport.Height / 2) + (scoreFont.MeasureString(subtitleText).Y);
            livesVector.X = GraphicsDevice.Viewport.Bounds.Right - scoreFont.MeasureString(livesText).X - 5;

            prevState = pad1;
            prevKeyb = keyb;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightSkyBlue);

            _spriteBatch.Begin();

            _spriteBatch.Draw(blankTexture, playAreaRectangle, playAreaColor);
            _spriteBatch.Draw(paddleTexture, paddleRectangle, paddleColor);

            if (gameState == SPLASH)
            {
                _spriteBatch.Draw(splashScreen, GraphicsDevice.Viewport.Bounds, Color.White);
            }

            if (gameState == PLAYING || gameState == GAME_OVER)
            {
                _spriteBatch.Draw(ballTexture, ballRectangle, ballColor);
            }

            if (gameState != WAITING && gameState != SPLASH)
            {
                _spriteBatch.DrawString(scoreFont, scoreText, scoreVector, Color.Black);
                _spriteBatch.DrawString(scoreFont, livesText, livesVector, Color.Black);
            }

            if (gameState != PLAYING && gameState != SPLASH)
            {
                _spriteBatch.DrawString(centerFont, centreText, centreVector, Color.Gray);
                _spriteBatch.DrawString(scoreFont, subtitleText, subtitleVector, Color.Gray);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
