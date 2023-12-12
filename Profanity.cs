using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Profanity.Components;

namespace Profanity;

public class Profanity : Game
{
    private const int ScreenWidth = 1920, ScreenHeight = 1080;
    private GraphicsDevice gpu;
    private SpriteFont font;
    public static int screenW, screenH;
    private GraphicsDevice _device;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    public static Camera _camera;
    private Input inp;
    private RenderTarget2D MainTarget;


    private Texture2D _texture;
    public static Effect effect;

    private VertexPositionTexture[] _vertices;

    private Rectangle desktopRect;
    private Rectangle screenRect;

    private Obj obj = new Obj();

    public Profanity()
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
        General.content = Content;
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
        _camera = new Camera(gpu, Vector3.Up);
        _camera.target = new Vector3(0,0,0);
        inp = new Input(pp, MainTarget);
        _device = _graphics.GraphicsDevice;
        base.Initialize();
    }
    private void SetUpVertices(){
        _vertices = new VertexPositionTexture[6];

        _vertices[0].Position = new Vector3(-10f, 10f, 0f);
            _vertices[0].TextureCoordinate.X = 0;
            _vertices[0].TextureCoordinate.Y = 0;

            _vertices[1].Position = new Vector3(10f, -10f, 0f);
            _vertices[1].TextureCoordinate.X = 1;
            _vertices[1].TextureCoordinate.Y = 1;

            _vertices[2].Position = new Vector3(-10f, -10f, 0f);
            _vertices[2].TextureCoordinate.X = 0;
            _vertices[2].TextureCoordinate.Y = 1;

            _vertices[3].Position = new Vector3(10.0f, -10.0f, 0f);
            _vertices[3].TextureCoordinate.X = 1;
            _vertices[3].TextureCoordinate.Y = 1;

            _vertices[4].Position = new Vector3(-10.0f, 10.0f, 0f);
            _vertices[4].TextureCoordinate.X = 0;
            _vertices[4].TextureCoordinate.Y = 0;

            _vertices[5].Position = new Vector3(10.0f, 10.0f, 0f);
            _vertices[5].TextureCoordinate.X = 1;
            _vertices[5].TextureCoordinate.Y = 0;
    }

    protected override void LoadContent()
    {
        font = Content.Load<SpriteFont>("Font");
        effect = Content.Load<Effect>("effects");
        _texture = Content.Load<Texture2D>("riemerstexture");
        //SetUpVertices();

        obj.AddComponent<Model3D>().SetModel("drzewko", effect);
    }

    protected override void Update(GameTime gameTime)
    {
        inp.Update();
        _camera.UpdateCamera(gpu);
        if (inp.KeyDown(Keys.Escape))
            Exit();
        if (GamePad.GetState(0).Triggers.Right > 0.5f)
        {
            _camera.target += new Vector3(-GamePad.GetState(0).ThumbSticks.Left.X * 4, GamePad.GetState(0).ThumbSticks.Right.Y * 2,
                -GamePad.GetState(0).ThumbSticks.Left.Y * 4);
        }else
        {
            _camera.pos += new Vector3(-GamePad.GetState(0).ThumbSticks.Left.X * 2, GamePad.GetState(0).ThumbSticks.Right.Y *4,
                -GamePad.GetState(0).ThumbSticks.Left.Y * 4);
        }

        base.Update(gameTime);
    }

    private void Set3DStates()
    {
        gpu.BlendState = BlendState.NonPremultiplied; 
        gpu.DepthStencilState = DepthStencilState.Default;
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
        
        obj.Draw(_camera);

        //Draw mainTarget To BackBuffer
        gpu.SetRenderTarget(null);
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.None,
            RasterizerState.CullNone);
        _spriteBatch.Draw(MainTarget, desktopRect, Color.White);
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }
}