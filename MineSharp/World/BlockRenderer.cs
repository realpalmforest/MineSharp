using MineSharp.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MineSharp.World;

public static class BlockRenderer
{
    private static List<VertexPositionTexture> vertices = new();
    private static List<short> indices = new();

    private static Texture2D blocksTexture = Globals.Content.Load<Texture2D>("blocks");

    private static BasicEffect effect = new BasicEffect(Game1.Instance.GraphicsDevice)
    {
        FogEnabled = false,
        TextureEnabled = true,
        Texture = blocksTexture,
        LightingEnabled = false
    };


    public static ChunkMesh GenerateChunkMesh(Chunk chunk)
    {
        vertices.Clear();
        indices.Clear();

        foreach (Block block in chunk.Blocks)
        {
            if (block.Type == BlockType.Air)
                continue;

            AddBlockVertices(chunk, block);
        }

        return new ChunkMesh(vertices, indices);
    }

    public static void AddBlockVertices(Chunk chunk, Block block)
    {
        if (block.Type == BlockType.Air)
            return;

        // Create block faces if there is no block in that direction
        if (chunk.World.GetBlockAt(block.Position - new Vector3(0, 0, 1)).Type == BlockType.Air)
            AddFaceVertices(block.Position, FaceDirection.Front, block.Type, (short)vertices.Count);
        if (chunk.World.GetBlockAt(block.Position + new Vector3(0, 0, 1)).Type == BlockType.Air)
            AddFaceVertices(block.Position, FaceDirection.Back, block.Type, (short)vertices.Count);
        if (chunk.World.GetBlockAt(block.Position - new Vector3(1, 0, 0)).Type == BlockType.Air)
            AddFaceVertices(block.Position, FaceDirection.Left, block.Type, (short)vertices.Count);
        if (chunk.World.GetBlockAt(block.Position + new Vector3(1, 0, 0)).Type == BlockType.Air)
            AddFaceVertices(block.Position, FaceDirection.Right, block.Type, (short)vertices.Count);
        if (chunk.World.GetBlockAt(block.Position + new Vector3(0, 1, 0)).Type == BlockType.Air)
            AddFaceVertices(block.Position, FaceDirection.Top, block.Type, (short)vertices.Count);
        if (chunk.World.GetBlockAt(block.Position - new Vector3(0, 1, 0)).Type == BlockType.Air)
            AddFaceVertices(block.Position, FaceDirection.Bottom, block.Type, (short)vertices.Count);
    }

    public static void AddFaceVertices(Vector3 pos, FaceDirection face, BlockType type, short baseIndex)
    {
        Vector3[] faceOffsets = GetFaceOffsets(face);
        Vector2[] uvs = GetFaceUVs(face, type);

        for (int i = 0; i < 4; i++)
        {
            vertices.Add(new VertexPositionTexture(pos + faceOffsets[i], uvs[i]));
        }

        indices.AddRange(new short[] {
            baseIndex, (short)(baseIndex + 1), (short)(baseIndex + 2),
            baseIndex, (short)(baseIndex + 2), (short)(baseIndex + 3)
        });
    }

    public static void DrawChunkMesh(Chunk chunk)
    {
        if (chunk.Mesh.IsEmpty)
            return;

        Game1.Instance.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

        effect.View = chunk.World.Camera.ViewMatrix;
        effect.Projection = chunk.World.Camera.ProjectionMatrix;
        //effect.World = Matrix.CreateTranslation(new Vector3(chunk.Position.X, 0, chunk.Position.Y) * new Vector3(Chunk.Width, 0, Chunk.Depth));
        effect.World = Matrix.Identity;

        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            Game1.Instance.GraphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.TriangleList,
                chunk.Mesh.Vertices.ToArray(), 0, chunk.Mesh.Vertices.Count,
                chunk.Mesh.Indices.ToArray(), 0, chunk.Mesh.Indices.Count / 3
            );
        }
    }


    private static Vector3[] GetFaceOffsets(FaceDirection face)
    {
        return face switch
        {
            FaceDirection.Front => new Vector3[]
            {
            new(0.5f, -0.5f, -0.5f),
            new(0.5f, 0.5f, -0.5f),
            new(-0.5f, 0.5f, -0.5f),
            new(-0.5f, -0.5f, -0.5f)
            },
            FaceDirection.Back => new Vector3[]
            {
            new(-0.5f, -0.5f, 0.5f),
            new(-0.5f, 0.5f, 0.5f),
            new(0.5f, 0.5f, 0.5f),
            new(0.5f, -0.5f, 0.5f)

            },
            FaceDirection.Left => new Vector3[]
            {
            new(-0.5f, -0.5f, -0.5f),
            new(-0.5f, 0.5f, -0.5f),
            new(-0.5f, 0.5f, 0.5f),
            new(-0.5f, -0.5f, 0.5f)

            },
            FaceDirection.Right => new Vector3[]
            {
            new(0.5f, -0.5f, 0.5f),
            new(0.5f, 0.5f, 0.5f),
            new(0.5f, 0.5f, -0.5f),
            new(0.5f, -0.5f, -0.5f)

            },
            FaceDirection.Top => new Vector3[]
            {
            new(-0.5f, 0.5f, -0.5f),
            new(0.5f, 0.5f, -0.5f),
            new(0.5f, 0.5f, 0.5f),
            new(-0.5f, 0.5f, 0.5f)
            },
            FaceDirection.Bottom => new Vector3[]
            {
            new(-0.5f, -0.5f, -0.5f),
            new(-0.5f, -0.5f, 0.5f),
            new(0.5f, -0.5f, 0.5f),
            new(0.5f, -0.5f, -0.5f)
            },
            _ => throw new ArgumentOutOfRangeException(nameof(face), "Invalid face direction")
        };
    }

    private static Vector2[] GetFaceUVs(FaceDirection face, BlockType type)
    {
        if (!BlockTextures.TryGetValue((type, face), out Rectangle sourceRect))
        {
            throw new Exception($"No texture mapping found for {type} {face}");
        }

        // Normalize UV coordinates (convert from pixel to 0-1 range)
        Vector2 topLeft = new((float)sourceRect.X / blocksTexture.Width, (float)sourceRect.Y / blocksTexture.Height);
        Vector2 topRight = new((float)(sourceRect.X + sourceRect.Width) / blocksTexture.Width, (float)sourceRect.Y / blocksTexture.Height);
        Vector2 bottomRight = new((float)(sourceRect.X + sourceRect.Width) / blocksTexture.Width, (float)(sourceRect.Y + sourceRect.Height) / blocksTexture.Height);
        Vector2 bottomLeft = new((float)sourceRect.X / blocksTexture.Width, (float)(sourceRect.Y + sourceRect.Height) / blocksTexture.Height);

        return new Vector2[] { bottomLeft, topLeft, topRight, bottomRight };
    }


    private static readonly Dictionary<(BlockType, FaceDirection), Rectangle> BlockTextures = new()
    {
        // Grass Block
        { (BlockType.Grass, FaceDirection.Top), new Rectangle(32, 0, 16, 16) },
        { (BlockType.Grass, FaceDirection.Bottom), new Rectangle(16, 0, 16, 16) },
        { (BlockType.Grass, FaceDirection.Front), new Rectangle(0, 0, 16, 16) },
        { (BlockType.Grass, FaceDirection.Back), new Rectangle(0, 0, 16, 16) },
        { (BlockType.Grass, FaceDirection.Left), new Rectangle(0, 0, 16, 16) },
        { (BlockType.Grass, FaceDirection.Right), new Rectangle(0, 0, 16, 16) },

        // Dirt Block
        { (BlockType.Dirt, FaceDirection.Top), new Rectangle(16, 0, 16, 16) },
        { (BlockType.Dirt, FaceDirection.Bottom), new Rectangle(16, 0, 16, 16) },
        { (BlockType.Dirt, FaceDirection.Front), new Rectangle(16, 0, 16, 16) },
        { (BlockType.Dirt, FaceDirection.Back), new Rectangle(16, 0, 16, 16) },
        { (BlockType.Dirt, FaceDirection.Left), new Rectangle(16, 0, 16, 16) },
        { (BlockType.Dirt, FaceDirection.Right), new Rectangle(16, 0, 16, 16) },

        // Stone Block
        { (BlockType.Stone, FaceDirection.Top), new Rectangle(48, 0, 16, 16) },
        { (BlockType.Stone, FaceDirection.Bottom), new Rectangle(48, 0, 16, 16) },
        { (BlockType.Stone, FaceDirection.Front), new Rectangle(48, 0, 16, 16) },
        { (BlockType.Stone, FaceDirection.Back), new Rectangle(48, 0, 16, 16) },
        { (BlockType.Stone, FaceDirection.Left), new Rectangle(48, 0, 16, 16) },
        { (BlockType.Stone, FaceDirection.Right), new Rectangle(48, 0, 16, 16) },
    };
}