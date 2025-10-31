using OpenTK.Mathematics;

namespace MidtermGame.Engine;

public class Camera
{
    public Vector3 Position;
    public float Yaw = -90f;
    public float Pitch = 0f;
    public float AspectRatio;

    public Camera(Vector3 position, float aspect)
    {
        Position = position;
        AspectRatio = aspect;
    }

    public Vector3 Front
    {
        get
        {
            var front = new Vector3(
                MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch)),
                MathF.Sin(MathHelper.DegreesToRadians(Pitch)),
                MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch))
            );
            return Vector3.Normalize(front);
        }
    }

    public Vector3 Right => Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
    public Vector3 Up    => Vector3.Normalize(Vector3.Cross(Right, Front));

    public Matrix4 GetViewMatrix() => Matrix4.LookAt(Position, Position + Front, Up);
}