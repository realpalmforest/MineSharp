using MineSharp.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.ImGuiNet;

namespace MineSharp
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static ImGuiRenderer GuiRenderer;
        public static Game1 Instance;

        public Game1()
        {
            Instance = this;

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;

            // _graphics.GraphicsProfile = GraphicsProfile.HiDef;

            _graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;

            _graphics.PreferMultiSampling = false;

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            Window.IsBorderless = true;
            Window.Position = Point.Zero;
        }

        protected override void Initialize()
        {
            GuiRenderer = new ImGuiRenderer(this);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            GuiRenderer.RebuildFontAtlas();

            Globals.Load(_spriteBatch, GraphicsDevice, Content);
            GameManager.Load();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Globals.Update(gameTime);

            GameManager.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GameManager.Draw();
            base.Draw(gameTime);


            GuiRenderer.BeginLayout(gameTime);
            GameManager.DrawImGui();
            GuiRenderer.EndLayout();
        }
    }
}
