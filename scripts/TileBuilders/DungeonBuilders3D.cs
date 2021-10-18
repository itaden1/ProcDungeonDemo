using System.Collections.Generic;
using Godot;

namespace ProcDungeon.TileBuilders
{
    public class SouthWallBuilder3D: TileBuilderBase
    {
        public SouthWallBuilder3D(int tileSize, Dictionary<string, Mesh> meshSet) : base(tileSize, meshSet)
        {
        }

        public override void Build(SurfaceTool st, int x, int z)
        {        
            Mesh floor = _meshSet["floor"];
            Mesh wallEastWest = _meshSet["wallEastWest"];


            // Build a wall on the south side rest open
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, _tileSize * 2, z * _tileSize)));
            st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize + _tileSize)));
        }
    }

    public class NorthWallBuilder3D: TileBuilderBase
    {
        public NorthWallBuilder3D(int tileSize, Dictionary<string, Mesh> meshSet) : base(tileSize, meshSet)
        {
        }

        public override void Build(SurfaceTool st, int x, int z)
        {        
            Mesh floor = _meshSet["floor"];
            Mesh wallEastWest = _meshSet["wallEastWest"];

            // Build a wall on the north side rest open
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, _tileSize * 2, z * _tileSize)));
            st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
        }
    }
    public class WestWallBuilder3D: TileBuilderBase
    {
        public WestWallBuilder3D(int tileSize, Dictionary<string, Mesh> meshSet) : base(tileSize, meshSet)
        {
        }

        public override void Build(SurfaceTool st, int x, int z)
        {        
            Mesh floor = _meshSet["floor"];
            Mesh wallNorthSouth = _meshSet["wallNorthSouth"];

            // Build a wall on the west side rest open
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, _tileSize * 2, z * _tileSize)));
            st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
        }
    }
    public class EastWallBuilder3D: TileBuilderBase
    {
        public EastWallBuilder3D(int tileSize, Dictionary<string, Mesh> meshSet) : base(tileSize, meshSet)
        {
        }

        public override void Build(SurfaceTool st, int x, int z)
        {        
            Mesh floor = _meshSet["floor"];
            Mesh wallNorthSouth = _meshSet["wallNorthSouth"];

            // Build a wall on the east side rest open
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, _tileSize * 2, z * _tileSize)));
            st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize + _tileSize, 0, z * _tileSize)));
        }
    }

    public class CornerNorthWestBuilder3D: TileBuilderBase
    {
        public CornerNorthWestBuilder3D(int tileSize, Dictionary<string, Mesh> meshSet) : base(tileSize, meshSet)
        {
        }

        public override void Build(SurfaceTool st, int x, int z)
        {        
            Mesh floor = _meshSet["floor"];
            Mesh wallNorthSouth = _meshSet["wallNorthSouth"];
            Mesh wallEastWest = _meshSet["wallEastWest"];

            // Build a corner on the north west
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, _tileSize * 2, z * _tileSize)));
            st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
            st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
        }
    }
    public class CornerNorthEastBuilder3D: TileBuilderBase
    {
        public CornerNorthEastBuilder3D(int tileSize, Dictionary<string, Mesh> meshSet) : base(tileSize, meshSet)
        {
        }

        public override void Build(SurfaceTool st, int x, int z)
        {        
            Mesh floor = _meshSet["floor"];
            Mesh wallNorthSouth = _meshSet["wallNorthSouth"];
            Mesh wallEastWest = _meshSet["wallEastWest"];

            // Build a corner on the north east
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, _tileSize * 2, z * _tileSize)));
            st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
            st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize + _tileSize, 0, z * _tileSize)));
        }
    }
    public class CornerSouthWestBuilder3D: TileBuilderBase
    {
        public CornerSouthWestBuilder3D(int tileSize, Dictionary<string, Mesh> meshSet) : base(tileSize, meshSet)
        {
        }

        public override void Build(SurfaceTool st, int x, int z)
        {        
            Mesh floor = _meshSet["floor"];
            Mesh wallNorthSouth = _meshSet["wallNorthSouth"];
            Mesh wallEastWest = _meshSet["wallEastWest"];

            // Build a corner on the south west
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, _tileSize * 2, z * _tileSize)));
            st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize + _tileSize)));
            st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
        }
    }
    public class CornerSouthEastBuilder3D: TileBuilderBase
    {
        public CornerSouthEastBuilder3D(int tileSize, Dictionary<string, Mesh> meshSet) : base(tileSize, meshSet)
        {
        }

        public override void Build(SurfaceTool st, int x, int z)
        {        
            Mesh floor = _meshSet["floor"];
            Mesh wallNorthSouth = _meshSet["wallNorthSouth"];
            Mesh wallEastWest = _meshSet["wallEastWest"];

        // Build a corner peice on south east corner
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, _tileSize * 2, z * _tileSize)));
            st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize + _tileSize)));
            st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize + _tileSize, 0, z * _tileSize)));
        }
    }
    public class OpenAreaBuilder3D: TileBuilderBase
    {
        public OpenAreaBuilder3D(int tileSize, Dictionary<string, Mesh> meshSet) : base(tileSize, meshSet)
        {
        }

        public override void Build(SurfaceTool st, int x, int z)
        {        
            Mesh floor = _meshSet["floor"];
            Mesh wallNorthSouth = _meshSet["wallNorthSouth"];
            Mesh wallEastWest = _meshSet["wallEastWest"];

            // Build an apen area with no walls
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, _tileSize * 2, z * _tileSize)));
        }
    }
    public class CorridoorEastWestBuilder: TileBuilderBase
    {
        public CorridoorEastWestBuilder(int tileSize, Dictionary<string, Mesh> meshSet) : base(tileSize, meshSet)
        {
        }

        public override void Build(SurfaceTool st, int x, int z)
        {        
            Mesh floor = _meshSet["floor"];
            Mesh wallEastWest = _meshSet["wallEastWest"];
            Mesh corridoorRoof = _meshSet["corridoorRoofEastWest"];

            // Build a corridoor spanning east to west
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
            st.AppendFrom(corridoorRoof, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, _tileSize, z * _tileSize)));
            st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
            st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize + _tileSize)));
        }
    }
    public class CorridoorNorthSouthBuilder: TileBuilderBase
    {
        public CorridoorNorthSouthBuilder(int tileSize, Dictionary<string, Mesh> meshSet) : base(tileSize, meshSet)
        {
        }

        public override void Build(SurfaceTool st, int x, int z)
        {        
            Mesh floor = _meshSet["floor"];
            Mesh wallNorthSouth = _meshSet["wallNorthSouth"];
            Mesh corridoorRoof = _meshSet["corridoorRoofNorthSouth"];

            // Build a corridoor spanning north to south
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
            st.AppendFrom(corridoorRoof, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, _tileSize, z * _tileSize)));
            st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
            st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize + _tileSize, 0, z * _tileSize)));
        }
    }
    public class CorridoorCornerSouthEastBuilder: TileBuilderBase
    {
        public CorridoorCornerSouthEastBuilder(int tileSize, Dictionary<string, Mesh> meshSet) : base(tileSize, meshSet)
        {
        }

        public override void Build(SurfaceTool st, int x, int z)
        {        
            Mesh floor = _meshSet["floor"];
            Mesh wallNorthSouth = _meshSet["wallNorthSouth"];
            Mesh wallEastWest = _meshSet["wallEastWest"];

            // Build a corridoor corridoor on the to south east corner
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, _tileSize, z * _tileSize)));
            st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize + _tileSize, 0, z * _tileSize)));
            st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize  + _tileSize)));
        }
    }
    public class CorridoorCornerSouthWestBuilder: TileBuilderBase
    {
        public CorridoorCornerSouthWestBuilder(int tileSize, Dictionary<string, Mesh> meshSet) : base(tileSize, meshSet)
        {
        }

        public override void Build(SurfaceTool st, int x, int z)
        {        
            Mesh floor = _meshSet["floor"];
            Mesh wallNorthSouth = _meshSet["wallNorthSouth"];
            Mesh wallEastWest = _meshSet["wallEastWest"];

            // Build a corridoor corridoor on the south west corner
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, _tileSize, z * _tileSize)));
            st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
            st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize  + _tileSize)));
        }
    }
    public class CorridoorCornerNorthWestBuilder: TileBuilderBase
    {
        public CorridoorCornerNorthWestBuilder(int tileSize, Dictionary<string, Mesh> meshSet) : base(tileSize, meshSet)
        {
        }

        public override void Build(SurfaceTool st, int x, int z)
        {        
            Mesh floor = _meshSet["floor"];
            Mesh wallNorthSouth = _meshSet["wallNorthSouth"];
            Mesh wallEastWest = _meshSet["wallEastWest"];

            // Build a corridoor corridoor on the north west corner
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, _tileSize, z * _tileSize)));
            st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
            st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
        }
    }
    public class CorridoorCornerNorthEastBuilder: TileBuilderBase
    {
        public CorridoorCornerNorthEastBuilder(int tileSize, Dictionary<string, Mesh> meshSet) : base(tileSize, meshSet)
        {
        }

        public override void Build(SurfaceTool st, int x, int z)
        {        
            Mesh floor = _meshSet["floor"];
            Mesh wallNorthSouth = _meshSet["wallNorthSouth"];
            Mesh wallEastWest = _meshSet["wallEastWest"];

            // Build a corridoor corridoor on the north east corner
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
            st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, _tileSize, z * _tileSize)));
            st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize + _tileSize, 0, z * _tileSize)));
            st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * _tileSize, 0, z * _tileSize)));
        }
    }
}