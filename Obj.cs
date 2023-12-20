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
        AddComponent<Transform>();
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

    private void DrawModel(Camera camera)
    {
        Model model = GetComponent<Model3D>()._model;
        Transform trs = GetComponent<Transform>();
        Matrix worldMatrix = Matrix.CreateScale(new Vector3(0.010f,0.001f,0.001f) * trs.scale) * Matrix.CreateTranslation(trs.position);
        

        Matrix[] modelTransforms = new Matrix[model.Bones.Count];
        model.CopyAbsoluteBoneTransformsTo(modelTransforms);

        foreach (ModelMesh mesh in model.Meshes)
        {
            foreach (Effect currentEffect in mesh.Effects)
            {
                currentEffect.CurrentTechnique = currentEffect.Techniques["Colored"];
                currentEffect.Parameters["xEnableLighting"].SetValue(true);
                currentEffect.Parameters["xAmbient"].SetValue(0.9f);
                currentEffect.Parameters["xWorld"].SetValue(modelTransforms[mesh.ParentBone.Index] * worldMatrix);
                currentEffect.Parameters["xView"].SetValue(camera.view);
                currentEffect.Parameters["xProjection"].SetValue(camera.proj);
            }
            mesh.Draw();
        }
    }
    

    public void Draw(Camera camera)
    {
        if (ContainsComp<Model3D>())
        {
            DrawModel(camera);
        }
    }
}
