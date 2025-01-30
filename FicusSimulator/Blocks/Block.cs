using FicusSimulator.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FicusSimulator.Blocks;

public enum BlockType
{
    Grass,
    Dirt,
    Stone
}

public class Block
{
    public Vector3 MinVertex { get; private set; }
    public Vector3 MaxVertex { get; private set; }

    public float X
    {
        get => Position.X;
        set => Position = new Vector3(value, Position.Y, Position.Z);
    }
    public float Y
    {
        get => Position.Y;
        set => Position = new Vector3(Position.X, value, Position.Z);
    }
    public float Z
    {
        get => Position.Z;
        set => Position = new Vector3(Position.X, Position.Y, value);
    }

    public Vector3 Position
    {
        get => position;
        set
        {
            position = value;
            WorldMatrix = Matrix.CreateTranslation(value * Size);
        }
    }

    public byte Size = 2;

    public Matrix WorldMatrix;
    private Vector3 position;

    private VertexPositionTexture[] vertices;
    private short[] indices;

    private Texture2D texture;
    private BasicEffect effect;

    private Rectangle[] faceSources = new Rectangle[6];

    private Rectangle frontFaceSource
    {
        get => faceSources[0];
        set => faceSources[0] = value;
    }
    private Rectangle backFaceSource
    {
        get => faceSources[1];
        set => faceSources[1] = value;
    }
    private Rectangle leftFaceSource
    {
        get => faceSources[2];
        set => faceSources[2] = value;
    }
    private Rectangle rightFaceSource
    {
        get => faceSources[3];
        set => faceSources[3] = value;
    }
    private Rectangle topFaceSource
    {
        get => faceSources[4];
        set => faceSources[4] = value;
    }
    private Rectangle bottomFaceSource
    {
        get => faceSources[5];
        set => faceSources[5] = value;
    }

    public Block()
    {
        LoadContent();

        frontFaceSource = backFaceSource = rightFaceSource = leftFaceSource = new Rectangle(0, 0, 16, 16);
        bottomFaceSource = new Rectangle(16, 0, 16, 16);
        topFaceSource = new Rectangle(32, 0, 16, 16);

        Position = position;

        InitializeCube();
    }

    private void LoadContent()
    {
        texture = Globals.Content.Load<Texture2D>("blocks");

        effect = new BasicEffect(Globals.GraphicsDevice)
        {
            FogEnabled = false,
            TextureEnabled = true,
            Texture = texture,
            LightingEnabled = false
        };
    }

    private void InitializeCube()
    {


        indices = new short[]
        {
            // Front
            0, 2, 1, 0, 3, 2,
            // Back
            4, 6, 5, 4, 7, 6,
            // Left
            8, 10, 9, 8, 11, 10,
            // Right
            12, 14, 13, 12, 15, 14,
            // Top
            16, 17, 18, 16, 18, 19,
            // Bottom
            20, 22, 21, 20, 23, 22
        };
    }

    public void CreateVertices()
    {
        vertices = new VertexPositionTexture[24];

        if (!GameManager.World.DoesBlockExist(position - new Vector3(0, 0, 1)))
            ApplyFaceUVs(0, frontFaceSource);
        if (!GameManager.World.DoesBlockExist(position + new Vector3(0, 0, 1)))
            ApplyFaceUVs(1, backFaceSource);
        if (!GameManager.World.DoesBlockExist(position - new Vector3(1, 0, 0)))
            ApplyFaceUVs(2, leftFaceSource);
        if (!GameManager.World.DoesBlockExist(position + new Vector3(1, 0, 0)))
            ApplyFaceUVs(3, rightFaceSource);
        if (!GameManager.World.DoesBlockExist(position + new Vector3(0, 1, 0)))
            ApplyFaceUVs(4, topFaceSource);
        if (!GameManager.World.DoesBlockExist(position - new Vector3(0, 1, 0)))
            ApplyFaceUVs(5, bottomFaceSource);

        CalculateSize();
    }

    public void Update()
    {

    }

    public void DrawCube(Matrix view, Matrix projection)
    {
        effect.World = WorldMatrix;
        effect.View = view;
        effect.Projection = projection;

        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            Globals.GraphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.TriangleList,
                vertices, 0, 24,
                indices, 0, 12
            );
        }
    }


    private Vector2 GetUVFromSourceRect(Rectangle sourceRect)
    {
        return new Vector2(
            (float)sourceRect.X / texture.Width,
            (float)sourceRect.Y / texture.Height
        );
    }

    private void ApplyFaceUVs(int faceIndex, Rectangle sourceRect)
    {
        Vector2 topLeft = GetUVFromSourceRect(sourceRect);
        Vector2 bottomRight = new Vector2(
            topLeft.X + (float)sourceRect.Width / texture.Width,
            topLeft.Y + (float)sourceRect.Height / texture.Height
        );

        switch (faceIndex)
        {
            case 0: // Front Face (Z = -1)
                vertices[0] = new VertexPositionTexture(new Vector3(-1, -1, -1), new Vector2(topLeft.X, bottomRight.Y)); // Bottom-left
                vertices[1] = new VertexPositionTexture(new Vector3(-1, 1, -1), new Vector2(topLeft.X, topLeft.Y));    // Top-left
                vertices[2] = new VertexPositionTexture(new Vector3(1, 1, -1), new Vector2(bottomRight.X, topLeft.Y)); // Top-right
                vertices[3] = new VertexPositionTexture(new Vector3(1, -1, -1), new Vector2(bottomRight.X, bottomRight.Y)); // Bottom-right
                break;

            case 1: // Back Face (Z = 1)
                vertices[4] = new VertexPositionTexture(new Vector3(1, -1, 1), new Vector2(topLeft.X, bottomRight.Y)); // Bottom-left
                vertices[5] = new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(topLeft.X, topLeft.Y));   // Top-left
                vertices[6] = new VertexPositionTexture(new Vector3(-1, 1, 1), new Vector2(bottomRight.X, topLeft.Y));  // Top-right
                vertices[7] = new VertexPositionTexture(new Vector3(-1, -1, 1), new Vector2(bottomRight.X, bottomRight.Y)); // Bottom-right
                break;

            case 2: // Left Face (X = -1)
                vertices[8] = new VertexPositionTexture(new Vector3(-1, -1, 1), new Vector2(bottomRight.X, bottomRight.Y));
                vertices[9] = new VertexPositionTexture(new Vector3(-1, 1, 1), new Vector2(bottomRight.X, topLeft.Y));
                vertices[10] = new VertexPositionTexture(new Vector3(-1, 1, -1), new Vector2(topLeft.X, topLeft.Y));
                vertices[11] = new VertexPositionTexture(new Vector3(-1, -1, -1), new Vector2(topLeft.X, bottomRight.Y));
                break;

            case 3: // Right Face (X = 1)
                vertices[12] = new VertexPositionTexture(new Vector3(1, -1, -1), new Vector2(bottomRight.X, bottomRight.Y));
                vertices[13] = new VertexPositionTexture(new Vector3(1, 1, -1), new Vector2(bottomRight.X, topLeft.Y));
                vertices[14] = new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(topLeft.X, topLeft.Y));
                vertices[15] = new VertexPositionTexture(new Vector3(1, -1, 1), new Vector2(topLeft.X, bottomRight.Y));
                break;

            case 4: // Top Face (Y = 1)
                vertices[16] = new VertexPositionTexture(new Vector3(-1, 1, -1), new Vector2(topLeft.X, bottomRight.Y));  // Bottom-left
                vertices[17] = new VertexPositionTexture(new Vector3(1, 1, -1), new Vector2(bottomRight.X, bottomRight.Y)); // Bottom-right
                vertices[18] = new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(bottomRight.X, topLeft.Y)); // Top-right
                vertices[19] = new VertexPositionTexture(new Vector3(-1, 1, 1), new Vector2(topLeft.X, topLeft.Y)); // Top-left
                break;

            case 5: // Bottom Face (Y = -1)
                vertices[20] = new VertexPositionTexture(new Vector3(-1, -1, -1), new Vector2(topLeft.X, topLeft.Y)); // Top-left
                vertices[21] = new VertexPositionTexture(new Vector3(1, -1, -1), new Vector2(bottomRight.X, topLeft.Y)); // Top-right
                vertices[22] = new VertexPositionTexture(new Vector3(1, -1, 1), new Vector2(bottomRight.X, bottomRight.Y)); // Bottom-right
                vertices[23] = new VertexPositionTexture(new Vector3(-1, -1, 1), new Vector2(topLeft.X, bottomRight.Y)); // Bottom-left
                break;
        }
    }


    public void CalculateSize()
    {
        // Find the min and max coordinates
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;
        float minZ = float.MaxValue, maxZ = float.MinValue;

        foreach (var vertex in vertices)
        {
            minX = Math.Min(minX, vertex.Position.X);
            maxX = Math.Max(maxX, vertex.Position.X);

            minY = Math.Min(minY, vertex.Position.Y);
            maxY = Math.Max(maxY, vertex.Position.Y);

            minZ = Math.Min(minZ, vertex.Position.Z);
            maxZ = Math.Max(maxZ, vertex.Position.Z);
        }

        // Calculate the width, height, and depth of the cube
        MinVertex = new Vector3(minX, minY, minZ);
        MaxVertex = new Vector3(maxX, maxY, maxZ);
    }


    public static Block CreateBlock(BlockType type)
    {
        return type switch
        {
            _ => new Block()
        };
    }
}
