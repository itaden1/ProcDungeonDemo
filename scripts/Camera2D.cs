using Godot;
using System;

public class Camera2D : Godot.Camera2D
{
	[Export]
	public float maxZoomLevel = 0.5f;

	[Export]
	public float minZoomLevel = 4.0f;

	[Export]
	public float zoomIncrement = 0.05f;

	private float _currentZoomLevel = 1f;
	private bool _drag = false;

	public override void _Input(InputEvent InputEvent)
	{
		if (InputEvent.IsActionPressed("cam_drag"))
		{
			_drag = true;
		}
		if (InputEvent.IsActionReleased("cam_drag"))
		{
			_drag = false;
		}
		if (Input.IsActionJustPressed("zoom_in"))
		{
			_currentZoomLevel -= zoomIncrement;
			if(_currentZoomLevel <= maxZoomLevel) _currentZoomLevel = maxZoomLevel;
			Zoom = new Vector2(_currentZoomLevel, _currentZoomLevel); 

		}
		if (Input.IsActionJustPressed("zoom_out"))
		{
			_currentZoomLevel += zoomIncrement;
			if(_currentZoomLevel >= minZoomLevel) _currentZoomLevel = minZoomLevel;
			Zoom = new Vector2(_currentZoomLevel, _currentZoomLevel); 

		}
		if (InputEvent is InputEventMouseMotion mouseEvent && _drag)
		{
			Offset = Offset - mouseEvent.Relative * _currentZoomLevel;
		}
		
	}
}
