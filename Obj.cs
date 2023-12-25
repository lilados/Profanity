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

    public String meshFile
    {
        set
        {
            objName = value;
            GetComponent<Model3D>().SetModel(value);
            SetCollider();
        }
    }

    public String objName;
    public Obj[] ChildObjects;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

    public BoundingBox collider;


    public Obj()
    {
        Inst();
    }


    public void Inst()
    {
        GameObject = this;
        AddComponent<Model3D>();
        position = Vector3.Zero;
        rotation = Vector3.Zero;
        scale = Vector3.One;
    }

    private void SetCollider()
    {
        Vector3 min = new Vector3(float.MaxValue);
        Vector3 max = new Vector3(float.MinValue);
        Model model = GetComponent<Model3D>()._model;

        foreach (ModelMesh mesh in model.Meshes)
        {
            foreach (ModelMeshPart part in mesh.MeshParts)
            {
                int vertexStride = part.VertexBuffer.VertexDeclaration.VertexStride;
                int vertexBufferSize = part.NumVertices * vertexStride;

                int vertexDataSize = vertexBufferSize / sizeof(float);
                float[] vertexData = new float[vertexDataSize];
                part.VertexBuffer.GetData(vertexData);
                for (int i = 0; i < vertexDataSize; i+= vertexStride / sizeof(float))
                {
                    Vector3 vertex = new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]);
                    min = Vector3.Min(min, vertex);
                    max = Vector3.Max(max, vertex);
                }
            }
        }
        Console.WriteLine(objName + min + max);
        collider = new BoundingBox(min, max);
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

        return AddComponent<T>();
    }

    public void Update()
    {
        
    }

    private void DrawModel()
    {
        Model model = GetComponent<Model3D>()._model;
        Camera _camera = Profanity.Profanity._camera;
        GraphicsDevice gpu = Profanity.Profanity.gpu;
        
        Matrix worldMatrix = Matrix.CreateScale(0.1f * scale) * Matrix.CreateTranslation(position) * 
                             Matrix.CreateFromYawPitchRoll(rotation.X,rotation.Y,rotation.Z);
        
        Matrix[] modelTransforms = new Matrix[model.Bones.Count];
        model.CopyAbsoluteBoneTransformsTo(modelTransforms);
        
        foreach (ModelMesh mesh in model.Meshes)
        {
            foreach (ModelMeshPart meshPart in mesh.MeshParts)
            {
                var effect2 = meshPart.Effect;
                BasicEffect basicEffect = (BasicEffect)effect2;
                if (basicEffect != null)
                {
                    basicEffect.World = modelTransforms[mesh.ParentBone.Index] * worldMatrix;
                    basicEffect.EnableDefaultLighting();
                    basicEffect.DirectionalLight0.Direction = new Vector3(4, -1, 2);
                    basicEffect.DirectionalLight0.DiffuseColor = new Vector3(1f, 1, 1f);
                    basicEffect.DirectionalLight0.SpecularColor = new Vector3(0.6f, 0.3f, 1);
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
            DrawModel();
        }
    }
}
