using Godot;

public class GUI : CanvasLayer
{
    [Signal]
    public delegate void RegenerateDungeon(int rooms, int size);

    private int _roomCount = 5;
    private int _dungeonSize = 60;

    private void _on_DungeonSizeChanged(float value)
    {
        _dungeonSize = (int)value;
    }
    private void _on_RoomCountChanged(float value)
    {
        _roomCount = (int)value;
    }
    private void _on_RegenerateBtnPressed()
    {
        EmitSignal(nameof(RegenerateDungeon), _roomCount, _dungeonSize);
    }

}
