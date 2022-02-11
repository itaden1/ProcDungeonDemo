using Godot;
using GamePasta.DungeonAlgorythms;
using System.Collections.Generic;
using System;

public class Dungeon3Dversion2 : Spatial
{
    [Export]
    public int MapSize = 60;
    [Export]
    public int SegmentCount = 4;
    [Export]
    public int RoomCount = 4;
    [Export]
    public float tileSize = 4f;
    public override void _Ready()
    {
        GenerateLevel();
    }
    public void GenerateLevel()
    {
        GridDungeon dungeonBuilder = new GridDungeon(MapSize, SegmentCount, RoomCount);

        Dictionary<string, Mesh> caveTileMeshes = new Dictionary<string, Mesh>()
        {
            {"northWall", ResourceLoader.Load<Mesh>("res://3DPrefabs/CaveTiles/northwall.tres")},
            {"southWall", ResourceLoader.Load<Mesh>("res://3DPrefabs/CaveTiles/southwall.tres")},
            {"eastWall", ResourceLoader.Load<Mesh>("res://3DPrefabs/CaveTiles/eastwall.tres")},
            {"westWall", ResourceLoader.Load<Mesh>("res://3DPrefabs/CaveTiles/westwall.tres")},

            {"northEastCorner", ResourceLoader.Load<Mesh>("res://3DPrefabs/CaveTiles/northeastcorner.tres")},
            {"southEastCorner", ResourceLoader.Load<Mesh>("res://3DPrefabs/CaveTiles/southeastcorner.tres")},
            {"northWestCorner", ResourceLoader.Load<Mesh>("res://3DPrefabs/CaveTiles/northwestcorner.tres")},
            {"southWestCorner", ResourceLoader.Load<Mesh>("res://3DPrefabs/CaveTiles/southwestcorner.tres")},

            {"northCorridor", ResourceLoader.Load<Mesh>("res://3DPrefabs/CaveTiles/northcorridor.tres")},
            {"eastCorridor", ResourceLoader.Load<Mesh>("res://3DPrefabs/CaveTiles/eastcorridor.tres")},

            {"northDeadEnd", ResourceLoader.Load<Mesh>("res://3DPrefabs/CaveTiles/northdeadend.tres")},
            {"southDeadEnd", ResourceLoader.Load<Mesh>("res://3DPrefabs/CaveTiles/southdeadend.tres")},
            {"eastDeadEnd", ResourceLoader.Load<Mesh>("res://3DPrefabs/CaveTiles/eastdeadend.tres")},
            {"westDeadEnd", ResourceLoader.Load<Mesh>("res://3DPrefabs/CaveTiles/westdeadend.tres")},

            {"open", (Mesh)ResourceLoader.Load<Mesh>("res://3DPrefabs/CaveTiles/opentile.tres")},
        };

        Dictionary<byte, Mesh> bitMaskMap = new Dictionary<byte, Mesh>(){
            {0, caveTileMeshes["open"]},

            {1, caveTileMeshes["northWall"]},
            {2, caveTileMeshes["westWall"]},
            {4, caveTileMeshes["eastWall"]},
            {8, caveTileMeshes["southWall"]},

            {3, caveTileMeshes["northWestCorner"]},
            {5, caveTileMeshes["northEastCorner"]},
            {10, caveTileMeshes["southWestCorner"]},
            {12, caveTileMeshes["southEastCorner"]},

            {6, caveTileMeshes["northCorridor"]},
            {9, caveTileMeshes["eastCorridor"]},

            {7, caveTileMeshes["northDeadEnd"]},
            {14, caveTileMeshes["southDeadEnd"]},
            {13, caveTileMeshes["eastDeadEnd"]},
            {11, caveTileMeshes["westDeadEnd"]},
        };

        SurfaceTool st = new SurfaceTool();
        st.Begin(Mesh.PrimitiveType.Triangles);

        foreach (var vec in dungeonBuilder.FullMask)
        {
            Vector2 vector = ToGDVec2(vec);
            byte mask = Helpers.getFourBitMask(dungeonBuilder.FullMask, vec);
            if (bitMaskMap.ContainsKey(mask))
            {
                Mesh m = bitMaskMap[mask];
                st.AppendFrom(m, 0, new Transform(Basis.Identity, new Vector3(vector.x * tileSize, 0, vector.y * tileSize)));
            }
        }
        st.Index();

        Mesh mesh = st.Commit();
        SpatialMaterial material = (SpatialMaterial)ResourceLoader.Load<SpatialMaterial>("res://material/dungeon_wall.material");
        mesh.SurfaceSetMaterial(0, material);
        GD.Print("generation complete");

        GD.Print("adding to scene");
        MeshInstance meshInstance = new MeshInstance();
        meshInstance.Name = "DungeonMesh";
        meshInstance.Mesh = mesh;
        meshInstance.CreateTrimeshCollision();
        AddChild(meshInstance);

        Random rng = new Random();
        var startChamber = dungeonBuilder.Chambers[dungeonBuilder.MainPath[0]][0];
        var chamberVecs = startChamber.ToList();
        var startPos = chamberVecs[rng.Next(0, chamberVecs.Count - 1)];

        PackedScene playerScene = (PackedScene)ResourceLoader.Load<PackedScene>("res://3DPrefabs/FPCharacter.tscn");
        KinematicBody player = (KinematicBody)playerScene.Instance();
        Transform playerTransform = new Transform(player.Transform.basis, new Vector3(startPos.X, Transform.origin.y + 1, startPos.Y));
        player.Transform = playerTransform;
        player.Name = "Player";
        Node world = GetTree().Root.GetNode("World");
        world.CallDeferred("add_child", player);

    }
    private Vector2 ToGDVec2(System.Numerics.Vector2 vec)
    {
        return new Vector2(vec.X, vec.Y);
    }
}
