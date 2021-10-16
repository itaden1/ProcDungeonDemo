using Godot;
using System;
using System.Collections.Generic;

using ProcDungeon.Structures;
using ProcDungeon.Algorythms;
using ProcDungeon;

using ProcDungeon.TileBuilders;


public struct UVCoordSet
{
    public float x1;
    public float x2;
    public float y1;
    public float y2;
}
public class DungeonGeneration3D : Spatial
{
    [Export]
    public int TileSize = 2;
 
    private Mesh pillar;
    private Mesh floor;
    private Mesh wallEastWest;
    private Mesh wallNorthSouth;
    private Mesh skirtingNorthSouth;
    private Mesh skirtingEastWest;

    private bool _playerSpawned = false;
    public override void _Ready()
    {
        GD.Print("starting");
        GetParent().GetNode("GUI").Connect("RegenerateDungeon", this, nameof(_onRegenerateButtonPressed));
        pillar = (Mesh)ResourceLoader.Load<Mesh>("res://3DPrefabs/pillar.tres");
        floor = (Mesh)ResourceLoader.Load<Mesh>("res://3DPrefabs/floor.tres");
        wallEastWest = (Mesh)ResourceLoader.Load<Mesh>("res://3DPrefabs/wallEW.tres");
        wallNorthSouth = (Mesh)ResourceLoader.Load<Mesh>("res://3DPrefabs/wallNS.tres");
        skirtingNorthSouth = (Mesh)ResourceLoader.Load<Mesh>("res://3DPrefabs/skirtingNS.tres");
        skirtingEastWest = (Mesh)ResourceLoader.Load<Mesh>("res://3DPrefabs/skirtingEW.tres");
		// Generate();
    }

    public void Generate(DungeonGrid myMap)
    {
        // GD.Print("generating");
		// var alg = new BSPDungeonAlgorythm(_roomCount);
		// var myMap = new DungeonGrid(_mapSize, alg);
		// myMap.GenerateLayout();



        Dictionary<string, Mesh> meshSet = new Dictionary<string, Mesh>
        {
            {"pillar", (Mesh)ResourceLoader.Load<Mesh>("res://3DPrefabs/pillar.tres")},
            {"floor", (Mesh)ResourceLoader.Load<Mesh>("res://3DPrefabs/floor.tres")},
            {"wallEastWest", (Mesh)ResourceLoader.Load<Mesh>("res://3DPrefabs/wallEastWest.tres")},
            {"wallNorthSouth", (Mesh)ResourceLoader.Load<Mesh>("res://3DPrefabs/wallNorthSouth.tres")},
            {"skirtingNorthSouth", (Mesh)ResourceLoader.Load<Mesh>("res://3DPrefabs/skirtingNorthSouth.tres")},
            {"skirtingEastWest", (Mesh)ResourceLoader.Load<Mesh>("res://3DPrefabs/skirtingEastWest.tres")},
        };


        Dictionary<int, TileBuilderBase> tileSet = new Dictionary<int, TileBuilderBase>
        {
            { 0, new TestTileBuilder(TileSize, meshSet) },
            { 2, new TestTileBuilder(TileSize, meshSet) },
            { 8, new TestTileBuilder(TileSize, meshSet) },
            { 10, new TestTileBuilder(TileSize, meshSet) },
            { 11, new TestTileBuilder(TileSize, meshSet) },
            { 16, new TestTileBuilder(TileSize, meshSet) },
            { 18, new TestTileBuilder(TileSize, meshSet) },
            { 22, new TestTileBuilder(TileSize, meshSet) },
            { 24, new TestTileBuilder(TileSize, meshSet) },
            { 26, new TestTileBuilder(TileSize, meshSet) },
            { 27, new TestTileBuilder(TileSize, meshSet) },
            { 30, new TestTileBuilder(TileSize, meshSet) },
            { 31, new SouthWallBuilder3D(TileSize, meshSet) },
            { 64, new TestTileBuilder(TileSize, meshSet) },
            { 66, new TestTileBuilder(TileSize, meshSet) },
            { 72, new TestTileBuilder(TileSize, meshSet) },
            { 74, new TestTileBuilder(TileSize, meshSet) },
            { 75, new TestTileBuilder(TileSize, meshSet) },
            { 80, new TestTileBuilder(TileSize, meshSet) },
            { 82, new TestTileBuilder(TileSize, meshSet) },
            { 86, new TestTileBuilder(TileSize, meshSet) },
            { 88, new TestTileBuilder(TileSize, meshSet) },
            { 90, new TestTileBuilder(TileSize, meshSet) },
            { 91, new TestTileBuilder(TileSize, meshSet) },
            { 94, new TestTileBuilder(TileSize, meshSet) },
            { 95, new TestTileBuilder(TileSize, meshSet) },
            { 104, new TestTileBuilder(TileSize, meshSet) },
            { 106, new TestTileBuilder(TileSize, meshSet) },
            { 107, new EastWallBuilder3D(TileSize, meshSet) },
            { 120, new TestTileBuilder(TileSize, meshSet) },
            { 122, new TestTileBuilder(TileSize, meshSet) },
            { 123, new TestTileBuilder(TileSize, meshSet) },
            { 126, new TestTileBuilder(TileSize, meshSet) },
            { 127, new TestTileBuilder(TileSize, meshSet) },
            { 208, new CornerNorthWestBuilder3D(TileSize, meshSet) },
            { 210, new TestTileBuilder(TileSize, meshSet) },
            { 214, new WestWallBuilder3D(TileSize, meshSet) },
            { 216, new TestTileBuilder(TileSize, meshSet) },
            { 218, new TestTileBuilder(TileSize, meshSet) },
            { 219, new TestTileBuilder(TileSize, meshSet) },
            { 222, new TestTileBuilder(TileSize, meshSet) },
            { 223, new TestTileBuilder(TileSize, meshSet) },
            { 248, new NorthWallBuilder3D(TileSize, meshSet) },
            { 250, new TestTileBuilder(TileSize, meshSet) },
            { 251, new TestTileBuilder(TileSize, meshSet) },
            { 254, new TestTileBuilder(TileSize, meshSet) },
            { 255, new TestTileBuilder(TileSize, meshSet) }
        };

        SurfaceTool st = new SurfaceTool();
        st.Begin(Mesh.PrimitiveType.Triangles);

        // y coord becomes the z coord in 3D space
        for (int z = 0; z < myMap.Grid.GetLength(0); z++)
        {
            for (int x = 0; x < myMap.Grid.GetLength(1); x++)
            {
                if (myMap.Grid[z, x].Blocking == false)
                {
                    int mask = getFourBitMask(myMap.Grid, x, z);
                    // GD.Print(mask);
                    if (tileSet.ContainsKey(mask))
                    {
                        TileBuilderBase tb = tileSet[mask];
                        tb.Build(st, x, z);
                    }
                    else GD.Print($"Missing key: {mask}");


                    // switch(mask)
                    // {
                    //     case 2: // open all sides
                    //         buildTallOpen(st, x, z);
                    //         break;
                    //     case 8: // t2
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 10: // t3
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 11: // south east corner
                    //         buildCornerSouthEast(st, x, z);
                    //         break;
                    //     case 16: //t5
                    //         break;
                    //     case 18: // t6
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 22: // south west corner
                    //         buildCornerSouthWest(st, x, z);
                    //         break;
                    //     case 24: // t8
                    //         break;
                    //     case 26: // t9
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 27: //t10
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 30: // t11
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 31: // south wall
                    //         // buildWallSouth(st, x, z);
                    //         SouthWallBuilder3D b = new SouthWallBuilder3D(TileSize);
                    //         b.Build(st, x, z);
                    //         break;
                    //     case 64: // t13
                    //         break;
                    //     case 66: // t14
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 72: // t15
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 74: //t16
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 75: // t17
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 80: //t18
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 82: //t19
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 86: // t20
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 88: // t21
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 90: //t22
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 91: //t23
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 94: //t24
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 95: //t25
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 104: // north east corner
                    //         buildCornerNorthEast(st, x, z);
                    //         break;
                    //     case 106: //t27
                    //         break;
                    //     case 107: // west wall
                    //         buildWallEast( st, x, z);
                    //         break;
                    //     case 120: //t29
                    //         break;
                    //     case 122: // 30
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 123: //t31
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 126: //t32
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 127: //t33
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 208: // north west corner
                    //         buildCornerNorthWest(st, x, z);
                    //         break;
                    //     case 210: //t35
                    //         break;
                    //     case 214: //t36
                    //         buildWallWest(st, x, z);
                    //         break;
                    //     case 216: //t37
                    //         break;
                    //     case 218: //t38
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 219: //t39
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 222: //t40
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 223: //t41
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 248: // north wall
                    //         buildWallNorth(st, x, z);
                    //         break;
                    //     case 250: //t43
                    //         break;
                    //     case 251: //t44
                    //         buildTest(st, x, z);
                    //         break;
                    //     case 254: //t45
                    //         // buildTallOpen(st, x, z);
                    //         break;
                    //     case 255: // open all sides
                    //         buildTallOpen(st, x, z);
                    //         break;
                    //     case 0: //t47
                    //         buildTest(st, x, z);
                    //         break;
                    // }
                    // level 1 walls
                    // if (myMap.Grid[z, x-1].Blocking)
                    // {
                    //     st.AppendFrom(pillar, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize + TileSize)));
                    //     st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
                    //     st.AppendFrom(skirtingNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
                    // }
                    // if (myMap.Grid[z-1, x].Blocking)
                    // {
                    //     st.AppendFrom(pillar, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
                    //     st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
                    //     st.AppendFrom(skirtingEastWest, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
                    // }
                    // if (myMap.Grid[z, x+1].Blocking)
                    // {
                    //     st.AppendFrom(pillar, 0, new Transform(Basis.Identity, new Vector3(x * TileSize + TileSize, 0, z * TileSize)));
                    //     st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * TileSize + TileSize, 0, z * TileSize)));
                    //     st.AppendFrom(skirtingNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * TileSize + TileSize, 0, z * TileSize)));
                    // }
                    // if (myMap.Grid[z+1, x].Blocking)
                    // {
                    //     st.AppendFrom(pillar, 0, new Transform(Basis.Identity, new Vector3(x * TileSize + TileSize, 0, z * TileSize + TileSize)));
                    //     st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize + TileSize)));
                    //     st.AppendFrom(skirtingEastWest, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize + TileSize)));
                    // }

                    // st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
                    // st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, TileSize, z * TileSize)));

                    
                    if(!_playerSpawned){
                        SpawnPlayer(x, z);
                    }
                }
            }
        }

        st.Index();

        Mesh mesh = st.Commit();
        SpatialMaterial material = (SpatialMaterial)ResourceLoader.Load<SpatialMaterial>("res://material/maze_material.tres");
        mesh.SurfaceSetMaterial(0, material);
        GD.Print("generation complete");

        GD.Print("adding to scene");
        MeshInstance meshInstance = new MeshInstance();
        meshInstance.Name = "DungeonMesh";
        meshInstance.Mesh = mesh;
        meshInstance.CreateTrimeshCollision();
        AddChild(meshInstance);
    }

    // private void buildWallNorth(SurfaceTool st, int x, int z)
    // {
    //     GD.Print("north");
    //     // Build a wall on the south side rest open
    //     st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
    //     st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, TileSize * 2, z * TileSize)));
    //     st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
    // }

    // private void buildWallSouth(SurfaceTool st, int x, int z)
    // {
    //     GD.Print("south");
    //     // Build a wall on the south side rest open
    //     st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
    //     st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, TileSize * 2, z * TileSize)));
    //     st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize + TileSize)));
    // }

    // private void buildWallEast(SurfaceTool st, int x, int z)
    // {
    //     GD.Print("east");
    //     // Build a wall on the south side rest open
    //     st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
    //     st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, TileSize * 2, z * TileSize)));
    //     st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * TileSize + TileSize, 0, z * TileSize)));
    // }
    // private void buildWallWest(SurfaceTool st, int x, int z)
    // {
    //     GD.Print("west");
    //     // Build a wall on the south side rest open
    //     st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
    //     st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, TileSize * 2, z * TileSize)));
    //     st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
    // }


    private void buildCornerNorthWest(SurfaceTool st, int x, int z)
    {
        GD.Print("NW");
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, TileSize * 2, z * TileSize)));
        st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
        st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
    }

    private void buildCornerNorthEast(SurfaceTool st, int x, int z)
    {
                GD.Print("NE");
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, TileSize * 2, z * TileSize)));
        st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
        st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * TileSize + TileSize, 0, z * TileSize)));
    }

    private void buildCornerSouthWest(SurfaceTool st, int x, int z)
    {
                GD.Print("SW");
        // Build a corner peice on south west corner
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, TileSize * 2, z * TileSize)));
        st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize + TileSize)));
        st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
    }

    private void buildCornerSouthEast(SurfaceTool st, int x, int z)
    {
                        GD.Print("SE");
        // Build a corner peice on south east corner
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, TileSize * 2, z * TileSize)));
        st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize + TileSize)));
        st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * TileSize + TileSize, 0, z * TileSize)));
    }

    private void buildOpenNorth(SurfaceTool st, int x, int z)
    {
        // build a three sided square with opening to the north
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, TileSize, z * TileSize)));
        st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize + TileSize)));
        st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
        st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * TileSize + TileSize, 0, z * TileSize)));
    }

    private void buildTallOpen(SurfaceTool st, int x, int z)
    {
        GD.Print("open");
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, TileSize * 2, z * TileSize)));
    }

    private void buildTest(SurfaceTool st, int x, int z)
    {
        GD.Print("#$%*&^");
        // st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
    }

    private int getPosExponent(Tile[,] grid, int x, int z)
    {
        //TODO need extra check that corner tiles have adjacent neighbourd also adjacent to original
        if (grid[z, x].Blocking) return 0;
        return 1;
    }
    private int getFourBitMask(Tile[,] grid, int x, int z)
    {

        int total = 0;
        Dictionary<int, Vector2> positions = new Dictionary<int, Vector2>
        {
            {1, new Vector2(x-1, z-1)},
            {2, new Vector2(x, z-1)},
            {4, new Vector2(x+1, z-1)},
            {8, new Vector2(x-1, z)},
            {16, new Vector2(x+1, z)},
            {32, new Vector2(x-1, z+1)},
            {64, new Vector2(x, z+1)},
            {128, new Vector2(x+1, z+1)},
        };
        // Dictionary<int, Vector2> corners = new Dictionary<int, Vector2>
        // {
        //     {1, new Vector2(x-1, z-1)},
        //     {4, new Vector2(x+1, z-1)},
        //     {32, new Vector2(x-1, z+1)},
        //     {128, new Vector2(x+1, z+1)}
        // };

        // int tileCounter = 0;
        foreach(var pos in positions)
        {
            int _x = (int)pos.Value.x;
            int _z = (int)pos.Value.y;

            // only do the calculation if the corner tile has adjacent tiles as well
            // int exp =(int)Math.Pow(2, tileCounter);
            // GD.Print($"****{exp}");
            bool add = true;
            if (pos.Key == 1 && grid[z, x-1].Blocking && grid[z-1, x].Blocking) add = false;
            if (pos.Key == 4 && grid[z, x-1].Blocking && grid[z+1, x].Blocking) add = false;
            if (pos.Key == 32 && grid[z-1, x].Blocking && grid[z, x+1].Blocking) add = false;
            if (pos.Key == 128 && grid[z+1, x].Blocking && grid[z, x+1].Blocking) add = false;

            if (add) total += pos.Key * getPosExponent(grid, _x, _z);
            // tileCounter++;
        }
        return total;
    }

    public void SpawnPlayer(int _x, int _y)
    {
        int x = _x * TileSize + TileSize;
        int y = _y * TileSize + TileSize;

        PackedScene playerScene = (PackedScene)ResourceLoader.Load<PackedScene>("res://3DPrefabs/FPCharacter.tscn");
        KinematicBody player = (KinematicBody)playerScene.Instance();
        Transform playerTransform = new Transform(player.Transform.basis, new Vector3(x, Transform.origin.y + 1, y));
        player.Transform = playerTransform;
        player.Name = "Player";
        AddChild(player);

        _playerSpawned = true;

    }
    private async void _onRegenerateButtonPressed(int rooms, int mapSize)
    {
        var player = GetNode("Player");
        player.QueueFree();
        _playerSpawned = false;
        var dungeon = GetNode("DungeonMesh");
        dungeon.QueueFree();
        await ToSignal(player, "tree_exited");
        await ToSignal(dungeon, "tree_exited");
        GenerationController controller = GetParent<GenerationController>();
        controller.MapSize = mapSize;
        controller.RoomCount = rooms;
        controller.Generate();
    }
}
