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

    public int TileSize = 2;
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
                    generatePlane(st, x, y);
                }
            }
        }

        st.Index();

        Mesh mesh = st.Commit();
        SpatialMaterial material = (SpatialMaterial)ResourceLoader.Load<SpatialMaterial>("res://material/floor_material.tres");
        mesh.SurfaceSetMaterial(0, material);
        GD.Print("generation complete");
        Mesh = mesh;
        GD.Print("adding to scene");

    }

    private void generatePlane(SurfaceTool st, int _x, int _y)
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
    private void _onRegenerateButtonPressed(int rooms, int mapSize)
    {
        _roomCount = rooms;
		_mapSize = mapSize;
        Generate();
    }
}
