using Godot;
using System;

public class FPCharacter : KinematicBody
{
    [Export]
    public int Speed = 7;
    [Export]
    public int Acceleration = 10;
    [Export]
    public float gravity = 0; //0.09F;
    [Export]
    public int Jump = 10;

    [Export]
    public float MouseSensitivity = 0.03F;

    private Vector3 _velocity;
    private Vector3 _fall;
    private Camera _camera;
    public override void _Ready()
    {
        // Capture the mouse
        Input.SetMouseMode(Input.MouseMode.Captured);
        _camera = (Camera)GetNode("Camera");
    }
    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseMotion && Input.GetMouseMode() == Input.MouseMode.Captured)
        {
            InputEventMouseMotion _event = (InputEventMouseMotion)inputEvent;
            HandleMouseMotion(_event);

        }
        if (inputEvent.IsActionPressed("ToggleMenu"))
        {
            if (Input.GetMouseMode() == Input.MouseMode.Captured)
            {
                Input.SetMouseMode(Input.MouseMode.Visible);
            }
            else {
                Input.SetMouseMode(Input.MouseMode.Captured);
            }
        }
    }
    public override void _PhysicsProcess(float delta)
    {
        HandleMovement(delta);
    }
    public void HandleMouseMotion(InputEventMouseMotion mouseMotion)
    {

        // pivot whole entity around y axis based on mouse motion
        // get rotation in Radians
        float _rotDegX = mouseMotion.Relative.x * MouseSensitivity;
        float _rotRadX = (float)-(_rotDegX * Math.PI / 180);
        RotateY(_rotRadX);

        // pivot the camera up and down based on mousemotion
        // get rotation in Radians
        float _rotDegY = mouseMotion.Relative.y * MouseSensitivity;
        float _rotRadY = (float)(_rotDegY * Math.PI / 180);
        float _maxRotation = (float)(90 * Math.PI / 180);
        _camera.RotateX(-_rotRadY);

        // clamp the x rotation to 90 degrees
        float clampedX = Math.Clamp(_camera.Rotation.x, -_maxRotation, _maxRotation);
        Vector3 resultRotation = new Vector3(clampedX, _camera.Rotation.y, _camera.Rotation.z);
        _camera.Rotation = resultRotation;

    }
    public void HandleMovement(float delta)
    {
        Vector3 _direction = new Vector3();
        if (Input.IsActionPressed("MoveForward"))
        {
            _direction -= Transform.basis.z;
        }
        if (Input.IsActionPressed("MoveBackward"))
        {
            _direction += Transform.basis.z;
        }
        if (Input.IsActionPressed("MoveLeft"))
        {
            _direction -= Transform.basis.x;
        }
        if (Input.IsActionPressed("MoveRight"))
        {
            _direction += Transform.basis.x;
        }

        // if (!IsOnFloor())
        // {
        //     _fall.y -= gravity;
        //     MoveAndSlide(_fall, Vector3.Up);
        // }
        // else
        // {
        //     _fall.y = 0;
        // }

        _direction = _direction.Normalized();
        _velocity = _velocity.LinearInterpolate(_direction * Speed, Acceleration * delta);
        _velocity = MoveAndSlide(_velocity, Vector3.Up);
    }

}
