using Godot;
using System;
using ProcDungeon.Structures;
using ProcDungeon.Algorythms;
using ProcDungeon;
public class Map : Node2D
{

    protected Tile[,] _grid;
    public override void _Ready()
    {
        
    }
    public void Generate(Tile[,] grid)
    {
        _grid = grid;
        _Draw();
    }
    public override void _Draw()
    {
        for(int y = 0; y < _grid.GetLength(0); y++)
        {
            for(int x = 0; x < _grid.GetLongLength(1); x++)
            {
                
            }
        }
    }
}
