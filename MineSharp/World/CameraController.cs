using MineSharp.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace MineSharp.World;

public class CameraController
{
    public Camera Camera;

    public float MoveSpeed = 1f;
    public float Sensitivity = 25f;

    private bool mouseLocked = true;
    private Vector2 windowCenter = Globals.VirtualGameSize.ToVector2() / 2;

    public CameraController(Camera camera)
    {
        Camera = camera;
        CenterMouse();
    }

    public void Update()
    {
        UpdateKeyboard();

        if (mouseLocked)
            UpdateMouse();
    }

    private void UpdateMouse()
    {
        Vector2 delta = Mouse.GetState().Position.ToVector2() - windowCenter;
        CenterMouse();

        Camera.Rotation.Y -= delta.X * Sensitivity / 100;
        Camera.Rotation.X -= delta.Y * Sensitivity / 100;

        Camera.Rotation.X = Math.Clamp(Camera.Rotation.X, -89f, 89f);
    }

    private void UpdateKeyboard()
    {
        if (InputManager.GetKeyDown(Keys.LeftControl))
        {
            mouseLocked = !mouseLocked;
            Game1.Instance.IsMouseVisible = !mouseLocked;

            if (mouseLocked)
                CenterMouse();
        }


        if (InputManager.Keyboard.IsKeyDown(Keys.LeftShift))
            Camera.Position.Y -= MoveSpeed / 100;
        if (InputManager.Keyboard.IsKeyDown(Keys.Space))
            Camera.Position.Y += MoveSpeed / 100;

        if (InputManager.Keyboard.IsKeyDown(Keys.S))
            Camera.Position -= Camera.Direction * MoveSpeed / 100;
        if (InputManager.Keyboard.IsKeyDown(Keys.W))
            Camera.Position += Camera.Direction * MoveSpeed / 100;

        if (InputManager.Keyboard.IsKeyDown(Keys.A))
        {
            float oldRotX = Camera.Rotation.X;
            Camera.Rotation.X = 0f;

            Camera.Rotation.Y += 90f;
            Camera.Position += Camera.CalaculateDirection() * MoveSpeed / 100;
            Camera.Rotation.Y -= 90f;

            Camera.Rotation.X = oldRotX;
        }

        if (InputManager.Keyboard.IsKeyDown(Keys.D))
        {
            float oldRotX = Camera.Rotation.X;
            Camera.Rotation.X = 0f;

            Camera.Rotation.Y -= 90f;
            Camera.Position += Camera.CalaculateDirection() * MoveSpeed / 100;
            Camera.Rotation.Y += 90f;

            Camera.Rotation.X = oldRotX;
        }
    }

    private void CenterMouse()
    {
        Mouse.SetPosition((int)windowCenter.X, (int)windowCenter.Y);
    }
}
