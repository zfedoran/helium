using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using helium.core;
using helium.resources;
using helium.animation;

namespace helium
{
    /// <summary>
    /// XNA Application class which handles window setup and the game loop
    /// </summary>
    public class Application : Game
    {
        public const bool               VSINC = true;
        public const int                WIDTH = 1280;
        public const int                HEIGHT = 720;
        public const bool               FULLSCREEN = false;
        public static readonly Color    BACKGROUND_COLOR = new Color(0.3f, 0.3f, 0.3f);

        private GraphicsDeviceManager graphics;
        private GraphicsDevice device;
        private ContentManager content;

        private SpriteBatch spritebatch;
        private SkinnedEffect skinnedeffect;
        private Camera camera;
        private ExampleModelClass dude;
        private float elapsed, time;

        public Application()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            SetupGraphicsDeviceManager(graphics);
        }

        protected void SetupGraphicsDeviceManager(GraphicsDeviceManager graphics)
        {
            if (graphics != null)
            {
                graphics.IsFullScreen = FULLSCREEN;
                graphics.PreferredBackBufferWidth = WIDTH;
                graphics.PreferredBackBufferHeight = HEIGHT;
                graphics.SynchronizeWithVerticalRetrace = VSINC;
                IsFixedTimeStep = VSINC;
            }
        }

        protected override void Initialize()
        {
            device = GraphicsDevice;
            content = Content;

            camera = new Camera();
            camera.width = Application.WIDTH;
            camera.height = Application.HEIGHT;
            camera.near = 0.1f;
            camera.far = 500;
            camera.fov = 1.1f;
            camera.position = new Vector3(3, 5, 10);
            camera.target = new Vector3(0, 2, 0);
            camera.up = Vector3.UnitY;

            base.Initialize();
        }


        protected override void Update(GameTime gameTime)
        {
            elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            time = (float)gameTime.TotalGameTime.TotalSeconds;

            camera.position.X = (float)Math.Cos(time * 0.05f) * 5;
            camera.position.Z = (float)Math.Sin(time * 0.05f) * 5;
            camera.position.Y = (float)Math.Sin(time * 0.05f) * 2 + 3;

            camera.Update(elapsed);
            dude.Update(elapsed);
        }

        protected override void Draw(GameTime gameTime)
        {
            device.Clear(BACKGROUND_COLOR);

            spritebatch.Begin();
            spritebatch.DrawString(ResourceLibrary.debugfont, (1/elapsed).ToString("0"), Vector2.One, Color.White);
            spritebatch.End();

            device.DepthStencilState = DepthStencilState.Default;

            skinnedeffect.View = camera.view;
            skinnedeffect.Projection = camera.projection;


            dude.Draw(device);
        }

        protected override void LoadContent()
        { 
            ResourceLibrary.Load(content, device);

            spritebatch = new SpriteBatch(device);
            
            skinnedeffect = new SkinnedEffect(device);
            skinnedeffect.EnableDefaultLighting();
            skinnedeffect.FogEnabled = false;
            skinnedeffect.PreferPerPixelLighting = true;

            dude = new ExampleModelClass(ResourceLibrary.dude, skinnedeffect);
            dude.SetWorld(Matrix.CreateScale(0.05f));
            dude.PlayAnimation(0, -1.0f, true);
        }

        protected override void UnloadContent()
        { ResourceLibrary.Unload(); }

        private static Application instance;

        static Application()
        { instance = new Application(); }

        public static Application GetInstance()
        { return instance; }
    }

#if WINDOWS || XBOX
    static class Program
    {
        static void Main(string[] args)
        {
            using (Application game = Application.GetInstance())
            {
                game.Run();
            }
        }
    }
#endif
}

