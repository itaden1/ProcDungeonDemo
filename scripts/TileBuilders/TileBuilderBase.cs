using Godot;
using System.Collections.Generic;

namespace ProcDungeon.TileBuilders
{

    public class TileBuilderBase
    {
        protected int _tileSize;
        protected Dictionary<string, Mesh> _meshSet;

        public TileBuilderBase(int tileSize, Dictionary<string, Mesh> meshSet)
        {
            _tileSize = tileSize;
            _meshSet = meshSet;
        }
        // Build a tile
        public virtual void Build(SurfaceTool st, int x, int z){}
    }
}