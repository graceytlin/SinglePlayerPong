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

        public PongGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
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

            keyb = Keyboard.GetState();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            ballTexture = Content.Load<Texture2D>("ball");
            paddleTexture = Content.Load<Texture2D>("paddle");

        }

        protected override void Update(GameTime gameTime)
        {
            keyb = Keyboard.GetState();
            GamePadCapabilities capabilities = GamePad.GetCapabilities(PlayerIndex.One);

            if (capabilities.IsConnected)
                pad1 = GamePad.GetState(PlayerIndex.One);

            if (keyb.IsKeyDown(Keys.Right) || keyb.IsKeyDown(Keys.D) || pad1.ThumbSticks.Left.X > 0)
            {
                paddleRectangle.X += paddleSpeed;
                if (ballYSpeed == 0)
                {
                    ballXSpeed = playAreaRectangle.Width / 160;
                    ballYSpeed = playAreaRectangle.Width / 160;
                }
            }

            else if (keyb.IsKeyDown(Keys.Left) || keyb.IsKeyDown(Keys.A) || pad1.ThumbSticks.Left.X < 0)
            {
                paddleRectangle.X -= paddleSpeed;
                if (ballYSpeed == 0)
                {
                    ballXSpeed = playAreaRectangle.Width / 160;
                    ballYSpeed = playAreaRectangle.Width / 160;
                }
            }

            if (ballYSpeed == 0 && (keyb.IsKeyDown(Keys.Escape) || pad1.Buttons.B == ButtonState.Pressed))
                Exit();

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
            if (ballRectangle.Bottom > playAreaRectangle.Bottom)
                ballRectangle.X = playAreaRectangle.Bottom - ballRectangle.Height;

            if (ballRectangle.X == playAreaRectangle.Left || ballRectangle.X == (playAreaRectangle.Right - ballRectangle.Width))
                ballXSpeed *= -1;

            if (ballRectangle.Intersects(paddleRectangle) || ballRectangle.Y == playAreaRectangle.Top)
                ballYSpeed *= -1;

            if (ballRectangle.Bottom > playAreaRectangle.Bottom)
            {
                ballRectangle.Y = playAreaRectangle.Top;
                ballRectangle.X = (GraphicsDevice.Viewport.Width / 2) - (ballRectangle.Width / 2);
                ballYSpeed = 0;
                ballXSpeed = 0;
                paddleRectangle.X = (GraphicsDevice.Viewport.Width / 2) - (paddleRectangle.Width / 2);
            }


            ballRectangle.X += ballXSpeed;
            ballRectangle.Y += ballYSpeed;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightSkyBlue);

            _spriteBatch.Begin();
            _spriteBatch.Draw(blankTexture, playAreaRectangle, playAreaColor);
            _spriteBatch.Draw(ballTexture, ballRectangle, ballColor);
            _spriteBatch.Draw(paddleTexture, paddleRectangle, paddleColor);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
