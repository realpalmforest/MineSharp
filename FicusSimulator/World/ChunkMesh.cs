using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FicusSimulator.World;

public struct ChunkMesh
{
    public List<VertexPositionTexture> Vertices;
    public List<short> Indices;

    public ChunkMesh(List<VertexPositionTexture> vertices, List<short> indices)
    {
        Vertices = vertices;
        Indices = indices;
    }

    public bool IsEmpty => Vertices is null || Indices is null || Vertices.Count == 0 || Indices.Count == 0;
}
