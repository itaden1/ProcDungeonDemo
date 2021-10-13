using Godot;
using System;
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

    private bool _playerSpawned = false;
    public override void _Ready()
    {
        GD.Print("starting");
        GetParent().GetNode("GUI").Connect("RegenerateDungeon", this, nameof(_onRegenerateButtonPressed));
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
                    // int mask = getMask(myMap, x, y);
                    // level 1 walls
                    if (myMap.Grid[z, x-1].Blocking)
                    {
                        st.AppendFrom(pillar, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize + TileSize)));
                        st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
                        st.AppendFrom(skirtingNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
                    }
                    if (myMap.Grid[z-1, x].Blocking)
                    {
                        st.AppendFrom(pillar, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
                        st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
                        st.AppendFrom(skirtingEastWest, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
                    }
                    if (myMap.Grid[z, x+1].Blocking)
                    {
                        st.AppendFrom(pillar, 0, new Transform(Basis.Identity, new Vector3(x * TileSize + TileSize, 0, z * TileSize)));
                        st.AppendFrom(wallNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * TileSize + TileSize, 0, z * TileSize)));
                        st.AppendFrom(skirtingNorthSouth, 0, new Transform(Basis.Identity, new Vector3(x * TileSize + TileSize, 0, z * TileSize)));
                    }
                    if (myMap.Grid[z+1, x].Blocking)
                    {
                        st.AppendFrom(pillar, 0, new Transform(Basis.Identity, new Vector3(x * TileSize + TileSize, 0, z * TileSize + TileSize)));
                        st.AppendFrom(wallEastWest, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize + TileSize)));
                        st.AppendFrom(skirtingEastWest, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize + TileSize)));
                    }

                    st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, 0, z * TileSize)));
                    st.AppendFrom(floor, 0, new Transform(Basis.Identity, new Vector3(x * TileSize, TileSize, z * TileSize)));

                    
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

    private int getMask(DungeonGrid myMap, int x, object y)
    {

        throw new NotImplementedException();
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
