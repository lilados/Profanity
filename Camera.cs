using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Profanity;

public class Camera
{
    public const float FarPlane = 2000;
    public Vector3 pos, target;
    public Matrix view, proj, view_proj;
    public Vector3 up;
    private float currentAngle, angleVelocity, radius = 100.0f;
    private Vector3 unit_direction;

    private Input inp = General.input;

    public Camera(GraphicsDevice gpu, Vector3 upDir)
    {
        up = upDir;
        pos = new Vector3(0, 0, 10);
        target = new Vector3(0, 0, 0);
        view = Matrix.CreateLookAt(pos, target, up);
        proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, gpu.Viewport.AspectRatio, 0.1f, FarPlane);
        view_proj = view * proj;
        unit_direction = view.Forward; unit_direction.Normalize();
    }
    public void UpdateCamera(GraphicsDevice gpu){
        view = Matrix.CreateLookAt(pos, target, up);
        proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, gpu.Viewport.AspectRatio, 0.1f, FarPlane);
        view_proj = view * proj;
    }
}