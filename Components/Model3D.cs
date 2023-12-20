using System;
using Microsoft.Xna.Framework.Graphics;

namespace Profanity.Components;

public class Model3D : Component
{
    public Model _model;
    public Model3D(){}

    public void SetModel(string fileName, Effect eff){
        Model newModel = General.content.Load<Model>(fileName);
        
        foreach (ModelMesh mesh in newModel.Meshes)
        {
            foreach (ModelMeshPart meshPart in mesh.MeshParts)
            {
                meshPart.Effect = eff.Clone();
            }
        }
        _model = newModel;
    }
    public Model3D(string fileName)
    {
        _model = General.content.Load<Model>(fileName);
    }
}