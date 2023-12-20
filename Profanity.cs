using System;
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
    private GraphicsDevice _device;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Input inp;
    private RenderTarget2D MainTarget;


    private Texture2D _texture;
    public static Effect effect;
    

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
        inp = new Input(pp, MainTarget);
        _device = _graphics.GraphicsDevice;
        base.Initialize();
    }
    protected override void LoadContent()
    {
        effect = Content.Load<Effect>("effects");

        obj.AddComponent<Model3D>().SetModel("kuklafbx", effect);
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
            _camera.GetComponent<Transform>().position += new Vector3(-GamePad.GetState(0).ThumbSticks.Left.X * 2, GamePad.GetState(0).ThumbSticks.Right.Y *4,
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

    private void DrawModel2(Model model)
    {
        Matrix[] modelTransforms = new Matrix[model.Bones.Count];
        model.CopyAbsoluteBoneTransformsTo(modelTransforms);
        
        foreach (ModelMesh mesh in model.Meshes)
        {
            foreach (ModelMeshPart meshPart in mesh.MeshParts)
            {
                var effect2 = meshPart.Effect;

                BasicEffect basicEffect = effect2 as BasicEffect;
                if (basicEffect != null)
                {
                    basicEffect.World = modelTransforms[mesh.ParentBone.Index] * Matrix.CreateScale(new Vector3(0.01f,0.01f,0.01f)) * Matrix.Identity;
                    basicEffect.EnableDefaultLighting();
                    basicEffect.DirectionalLight0.Direction = new Vector3(4, -1, 2);
                    basicEffect.Projection = _camera.proj;
                    basicEffect.View = _camera.view;
                    basicEffect.Alpha = 1.0f;
                }
                else
                {
                    effect2.CurrentTechnique = effect2.Techniques["Colored"];
                    effect2.Parameters["xEnableLighting"].SetValue(true);
                    effect2.Parameters["xLightDirection"].SetValue(new Vector3(0,-2,5));
                    effect2.Parameters["xWorld"].SetValue(modelTransforms[mesh.ParentBone.Index] * Matrix.Identity);
                    effect2.Parameters["xView"].SetValue(_camera.view);
                    effect2.Parameters["xProjection"].SetValue(_camera.proj);
                }
                gpu.SetVertexBuffer(meshPart.VertexBuffer);
                gpu.Indices = meshPart.IndexBuffer;
                foreach (var pass in effect2.CurrentTechnique.Passes)    
                {
                    pass.Apply();
                    gpu.DrawIndexedPrimitives(PrimitiveType.TriangleList, meshPart.VertexOffset, meshPart.StartIndex, meshPart.PrimitiveCount);
                }
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        gpu.SetRenderTarget(MainTarget);
        Set3DStates();
        gpu.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

        DrawModel2(Content.Load<Model>("drz"));
        //obj.Draw(_camera);
        
        gpu.SetRenderTarget(null);
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.None,
            RasterizerState.CullNone);
        _spriteBatch.Draw(MainTarget, desktopRect, Color.White);
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }
}