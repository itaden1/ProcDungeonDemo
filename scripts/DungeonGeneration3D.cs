using Godot;
using System;
using System.Collections.Generic;
using ProcDungeon.Structures;
using ProcDungeon.Algorythms;
using ProcDungeon;


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
	private int _mapSize = 40;

	[Export]
	private int _roomCount = 25;

    [Export]
    public int TileSize = 2;
    [Export]
    public float UVOffset = 0.5F;

    private UVCoordSet _wallUV = new UVCoordSet(){x1=0, y1=0.5F, x2=0.5F, y2=1.0F};
    private UVCoordSet _floorUV = new UVCoordSet(){x1=0, y1=0, x2=0.5F, y2=0.5F};

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
		Generate();
    }

    public void Generate()
    {
        GD.Print("generating");
		var alg = new BSPDungeonAlgorythm(_roomCount);
		var myMap = new DungeonGrid(_mapSize, alg);
		myMap.GenerateLayout();

        Mesh pillar = (Mesh)ResourceLoader.Load<Mesh>("res://3DPrefabs/pillar.tres");
        Mesh floor = (Mesh)ResourceLoader.Load<Mesh>("res://3DPrefabs/floor.tres");
        Mesh wallEastWest = (Mesh)ResourceLoader.Load<Mesh>("res://3DPrefabs/wallEW.tres");
        Mesh wallNorthSouth = (Mesh)ResourceLoader.Load<Mesh>("res://3DPrefabs/wallNS.tres");
        Mesh skirtingNorthSouth = (Mesh)ResourceLoader.Load<Mesh>("res://3DPrefabs/skirtingNS.tres");
        Mesh skirtingEastWest = (Mesh)ResourceLoader.Load<Mesh>("res://3DPrefabs/skirtingEW.tres");


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
                    // { 2 = 1, 8 = 2, 10 = 3, 11 = 4, 16 = 5, 18 = 6, 22 = 7, 24 = 8, 26 = 9, 27 = 10, 30 = 11, 31 = 12, 64 = 13, 66 = 14, 72 = 15, 74 = 16, 75 = 17, 80 = 18, 82 = 19, 86 = 20, 88 = 21, 90 = 22, 91 = 23, 94 = 24, 95 = 25, 104 = 26, 106 = 27, 107 = 28, 120 = 29, 122 = 30, 123 = 31, 126 = 32, 127 = 33, 208 = 34, 210 = 35, 214 = 36, 216 = 37, 218 = 38, 219 = 39, 222 = 40, 223 = 41, 248 = 42, 250 = 43, 251 = 44, 254 = 45, 255 = 46, 0 = 47 }
                    switch(mask)
                    {
                        case 2: // t1  open all sides
                            buildTallOpen(st, x, z);
                            break;
                        case 8: // t2
                            buildTest(st, x, z);
                            break;
                        case 10: // t3
                            buildTest(st, x, z);
                            break;
                        case 11: // t4
                            buildTest(st, x, z);
                            break;
                        case 16: //t5
                            buildCornerSouthEast(st, x, z);
                            break;
                        case 18: // t6
                            buildTest(st, x, z);
                            break;
                        case 22: // t7
                            buildTest(st, x, z);
                            break;
                        case 24: // t8
                            buildCornerSouthWest(st, x, z);
                            break;
                        case 26: // t9
                            buildTest(st, x, z);
                            break;
                        case 27: //t10
                            buildTest(st, x, z);
                            break;
                        case 30: // t11
                            buildTest(st, x, z);
                            break;
                        case 31: // t12
                            buildTest(st, x, z);
                            break;
                        case 64: // t13
                            buildWallSouth(st, x, z);
                            break;
                        case 66: // t14
                            buildTest(st, x, z);
                            break;
                        case 72: // t15
                            buildTest(st, x, z);
                            break;
                        case 74: //t16
                            buildTest(st, x, z);
                            break;
                        case 75: // t17
                            buildTest(st, x, z);
                            break;
                        case 80: //t18
                            buildTest(st, x, z);
                            break;
                        case 82: //t19
                            buildTest(st, x, z);
                            break;
                        case 86: // t20
                            buildTest(st, x, z);
                            break;
                        case 88: // t21
                            buildTest(st, x, z);
                            break;
                        case 90: //t22
                            buildTest(st, x, z);
                            break;
                        case 91: //t23
                            buildTest(st, x, z);
                            break;
                        case 94: //t24
                            buildTest(st, x, z);
                            break;
                        case 95: //t25
                            buildTest(st, x, z);
                            break;
                        case 104: //t26
                            buildTest(st, x, z);
                            break;
                        case 106: //t27
                            buildCornerNorthEast(st, x, z);
                            break;
                        case 107: //t28
                            break;
                        case 120: //t29
                            buildWallEast( st, x, z);
                            break;
                        case 122: // 30
                            buildTest(st, x, z);
                            break;
                        case 123: //t31
                            buildTest(st, x, z);
                            break;
                        case 126: //t32
                            buildTest(st, x, z);
                            break;
                        case 127: //t33
                            buildTest(st, x, z);
                            break;
                        case 208: // t34
                            buildTest(st, x, z);
                            break;
                        case 210: //t35
                            buildCornerNorthWest(st, x, z);
                            break;
                        case 214: //t36
                            buildTest(st, x, z);
                            break;
                        case 216: //t37
                            buildWallWest(st, x, z);
                            break;
                        case 218: //t38
                            buildTest(st, x, z);
                            break;
                        case 219: //t39
                            buildTest(st, x, z);
                            break;
                        case 222: //t40
                            buildTest(st, x, z);
                            break;
                        case 223: //t41
                            buildTest(st, x, z);
                            break;
                        case 248: //t42
                            buildTest(st, x, z);
                            break;
                        case 250: //t43
                            buildWallNorth(st, x, z);
                            break;
                        case 251: //t44
                            buildTest(st, x, z);
                            break;
                        case 254: //t45
                            buildTallOpen(st, x, z);
                            break;
                        case 255: //t46
                            buildTallOpen(st, x, z);
                            break;
                        case 0: //t47
                            buildTest(st, x, z);
                            break;
                    }
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

    private void buildWallNorth(SurfaceTool st, int x, int z)
    {
        // Build a wall on the south side rest open
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, TileSize * 2, z * TileSize)));
        st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
    }

    private void buildWallSouth(SurfaceTool st, int x, int z)
    {
        // Build a wall on the south side rest open
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, TileSize * 2, z * TileSize)));
        st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize + TileSize)));
    }

    private void buildWallEast(SurfaceTool st, int x, int z)
    {
        // Build a wall on the south side rest open
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, TileSize * 2, z * TileSize)));
        st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * TileSize + TileSize, 0, z * TileSize)));
    }
    private void buildWallWest(SurfaceTool st, int x, int z)
    {
        // Build a wall on the south side rest open
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, TileSize * 2, z * TileSize)));
        st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
    }


    private void buildCornerNorthWest(SurfaceTool st, int x, int z)
    {
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, TileSize * 2, z * TileSize)));
        st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
        st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
    }

    private void buildCornerNorthEast(SurfaceTool st, int x, int z)
    {
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, TileSize * 2, z * TileSize)));
        st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
        st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * TileSize + TileSize, 0, z * TileSize)));
    }

    private void buildCornerSouthWest(SurfaceTool st, int x, int z)
    {
        // Build a corner peice on south west corner
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, TileSize, z * TileSize)));
        st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize + TileSize)));
        st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
    }

    private void buildCornerSouthEast(SurfaceTool st, int x, int z)
    {
        // Build a corner peice on south east corner
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, TileSize, z * TileSize)));
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
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, TileSize * 2, z * TileSize)));
    }

    private void buildTest(SurfaceTool st, int x, int z)
    {
        st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
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
        Dictionary<int, Vector2> positions = new Dictionary<int, Vector2>{
            {1, new Vector2(x-1, z-1)},
            {2, new Vector2(x, z-1)},
            {4, new Vector2(x+1, z-1)},
            {8, new Vector2(x-1, z)},
            {16, new Vector2(x+1, z)},
            {32, new Vector2(x-1, z+1)},
            {64, new Vector2(x, z+1)},
            {128, new Vector2(x+1, z+1)},
        };
        foreach(var pos in positions)
        {
            total += pos.Key * getPosExponent(grid, (int)pos.Value.x, (int)pos.Value.y);
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
        _roomCount = rooms;
		_mapSize = mapSize;
        var player = GetNode("Player");
        player.QueueFree();
        _playerSpawned = false;
        var dungeon = GetNode("DungeonMesh");
        dungeon.QueueFree();
        await ToSignal(player, "tree_exited");
        await ToSignal(dungeon, "tree_exited");
        Generate();
    }
}
