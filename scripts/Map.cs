using Godot;
using System;
using ProcDungeon.Structures;
using ProcDungeon.Algorythms;
using ProcDungeon;
public class Map : Node2D
{

    protected Tile[,] _grid;

    private int tileSize = 20;
    public override void _Ready()
    {
    }

    public void ConnectPlayerSignal(Node player, string signal)
    {
        player.Connect(signal, this, $"_on{signal}");
    }

    public void _onShowMap()
    {
        Visible = !Visible;
    }
    public void Generate(Tile[,] grid)
    {
        _grid = grid;
        // _Draw();
    }
    public override void _Process(float delta)
    {
        Update();
    }
    public override void _Draw()
    {
        for(int y = 0; y < _grid.GetLength(0); y++)
        {
            for(int x = 0; x < _grid.GetLength(1); x++)
            {
                if (_grid[y,x].Blocking)
                {
                    Rect2 rect = new Rect2(x * tileSize, y * tileSize, tileSize, tileSize);
                    DrawRect(rect, new Color(2, 2, 2, 0.3f));
                }
            }
        }
    }
}
