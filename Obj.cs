using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Profanity;
using Profanity.Components;
using Component = Profanity.Components.Component;
using Vector3 = Microsoft.Xna.Framework.Vector3;

public class Obj
{
    public List<Component> components = new List<Component>();
    public Obj GameObject;
    public Obj[] ChildObjects;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;


    public Obj()
    {
        Inst();
    }

    public Obj(string name, string modelFile)
    {
        Inst();
    }

    public void Inst()
    {
        GameObject = this;
        position = Vector3.Zero;
        rotation = Vector3.Zero;
        scale = Vector3.One;
    }

    public bool ContainsComp<T>() where T : Component
    {

        return components.Any(c => c.GetType() == Activator.CreateInstance<T>().GetType());
    }

    public T AddComponent<T>() where T : Component
    {
        if (!ContainsComp<T>())
        {
            components.Add(Activator.CreateInstance<T>());
            return GetComponent<T>();
        }

        return null;
    }

    public T GetComponent<T>() where T : Component
    {
        foreach (Component comp in components)
        {
            if (comp.GetType() == Activator.CreateInstance<T>().GetType())
            {
                return (T)comp;
            }
        }

        return null;
    }

    public void Update()
    {

    }

    private void DrawModel2()
    {
        Model model = GetComponent<Model3D>()._model;
        Camera _camera = Profanity.Profanity._camera;
        GraphicsDevice gpu = Profanity.Profanity.gpu;
        
        Matrix worldMatrix = Matrix.CreateScale(new Vector3(0.1f,0.1f,0.1f) * scale) * Matrix.CreateTranslation(position);
        
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
                    basicEffect.World = modelTransforms[mesh.ParentBone.Index] * worldMatrix;
                    basicEffect.EnableDefaultLighting();
                    basicEffect.DirectionalLight0.Direction = new Vector3(4, -1, 2);
                    basicEffect.DirectionalLight0.DiffuseColor = new Vector3(1f, 1, 1f);
                    basicEffect.EmissiveColor = new Vector3(0.1f, 0.1f, 0.1f);
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

    public void Draw()
    {
        if (ContainsComp<Model3D>())
        {
            DrawModel2();
        }
    }
}
