using Godot;
using System;
using System.Numerics;
using System.Security.AccessControl;
using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;

public partial class Player : CharacterBody3D
{
	[ExportGroup("Ground Movement")]
	[Export] public float CrouchSpeed = 4f;
	[Export] public float MoveSpeed = 6f;
	[Export] public float SprintMultiplier = 1.2f;
	[Export] public float Acceleration = 15f;
    [Export] public float RollSpeed = 10f;
    private bool _isCrouching = false;
    private Vector2 _inputDirection;
    private Vector3 _moveDirection = Vector3.Zero;
    private Vector3 _targetVelocity;

    private void HandleInput()
    {
		_inputDirection = Input.GetVector("move_forward", "move_back", "move_left", "move_right");
		_moveDirection = (Transform.Basis * new Vector3(_inputDirection.X, 0, _inputDirection.Y).Normalized());
		_targetVelocity = _moveDirection * MoveSpeed;
    }

    private void Crouch()
    {
	    if (Input.IsActionJustPressed("crouch") && _isCrouching == false)
	    {
		    _isCrouching = true;
		    _targetVelocity = _moveDirection * CrouchSpeed;
	    }
	    else if (Input.IsActionJustPressed("crouch") && _isCrouching == true)
	    {
		    _isCrouching = false;
		    _targetVelocity = _moveDirection * MoveSpeed;
	    }
    }
    
	public override void _UnhandledInput(InputEvent @event)
	{
		HandleInput();
		Crouch();
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		velocity.X = (float)Mathf.Lerp(velocity.X, _targetVelocity.Z, Acceleration * delta);
		velocity.Z = (float)Mathf.Lerp(velocity.Z, _targetVelocity.X, Acceleration * delta);
		
		Velocity = velocity;
		MoveAndSlide();
	}
}
