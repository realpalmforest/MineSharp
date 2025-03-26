using MineSharp.World;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MineSharp.Core;

public static class GameManager
{
    private static int frameCount = 0;
    private static double elapsedTime = 0;
    private static int fps = 0;

    private static Camera camera;
    private static CameraController controller;

    public static GameWorld World;

    private static System.Numerics.Vector3 newBlockPos = System.Numerics.Vector3.Zero;

    public static void Load()
    {
        camera = new Camera();
        controller = new CameraController(camera);

        World = new GameWorld(camera);

        World.FillBlocks(Vector3.Zero, new Vector3(15, 31, 15), BlockType.Stone);
        // World.SetBlockAt(new Vector3(0, 0, 0), new Block(BlockType.Grass));
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
        ImGui.Begin("Info", ImGuiWindowFlags.NoMove);

        ImGui.Text($"FPS: {fps}");

        ImGui.Separator();

        if (ImGui.CollapsingHeader("Renderer", ImGuiTreeNodeFlags.DefaultOpen))
        {
            ImGui.Checkbox("Wireframes", ref World.ShowWireFrames);
        }

        if (ImGui.CollapsingHeader("Camera", ImGuiTreeNodeFlags.DefaultOpen))
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

        ImGui.Separator();

        if (ImGui.CollapsingHeader("World", ImGuiTreeNodeFlags.DefaultOpen))
        {
            ImGui.Text("Chunk Count: " + World.Chunks.Count);
            ImGui.Text("Rendered Chunks: " + World.RenderedChunks);

            Vector2 chunkPos = World.GetPositionOfChunk(camera.Position);
            ImGui.Text("Current Chunk: " + (World.Chunks.ContainsKey(chunkPos) ? chunkPos : "None"));
            ImGui.Separator();

            ImGui.InputFloat3("World Position", ref newBlockPos);
            if (ImGui.Button("Create Block"))
            {
                World.SetBlockAt(newBlockPos, BlockType.Grass);
                newBlockPos = System.Numerics.Vector3.Zero;
            }

            ImGui.SameLine();

            if (ImGui.Button("Destroy Block"))
            {
                World.SetBlockAt(newBlockPos, BlockType.Air);
                newBlockPos = System.Numerics.Vector3.Zero;
            }


            ImGui.Separator();
            if (ImGui.CollapsingHeader("Blocks"))
            {
                foreach (var block in World.GetAllBlocks())
                {
                    ImGui.Text(block.Position + " | " + block.Type);
                }
            }
        }

        if (ImGui.CollapsingHeader("Controls", ImGuiTreeNodeFlags.DefaultOpen))
        {
            ImGui.Text("Toggle Camera Mode: [Left Ctrl]");
        }

        ImGui.End();
    }
}