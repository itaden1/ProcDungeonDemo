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

    private bool _playerSpawned = false;
    public override void _Ready()
    {
        GD.Print("starting");
        GetParent().GetNode("GUI").Connect("RegenerateDungeon", this, nameof(_onRegenerateButtonPressed));

    }

    public void Generate(DungeonGrid myMap)
    {
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
            { 11, new CornerSouthEastBuilder3D(TileSize, meshSet) },
            { 16, new TestTileBuilder(TileSize, meshSet) },
            { 18, new TestTileBuilder(TileSize, meshSet) },
            { 22, new CornerSouthWestBuilder3D(TileSize, meshSet) },
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
            { 104, new CornerNorthEastBuilder3D(TileSize, meshSet) },
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
                    if (tileSet.ContainsKey(mask))
                    {
                        TileBuilderBase tb = tileSet[mask];
                        GD.Print($"{mask}: {tb}");
                        tb.Build(st, x, z);
                    }
                    else GD.Print($"Missing key: {mask}");

                    
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

    private int getFourBitMask(Tile[,] grid, int x, int z)
    {

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

        byte total = 0;
        
        foreach(var pos in positions)
        {
            int _x = (int)pos.Value.x;
            int _z = (int)pos.Value.y;

            bool add = !grid[_z, _x].Blocking;
            // only add corner to the calculation if they have 2 adjoining open tiles that are not blocking
            if (pos.Key == 1 && add) add = (!grid[_z+1, _x].Blocking && !grid[_z, _x+1].Blocking);
            if (pos.Key == 4 && add) add = (!grid[_z, _x-1].Blocking && !grid[_z+1, _x].Blocking);
            if (pos.Key == 32 && add) add = (!grid[_z-1, _x].Blocking && !grid[_z, _x+1].Blocking);
            if (pos.Key == 128 && add) add = (!grid[_z, _x-1].Blocking && !grid[_z-1, _x].Blocking);
            
            if (add) total += (byte)pos.Key;
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
        Node world = GetTree().Root.GetNode("World");
        world.CallDeferred("add_child", player);

        Map map = GetTree().Root.GetNode<Map>("World/CanvasLayer/Map");
        map.ConnectPlayerSignal(player, "ShowMap");

        _playerSpawned = true;

    }
    private async void _onRegenerateButtonPressed(int rooms, int mapSize)
    {
        var player = GetTree().Root.GetNode("World/Player");
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
