using FicusSimulator.Blocks;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FicusSimulator.Core;

public static class GameManager
{
    private static int frameCount = 0;
    private static double elapsedTime = 0;
    private static int fps = 0;


    private static Camera camera;
    private static CameraController controller;

    public static World World;

    private static System.Numerics.Vector3 newBlockPos = System.Numerics.Vector3.Zero;

    public static void Load()
    {
        camera = new Camera();
        controller = new CameraController(camera);

        World = new World(camera);


        World.FillBlocks(BlockType.Grass, Vector3.Zero, new Vector3(20, 20, 20));
    }

    public static void Update()
    {
        elapsedTime += Globals.GameTime.ElapsedGameTime.TotalSeconds;
        frameCount++;

        if (elapsedTime >= 1.0) // Update every second
        {
            fps = frameCount;
            frameCount = 0;
            elapsedTime = 0;
        }


        controller.Update();
        camera.Update();

        World.Update();
    }

    public static void Draw()
    {
        Globals.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        Globals.SpriteBatch.Begin(blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointClamp);

        World.Draw();

        Globals.SpriteBatch.End();
    }

    public static void DrawImGui()
    {
        ImGui.Begin("Options", ImGuiWindowFlags.None);

        ImGui.Text($"FPS: {fps}");
        ImGui.Checkbox("Wireframes", ref World.ShowWireFrames);
        ImGui.Separator();

        if (ImGui.CollapsingHeader("Camera"))
        {
            System.Numerics.Vector3 cameraPosNumerics = camera.Position.ToNumerics();
            ImGui.InputFloat3("Position", ref cameraPosNumerics);
            camera.Position = cameraPosNumerics;

            System.Numerics.Vector3 cameraRotNumerics = camera.Rotation.ToNumerics();
            ImGui.InputFloat3("Rotation", ref cameraRotNumerics);
            camera.Rotation = cameraRotNumerics;

            ImGui.DragFloat("MoveSpeed", ref controller.MoveSpeed);
            ImGui.DragFloat("Sensitivity", ref controller.Sensitivity);
        }

        if (ImGui.CollapsingHeader("World"))
        {
            ImGui.Text("Block Count: " + World.Blocks.Count);
            ImGui.Separator();

            ImGui.InputFloat3("Block Position", ref newBlockPos);
            if (ImGui.Button("Create Block"))
            {
                World.CreateBlock(new Block(), newBlockPos);
                newBlockPos = System.Numerics.Vector3.Zero;
            }

            ImGui.SameLine();

            if (ImGui.Button("Destroy Block"))
            {
                World.DestroyBlock(newBlockPos);
                newBlockPos = System.Numerics.Vector3.Zero;
            }
        }

        ImGui.End();
    }
}