using System.Collections.Generic;
using Godot;

namespace ProcDungeon.TileBuilders
{
    public class TestTileBuilder : TileBuilderBase
    {
        public TestTileBuilder(int tileSize, Dictionary<string, Mesh> meshSet) : base(tileSize, meshSet)
        {
        }

        public override void Build(SurfaceTool st, int x, int z)
        {
            Mesh floor = _meshSet["floor"];
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
        }
        
    }
}