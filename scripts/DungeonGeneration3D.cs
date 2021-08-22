using Godot;
using System;
using ProcDungeon.Structures;
using ProcDungeon.Algorythms;
using ProcDungeon;

public class DungeonGeneration3D : MeshInstance
{
    [Export]
	private int _mapSize = 40;

	[Export]
	private int _roomCount = 25;

    [Export]
    public int TileSize = 2;

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

        SurfaceTool st = new SurfaceTool();
        st.Begin(Mesh.PrimitiveType.Triangles);
        for (int y = 0; y < myMap.Grid.GetLength(0); y++)
        {
            for (int x = 0; x < myMap.Grid.GetLength(1); x++)
            {
                if (myMap.Grid[y, x].Blocking == false)
                {
                    if (myMap.Grid[y, x-1].Blocking)
                    {
                        GenerateVerticalPlaneX(st, x, y);
                    }
                    if (myMap.Grid[y-1, x].Blocking)
                    {
                        GenerateVerticalPlaneY(st, x, y);
                    }
                    if (myMap.Grid[y, x+1].Blocking)
                    {
                        GenerateVerticalPlaneX(st, x+1, y);
                    }
                    if (myMap.Grid[y+1, x].Blocking)
                    {
                        GenerateVerticalPlaneY(st, x, y+1);
                    }
                    GenerateHorizontalPlane(st, x, y);
                    if(!_playerSpawned) SpawnPlayer(x, y);
                }
            }
        }

        st.Index();

        Mesh mesh = st.Commit();
        SpatialMaterial material = (SpatialMaterial)ResourceLoader.Load<SpatialMaterial>("res://material/floor_material.tres");
        mesh.SurfaceSetMaterial(0, material);
        GD.Print("generation complete");

        GD.Print("adding to scene");
        Mesh = mesh;
        CreateTrimeshCollision();
    }
    private void GenerateVerticalPlaneY(SurfaceTool st, int _x, int _y)
    {
        int x = _x * TileSize;
        int y = _y * TileSize;

        // First triangle
        st.AddNormal(new Vector3(0, 0, 1));
        st.AddUv(new Vector2(x, y));
        st.AddVertex(new Vector3(x, 0, y));

        st.AddNormal(new Vector3(0, 0, 1));
        st.AddUv(new Vector2(x, TileSize));
        st.AddVertex(new Vector3(x, TileSize, y));

        st.AddNormal(new Vector3(0, 0, 1));
        st.AddUv(new Vector2(x, y + TileSize));
        st.AddVertex(new Vector3(x + TileSize, TileSize, y));

        // Second triangle
        st.AddNormal(new Vector3(0, 0, 1));
        st.AddUv(new Vector2(x, y));
        st.AddVertex(new Vector3(x, 0, y));

        st.AddNormal(new Vector3(0, 0, 1));
        st.AddUv(new Vector2(x, y + TileSize));
        st.AddVertex(new Vector3(x + TileSize, TileSize, y));

        st.AddNormal(new Vector3(0, 0, 1));
        st.AddUv(new Vector2(x, y + TileSize));
        st.AddVertex(new Vector3(x + TileSize, 0, y));
    }
    private void GenerateVerticalPlaneX(SurfaceTool st, int _x, int _y)
    {
        int x = _x * TileSize;
        int y = _y * TileSize;

        // First triangle
        st.AddNormal(new Vector3(0, 0, 1));
        st.AddUv(new Vector2(x, y));
        st.AddVertex(new Vector3(x, 0, y));

        st.AddNormal(new Vector3(0, 0, 1));
        st.AddUv(new Vector2(x, TileSize));
        st.AddVertex(new Vector3(x, TileSize, y));

        st.AddNormal(new Vector3(0, 0, 1));
        st.AddUv(new Vector2(x, y + TileSize));
        st.AddVertex(new Vector3(x, TileSize, y + TileSize));

        // Second triangle
        st.AddNormal(new Vector3(0, 0, 1));
        st.AddUv(new Vector2(x, y));
        st.AddVertex(new Vector3(x, 0, y));

        st.AddNormal(new Vector3(0, 0, 1));
        st.AddUv(new Vector2(x, y + TileSize));
        st.AddVertex(new Vector3(x, TileSize, y + TileSize));

        st.AddNormal(new Vector3(0, 0, 1));
        st.AddUv(new Vector2(x, y + TileSize));
        st.AddVertex(new Vector3(x, 0, y + TileSize));
    }
    private void GenerateHorizontalPlane(SurfaceTool st, int _x, int _y)
    {
        int x = _x * TileSize;
        int y = _y * TileSize;

        // First triangle
        st.AddNormal(new Vector3(0, 0, 1));
        st.AddUv(new Vector2(x, y));
        st.AddVertex(new Vector3(x, 0, y));

        st.AddNormal(new Vector3(0, 0, 1));
        st.AddUv(new Vector2(x + TileSize, y));
        st.AddVertex(new Vector3(x + TileSize, 0, y));

        st.AddNormal(new Vector3(0, 0, 1));
        st.AddUv(new Vector2(x, y + TileSize));
        st.AddVertex(new Vector3(x, 0, y + TileSize));

        // second triangle
        st.AddNormal(new Vector3(0, 0, 1));
        st.AddUv(new Vector2(x + TileSize, y));
        st.AddVertex(new Vector3(x + TileSize, 0, y));

        st.AddNormal(new Vector3(0, 0, 1));
        st.AddUv(new Vector2(x + TileSize, y + TileSize));
        st.AddVertex(new Vector3(x + TileSize, 0, y + TileSize));

        st.AddNormal(new Vector3(0, 0, 1));
        st.AddUv(new Vector2(x, y + TileSize));
        st.AddVertex(new Vector3(x, 0, y + TileSize));
    }

    public void SpawnPlayer(int _x, int _y)
    {
        int x = _x * TileSize + TileSize;
        int y = _y * TileSize + TileSize;

        PackedScene playerScene = (PackedScene)ResourceLoader.Load<PackedScene>("res://3DPrefabs/FPCharacter.tscn");
        KinematicBody player = (KinematicBody)playerScene.Instance();
        Transform playerTransform = new Transform(player.Transform.basis, new Vector3(x, Transform.origin.y + 1, y));
        player.Transform = playerTransform;
        AddChild(player);

        _playerSpawned = true;

    }
    private void _onRegenerateButtonPressed(int rooms, int mapSize)
    {
        _roomCount = rooms;
		_mapSize = mapSize;
        Generate();
    }
}
