using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Profanity.Components;

namespace Profanity;

public class Profanity : Game
{
    private const int ScreenWidth = 1920, ScreenHeight = 1080;
    
    public static GraphicsDevice gpu;
    public static Camera _camera;
    
    public static int screenW, screenH;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Input inp;
    private RenderTarget2D MainTarget;

    private String coordinates;
    private VertexPositionColor[] _vert;

    private Texture2D _texture;
    public static Effect effect;

    private float moveSpeed = 1;

    private Rectangle desktopRect;
    private Rectangle screenRect;
    public static List<Obj> gameObjects = new List<Obj>();

    public BoundingSphere testCollider = new BoundingSphere();
    public Vector3 colliderPos = new Vector3(0, 0, -100);
    
    private float camAngle = MathF.PI/2;

    public Obj Player = new Obj();

    public Profanity()
    {
        int desktop_width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
        int desktop_height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = 900,
            PreferredBackBufferHeight = 900,
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
        _camera.position = new Vector3(-200, 18, 0);
        inp = new Input(pp, MainTarget);
        base.Initialize();
    }
    private void SetVertices()
    {
        _vert = new VertexPositionColor[6];
        _vert[0].Position = new Vector3(1000, 0, 1000);
        _vert[0].Color = Color.Green;
        _vert[1].Position = new Vector3(1000, 0, -1000);
        _vert[1].Color = Color.Green;
        _vert[2].Position = new Vector3(-1000, 0, -1000);
        _vert[2].Color = Color.Green;
        _vert[3].Position = new Vector3(1000, 0, 1000);
        _vert[3].Color = Color.Green;
        _vert[4].Position = new Vector3(-1000, 0, 1000);
        _vert[4].Color = Color.Green;
        _vert[5].Position = new Vector3(-1000, 0, -1000);
        _vert[5].Color = Color.Green;
    }
    protected override void LoadContent()
    {
        effect = Content.Load<Effect>("_Effects/effects");
        Obj obj = new Obj
        {
            position = new Vector3(100, 0, 70),
            meshFile = "kuklafbx"
        };
        Obj obj2 = new Obj
        {
            scale = Vector3.One * 2,
            position = new Vector3(20,5,20),
            meshFile = "drz"
        };
        Obj obj4 = new Obj
        {
            meshFile = "player",
            position = new Vector3(0,0,0)
        };
        gameObjects.Add(obj);
        gameObjects.Add(obj2);
        gameObjects.Add(obj4);  
        
        SetVertices();
    }

    

    protected override void Update(GameTime gameTime)
    {
        inp.Update();
        _camera.UpdateCamera(gpu);
        foreach (Obj obj in gameObjects)
        {
            obj.Update();
        }
        if (inp.KeyDown(Keys.Escape))
            Exit();
        Vector3 camDir = _camera.target - _camera.position;
        camAngle -= GamePad.GetState(0).ThumbSticks.Right.X / 10;
        if(!inp.ctrl_down){
            camAngle += inp.horizontalDelta / 10;
        }
        _camera.position += 
            new Vector3(-GamePad.GetState(0).ThumbSticks.Left.X * 4, GamePad.GetState(0).ThumbSticks.Right.Y * 2,
                -GamePad.GetState(0).ThumbSticks.Left.Y * 4);
        Vector3 tempPos = Vector3.Zero;
        if (inp.KeyDown(Keys.S))
        {
            tempPos -= camDir;
        }
        if (inp.KeyDown(Keys.A))
        {
            tempPos += Vector3.Transform(camDir, Matrix.CreateRotationY(MathF.PI/2));
        }
        if (inp.KeyDown(Keys.D))
        {
            tempPos -= Vector3.Transform(camDir, Matrix.CreateRotationY(MathF.PI/2));
        }
        if (inp.KeyDown(Keys.W))
        {
            tempPos += camDir;
        }
        if (inp.ctrl_down && inp.rightClick)
        {
            _camera.position.Y = -4;
        }else if (inp.rightClick)
        {
            _camera.position.Y = 4;
        }
        if (inp.KeyDown(Keys.W))
        {
            tempPos += camDir;
        }
        _camera.position += tempPos;
        _camera.target = _camera.position + new Vector3((float)Math.Sin(MathHelper.ToRadians(camAngle)), 0, (float)Math.Cos(MathHelper.ToRadians(camAngle)));
        if (inp.KeyDown(Keys.G))
        {
            gameObjects[0].position -= camDir;
        }
        if (inp.KeyDown(Keys.F))
        {
            gameObjects[0].position += Vector3.Transform(camDir, Matrix.CreateRotationY(MathF.PI/2));
        }
        if (inp.KeyDown(Keys.H))
        {
            gameObjects[0].position -= Vector3.Transform(camDir, Matrix.CreateRotationY(MathF.PI/2));
        }
        if (inp.KeyDown(Keys.T))
        {
            gameObjects[0].position += camDir;
        }

        coordinates = _camera.position.ToString();
        

        testCollider = new BoundingSphere(_camera.position, 5f);
        
        if (testCollider.Contains(gameObjects[1].collider) == ContainmentType.Intersects)
        {
            Console.Write(gameObjects[2].objName);
        }
        
        base.Update(gameTime);
    }

    private void Set3DStates()
    {  
        gpu.BlendState = BlendState.NonPremultiplied;   
        gpu.DepthStencilState = DepthStencilState.Default;  
        RasterizerState rs = new RasterizerState();  
        rs.FillMode = FillMode.Solid;  
        if (gpu.RasterizerState.CullMode == CullMode.None)  
        {  
            rs.CullMode = CullMode.CullCounterClockwiseFace;  
            gpu.RasterizerState = rs;  
        }  
    }
    protected override void Draw(GameTime gameTime)
    {
        gpu.SetRenderTarget(MainTarget);
        Set3DStates();
        gpu.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

        foreach (Obj obj in gameObjects)
        {
            obj.Draw();   
        }
        RasterizerState rs = new RasterizerState();
        rs.CullMode = CullMode.None;
        rs.FillMode = FillMode.Solid;
        gpu.RasterizerState = rs;
        
        
        effect.CurrentTechnique = effect.Techniques["ColoredNoShading"];
        effect.Parameters["xView"].SetValue(_camera.view);
        effect.Parameters["xProjection"].SetValue(_camera.proj);
        effect.Parameters["xWorld"].SetValue(Matrix.Identity);
        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            gpu.DrawUserPrimitives(PrimitiveType.TriangleList, _vert, 0, _vert.Length/3, VertexPositionColor.VertexDeclaration);
        }
        gpu.SetRenderTarget(null);
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.None,
            RasterizerState.CullNone);
        _spriteBatch.DrawString(new SpriteFont(),coordinates,new Vector2(100,200), Color.White);
        _spriteBatch.Draw(MainTarget, desktopRect, Color.White);
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }
}