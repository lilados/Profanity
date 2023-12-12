using Microsoft.Xna.Framework;
using Profanity.Components;

public class Light : Component{
    public bool defaultLight = true;
    public Vector3 lightDir = Vector3.One;
    public float lightStrenght = 1;

    public Light(){
        defaultLight = true;
    }

}