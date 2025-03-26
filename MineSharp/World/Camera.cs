using MineSharp.Core;
using Microsoft.Xna.Framework;

namespace MineSharp.World;

public class Camera
{
    public Vector3 Direction = Vector3.Zero;

    public Vector3 Position = Vector3.Zero;
    public Vector3 Rotation = Vector3.Zero;

    public Matrix ViewMatrix;


    public Matrix ProjectionMatrix;

    public Camera() { }

    public void Update()
    {
        Direction = CalaculateDirection();

        ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.Pi / 3, Globals.GraphicsDevice.Viewport.AspectRatio, 0.1f, 100f);
        ViewMatrix = Matrix.CreateLookAt(Position, Position + Direction, Vector3.Up);
    }

    public Vector3 CalaculateDirection()
    {
        Matrix rotationMatrix = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(Rotation.Y), MathHelper.ToRadians(Rotation.X), MathHelper.ToRadians(Rotation.Z));
        return Vector3.Normalize(Vector3.Transform(Vector3.Forward, rotationMatrix));
    }
}
