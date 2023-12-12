using Microsoft.Xna.Framework;
using Profanity.Components;

class Transform : Component
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

    public Transform(){
        position = Vector3.Zero;
        rotation = Vector3.Zero;
        scale = Vector3.One;
    }
    public override string ToString()
    {
        return "NEW TRANSFORM";//(position + rotation + scale).ToString();
    }
}