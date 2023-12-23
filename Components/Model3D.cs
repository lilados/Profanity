using System;
using Microsoft.Xna.Framework.Graphics;

namespace Profanity.Components;

public class Model3D : Component
{
    public Model _model;
    public Model3D(){}

    public void SetModel(string fileName){
        _model = General.content.Load<Model>("_Models/"+fileName);
    }
    public Model3D(string fileName)
    {
        _model = General.content.Load<Model>(fileName);
    }
}