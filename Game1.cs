using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Profanity;

public class Game1 : Game
{
    private const int ScreenWidth = 1920, ScreenHeight = 1080;
    private GraphicsDevice gpu;
    private SpriteFont font;
    public static int screenW, screenH;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Input inp;
    private RenderTarget2D MainTarget;

    private Rectangle desktopRect;
    private Rectangle screenRect;
    public Game1()
    {
        int desktop_width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 10;
        int desktop_height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 10;
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = desktop_width,
            PreferredBackBufferHeight = desktop_height,
            IsFullScreen = false, PreferredDepthStencilFormat = DepthFormat.None,
            GraphicsProfile = GraphicsProfile.HiDef
        };

        Window.IsBorderless = true;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        gpu = GraphicsDevice;
        PresentationParameters pp = gpu.PresentationParameters;
        _spriteBatch = new SpriteBatch(gpu);
        MainTarget = new RenderTarget2D(gpu, ScreenWidth, ScreenHeight, false, pp.BackBufferFormat, DepthFormat.Depth24);
        screenW = MainTarget.Width;
        screenH = MainTarget.Height;
        desktopRect = new Rectangle(0, 0, pp.BackBufferWidth, pp.BackBufferHeight);
        screenRect = new Rectangle(0, 0, screenW, screenH);

        inp = new Input(pp, MainTarget);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        font = Content.Load<SpriteFont>("Font");
    }

    protected override void Update(GameTime gameTime)
    {
        inp.Update();
        if (inp.KeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }
    private void Set3DStates()
    {
        gpu.BlendState = BlendState.NonPremultiplied; gpu.DepthStencilState = DepthStencilState.Default;
        if (gpu.RasterizerState.CullMode == CullMode.None)
        {
            RasterizerState rs = new RasterizerState { CullMode = CullMode.CullCounterClockwiseFace };
            gpu.RasterizerState = rs;
        }
    }
    protected override void Draw(GameTime gameTime)
    {
        gpu.SetRenderTarget(MainTarget);

        Set3DStates();
        gpu.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
        
        //Render Scene Objects
        
        
        
        //Draw mainTarget To BackBuffer
        gpu.SetRenderTarget(null);
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.None,
            RasterizerState.CullNone);
        _spriteBatch.Draw(MainTarget, desktopRect, Color.White);
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }

    
}