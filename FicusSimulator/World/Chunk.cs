using Microsoft.Xna.Framework;

namespace FicusSimulator.World
{
    public struct Chunk
    {
        public Vector2 Position;
        public Block[,,] Blocks;

        public ChunkMesh Mesh;

        public GameWorld World;

        public Chunk(GameWorld world, Vector2 position)
        {
            Blocks = new Block[Width, Height, Depth];
            World = world;
            Position = position;
        }

        public Block GetBlockAt(Vector3 position)
        {
            if (position.X < 0 || position.Y < 0 || position.Z < 0 || position.X > Width - 1 || position.Y > Height - 1 || position.Z > Depth - 1)
                return Block.Air;

            if (IsEmpty)
                return Block.Air;

            return Blocks[(int)position.X, (int)position.Y, (int)position.Z];
        }

        public void SetBlockAt(Vector3 position, Block block)
        {
            if (position.X < 0 || position.Y < 0 || position.Z < 0 || position.X > Width - 1 || position.Y > Height - 1 || position.Z > Depth - 1)
                return;

            block.Position = new Vector3(Position.X * Width, 0, Position.Y * Depth) + position;
            Blocks[(int)position.X, (int)position.Y, (int)position.Z] = block;

            SetMesh(BlockRenderer.GenerateChunkMesh(this));
            World.TryGenerateNeighborChunks(this, position);
        }

        public void SetMesh(ChunkMesh mesh)
        {
            Mesh = mesh;
            World.Chunks.Remove(Position);
            World.Chunks.Add(Position, this);
        }


        public bool IsEmpty => Blocks == null;

        public static readonly byte Width = 16;
        public static readonly byte Depth = 16;
        public static readonly byte Height = 32;
    }
}
