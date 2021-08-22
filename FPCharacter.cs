using Godot;
using System;

public class FPCharacter : KinematicBody
{
    [Export]
    public int Speed = 7;
    [Export]
    public int Acceleration = 10;
    [Export]
    public float gravity = 0.09F;
    [Export]
    public int Jump = 10;

    [Export]
    public float MouseSensitivity = 0.03F;

    private Vector3 _direction;
    private Vector3 _velocity;
    private Vector3 _gravity;

    private Camera _camera;
    public override void _Ready()
    {
        // Capture the mouse
        Input.SetMouseMode(Input.MouseMode.Captured);
        _camera = (Camera)GetNode("Camera");

    }
    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseMotion)
        {
            InputEventMouseMotion _event = (InputEventMouseMotion)inputEvent;

            // pivot whole entity around y axis based on mouse motion
            // get rotation in Radians
            float _rotDegX = _event.Relative.x * MouseSensitivity;
            float _rotRadX = (float)-(_rotDegX * Math.PI / 180);
            RotateY(_rotRadX);

            // pivot the camera up and down based on mousemotion
            // get rotation in Radians
            float _rotDegY = _event.Relative.y * MouseSensitivity;
            float _rotRadY = (float)(_rotDegY * Math.PI / 180);
            float _maxRotation = (float)(90 * Math.PI / 180);
            _camera.RotateX(-_rotRadY);

            // clamp the x rotation to 90 degrees
            float clampedX = Math.Clamp(_camera.Rotation.x, -_maxRotation, _maxRotation);
            Vector3 resultRotation = new Vector3(clampedX, _camera.Rotation.y, _camera.Rotation.z);
            _camera.Rotation = resultRotation;
        }
    }
    public override void _PhysicsProcess(float delta)
    {
     
    }
}
