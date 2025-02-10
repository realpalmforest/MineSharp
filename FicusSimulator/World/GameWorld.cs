using FicusSimulator.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace FicusSimulator.World;

public class GameWorld
{
    public short RenderedChunks;

    public Dictionary<Vector2, Chunk> Chunks = new Dictionary<Vector2, Chunk>();

    public bool ShowWireFrames = false;

    public Camera Camera;

    public GameWorld(Camera camera)
    {
        Camera = camera;
    }


    public void Draw()
    {
        RenderedChunks = 0;

        Globals.GraphicsDevice.RasterizerState = ShowWireFrames ? new RasterizerState() { FillMode = FillMode.WireFrame } : RasterizerState.CullCounterClockwise;
        Game1.Instance.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

        BoundingFrustum frustum = new BoundingFrustum(Camera.ViewMatrix * Camera.ProjectionMatrix);

        foreach (Chunk chunk in Chunks.Values)
        {
            if (chunk.Mesh.IsEmpty)
                continue;

            Vector3 chunkPosition = new Vector3((chunk.Position * new Vector2(Chunk.Width, Chunk.Depth)).X, 0, (chunk.Position * new Vector2(Chunk.Width, Chunk.Depth)).Y);
            BoundingBox box = new BoundingBox(chunkPosition, chunkPosition + new Vector3(Chunk.Width, Chunk.Height, Chunk.Depth));

            if (frustum.Contains(box) != ContainmentType.Disjoint)
            {
                BlockRenderer.DrawChunkMesh(chunk);
                RenderedChunks++;
            }
        }
    }

    public Block GetBlockAt(Vector3 position)
    {
        Vector3 blockPos = ToChunkPosition(position, out Vector2 chunkPosition);

        if (Chunks.TryGetValue(chunkPosition, out Chunk chunk))
        {
            return chunk.GetBlockAt(blockPos);
        }

        return new Block();
    }

    public void SetBlockAt(Vector3 position, Block block)
    {
        Vector3 blockPos = ToChunkPosition(position, out Vector2 chunkPosition);

        Chunks.TryAdd(chunkPosition, new Chunk(this, chunkPosition));
        Chunks.TryGetValue(chunkPosition, out Chunk chunk);

        chunk.SetBlockAt(blockPos, block);
    }

    public void FillBlocks(Vector3 startPosition, Vector3 endPosition, BlockType type)
    {
        List<Vector2> chunksToUpdate = new List<Vector2>();

        for (int x = (int)startPosition.X; x <= endPosition.X; x++)
        {
            for (int y = (int)startPosition.Y; y <= endPosition.Y; y++)
            {
                for (int z = (int)startPosition.Z; z <= endPosition.Z; z++)
                {
                    ToChunkPosition(new Vector3(x, y, z), out Vector2 chunkPos);

                    if (!chunksToUpdate.Contains(chunkPos))
                        chunksToUpdate.Add(chunkPos);

                    if(Chunks.GetValueOrDefault(chunkPos).IsEmpty)
                        Chunks.TryAdd(chunkPos, new Chunk(this, chunkPos));

                    Block block = new Block(type);
                    block.Position = new Vector3(x, y, z);
                    Chunks.GetValueOrDefault(chunkPos).Blocks[x, y, z] = block;
                }
            }
        }

        foreach (Vector2 pos in chunksToUpdate)
        {
            Chunk chunk = Chunks.GetValueOrDefault(pos);
            chunk.SetMesh(BlockRenderer.GenerateChunkMesh(chunk));
        }
    }

    /// <summary>
    /// Provides the block chunk position and position of chunk
    /// </summary>
    /// <param name="blockPosition">World position of block</param>
    /// <param name="chunkPosition">Outputs position of chunk</param>
    /// <returns>Block position in chunk</returns>
    public Vector3 ToChunkPosition(Vector3 blockPosition, out Vector2 chunkPosition)
    {
        float chunkX = MathF.Floor(blockPosition.X / Chunk.Width);
        float chunkY = MathF.Floor(blockPosition.Z / Chunk.Depth);

        float localX = blockPosition.X - (chunkX * Chunk.Width);
        float localZ = blockPosition.Z - (chunkY * Chunk.Depth);

        chunkPosition = new Vector2(chunkX, chunkY);
        return new Vector3(localX, blockPosition.Y, localZ);
    }

    public Vector2 GetPositionOfChunk(Vector3 blockPosition)
    {
        float chunkX = MathF.Floor(blockPosition.X / Chunk.Width);
        float chunkY = MathF.Floor(blockPosition.Z / Chunk.Depth);

        return new Vector2(chunkX, chunkY);
    }

    public void TryGenerateNeighborChunks(Chunk currentChunk, Vector3 blockPosition)
    {
        // Regenerate left chunk mesh
        if (Chunks.TryGetValue(new Vector2(currentChunk.Position.X - 1, currentChunk.Position.Y), out Chunk leftChunk))
        {
            if (blockPosition.X == 0 && !leftChunk.IsEmpty)
                leftChunk.SetMesh(BlockRenderer.GenerateChunkMesh(leftChunk));
        }
        // Regenerate right chunk mesh
        if (Chunks.TryGetValue(new Vector2(currentChunk.Position.X + 1, currentChunk.Position.Y), out Chunk rightChunk))
        {
            if (blockPosition.X == Chunk.Width - 1 && !rightChunk.IsEmpty)
                rightChunk.SetMesh(BlockRenderer.GenerateChunkMesh(rightChunk));
        }
        // Regenerate below chunk mesh
        if (Chunks.TryGetValue(new Vector2(currentChunk.Position.X, currentChunk.Position.Y - 1), out Chunk belowChunk))
        {
            if (blockPosition.Y == 0 && !belowChunk.IsEmpty)
                belowChunk.SetMesh(BlockRenderer.GenerateChunkMesh(belowChunk));
        }
        // Regenerate above chunk mesh
        if (Chunks.TryGetValue(new Vector2(currentChunk.Position.X, currentChunk.Position.Y + 1), out Chunk aboveChunk))
        {
            if (blockPosition.Y == Chunk.Depth - 1 && !aboveChunk.IsEmpty)
                aboveChunk.SetMesh(BlockRenderer.GenerateChunkMesh(aboveChunk));
        }
    }

    public List<Block> GetAllBlocks()
    {
        List<Block> blocks = new List<Block>();

        foreach (var chunk in Chunks.Values)
        {
            foreach (var block in chunk.Blocks)
            {
                if (block.Type != BlockType.Air)
                    blocks.Add(block);
            }
        }

        return blocks;
    }
}
