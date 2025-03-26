using Microsoft.Xna.Framework;

namespace MineSharp.World;

public enum BlockType
{
    Air,
    Grass,
    Dirt,
    Stone
}

public enum FaceDirection
{
    Front,
    Back,
    Left,
    Right,
    Top,
    Bottom
}

public struct Block
{
    public Vector3 Position;
    public BlockType Type = BlockType.Air;

    public Block(Vector3 position, BlockType type)
    {
        Position = position;
        Type = type;
    }

    public static readonly byte Size = 2;
    public static readonly Block Air = new Block();
}
