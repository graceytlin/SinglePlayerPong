using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SinglePlayerPong
{
    public class PongGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Rectangle playAreaRectangle;
        Color playAreaColor;
        Texture2D blankTexture;

        SpriteFont centerFont;
        SpriteFont scoreFont;

        Vector2 centreVector;
        Vector2 scoreVector;
        Vector2 subtitleVector;
        Vector2 livesVector;

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
        KeyboardState keyb;

        int gameState;
        const int WAITING = 0, PLAYING = 1, PAUSED = 2, GAME_OVER = 3;

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
            gameState = WAITING;

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

            keyb = Keyboard.GetState();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            ballTexture = Content.Load<Texture2D>("ball");
            paddleTexture = Content.Load<Texture2D>("paddle");

            centerFont = Content.Load<SpriteFont>("PongFont");
            scoreFont = Content.Load<SpriteFont>("ScoreFont");

        }

        protected override void Update(GameTime gameTime)
        {
            keyb = Keyboard.GetState();
            GamePadCapabilities capabilities = GamePad.GetCapabilities(PlayerIndex.One);

            if (capabilities.IsConnected)
                pad1 = GamePad.GetState(PlayerIndex.One);

            if (gameState != PLAYING)
            { 
                if (keyb.IsKeyDown(Keys.Space) || pad1.Buttons.Start == ButtonState.Pressed)
                {
                    if (gameState != PAUSED)
                    {
                        lives = 3;
                        livesText = "lives: " + lives;
                    }

                    if (gameState == GAME_OVER)
                    {
                        ballRectangle.Y = playAreaRectangle.Top;
                        ballRectangle.X = (GraphicsDevice.Viewport.Width / 2) - (ballRectangle.Width / 2);
                        score = 0;
                    }

                    gameState = PLAYING;
                    ballXSpeed = playAreaRectangle.Width / 160;
                    ballYSpeed = playAreaRectangle.Width / 160;
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
            }

            if ((gameState == WAITING || gameState == GAME_OVER) && (keyb.IsKeyDown(Keys.Escape) || pad1.Buttons.B == ButtonState.Pressed))
                Exit();

            if (gameState == PLAYING)
            {
                score++;
                if (paddleRectangle.Left < playAreaRectangle.Left)
                    paddleRectangle.X = playAreaRectangle.Left;
                if (paddleRectangle.Right > playAreaRectangle.Right)
                    paddleRectangle.X = playAreaRectangle.Right - paddleRectangle.Width;

                if (ballRectangle.Left < playAreaRectangle.Left)
                    ballRectangle.X = playAreaRectangle.Left;
                if (ballRectangle.Right > playAreaRectangle.Right)
                    ballRectangle.X = playAreaRectangle.Right - ballRectangle.Width;
                if (ballRectangle.Top < playAreaRectangle.Top)
                    ballRectangle.Y = playAreaRectangle.Top;

                if (ballRectangle.X == playAreaRectangle.Left || ballRectangle.X == (playAreaRectangle.Right - ballRectangle.Width))
                    ballXSpeed *= -1;

                if (ballRectangle.Y == playAreaRectangle.Top)
                    ballYSpeed *= -1;

                if (ballRectangle.Intersects(paddleRectangle))
                {
                    ballYSpeed *= -1;
                    ballRectangle.Y = paddleRectangle.Top - ballRectangle.Height;
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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightSkyBlue);

            _spriteBatch.Begin();

            _spriteBatch.Draw(blankTexture, playAreaRectangle, playAreaColor);
            _spriteBatch.Draw(paddleTexture, paddleRectangle, paddleColor);

            if (gameState == PLAYING || gameState == GAME_OVER)
            {
                _spriteBatch.Draw(ballTexture, ballRectangle, ballColor);
            }

            if (gameState != WAITING)
            {
                _spriteBatch.DrawString(scoreFont, scoreText, scoreVector, Color.Black);
                _spriteBatch.DrawString(scoreFont, livesText, livesVector, Color.Black);
            }

            if (gameState != PLAYING)
            {
                _spriteBatch.DrawString(centerFont, centreText, centreVector, Color.Gray);
                _spriteBatch.DrawString(scoreFont, subtitleText, subtitleVector, Color.Gray);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
