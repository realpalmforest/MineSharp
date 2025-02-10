using Microsoft.Xna.Framework;

namespace FicusSimulator.World;

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

    public Block(BlockType type)
    {
        Type = type;
    }

    public static readonly byte Size = 2;
    public static readonly Block Air = new Block();
}
