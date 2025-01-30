using FicusSimulator.Blocks;
using FicusSimulator.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FicusSimulator;

public class World
{
    public Dictionary<Vector3, Block> Blocks = new Dictionary<Vector3, Block>();

    public bool ShowWireFrames = false;

    private Camera camera;

    public World(Camera camera)
    {
        this.camera = camera;
    }

    public void Update()
    {
        Parallel.ForEach(Blocks, block =>
        {
            block.Value.Update();
        });
    }

    public void Draw()
    {
        Globals.GraphicsDevice.RasterizerState = ShowWireFrames ? new RasterizerState() { FillMode = FillMode.WireFrame } : RasterizerState.CullCounterClockwise;

        BoundingFrustum frustum = new BoundingFrustum(camera.ViewMatrix * camera.ProjectionMatrix);

        // Use depth test for visibility check
        Globals.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

        foreach (var block in Blocks.Values)
        {
            BoundingBox box = new BoundingBox(block.Position * block.Size + block.MinVertex, block.Position * block.Size + block.MaxVertex);

            if (frustum.Contains(box) != ContainmentType.Disjoint)
            {
                block.DrawCube(camera.ViewMatrix, camera.ProjectionMatrix);
            }
        }
    }

    public bool DoesBlockExist(Vector3 position)
    {
        return Blocks.ContainsKey(position);
    }

    public void FillBlocks(BlockType type, Vector3 startPosition, Vector3 endPosition)
    {
        for (int x = (int)startPosition.X; x <= endPosition.X; x++)
        {
            for (int y = (int)startPosition.Y; y <= endPosition.Y; y++)
            {
                for (int z = (int)startPosition.Z; z <= endPosition.Z; z++)
                {
                    if (!Blocks.ContainsKey(new Vector3(x, y, z)))
                    {
                        Block block = Block.CreateBlock(type);
                        block.Position = new Vector3(x, y, z);
                        Blocks.Add(block.Position, block);
                    }
                }
            }
        }

        foreach (var blck in Blocks)
            blck.Value.CreateVertices();
    }


    public void CreateBlock(BlockType type, Vector3 position) => CreateBlock(Block.CreateBlock(type), position);
    public void CreateBlock(Block block, Vector3 position)
    {
        if (Blocks.ContainsKey(position))
            return;

        block.Position = position;
        Blocks.Add(position, block);

        foreach (var blck in Blocks)
            blck.Value.CreateVertices();
    }

    public void DestroyBlock(Vector3 position)
    {
        Blocks.Remove(position);
    }
}
