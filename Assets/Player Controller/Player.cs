using Godot;
using System;
using System.Net.Http;
using System.Numerics;
using System.Security.AccessControl;
using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;

public partial class Player : CharacterBody3D
{
	[ExportGroup("Ground Movement")]
	[Export] public float CrouchSpeed = 3.5f;
	[Export] public float MoveSpeed = 6f;
	[Export] public float SprintMultiplier = 1.5f;
	[Export] public float Acceleration = 15f;
    [Export] public float RollDistance = 10f;
    private bool _isCrouching = false;
    private bool _isSprinting = false;
    private bool _canRoll = true;
    private bool _isRolling = false;
    private Timer _rollCooldown;
    private Timer _rollDuration;
    private Vector2 _inputDirection;
    private Vector3 _moveDirection = Vector3.Zero;
    private Vector3 _targetVelocity;

    public override void _Ready()
    {
	    _rollCooldown = GetNode<Timer>("Roll Cooldown");
	    _rollDuration = GetNode<Timer>("Roll Duration");
	    _rollCooldown.Timeout += () => _canRoll = true;
	    _rollDuration.Timeout += () => _isRolling = false;
    }
    
    public override void _Input(InputEvent @event)
	{
		_inputDirection = Input.GetVector("move_forward", "move_back", "move_left", "move_right");
		_moveDirection =  new Vector3(_inputDirection.X, 0, _inputDirection.Y).Normalized();

		if (@event.IsActionPressed("crouch") && _isCrouching == false)
			_isCrouching = true;
		else if (@event.IsActionPressed("crouch") && _isCrouching == true)
			_isCrouching = false;

		if (@event.IsActionPressed("sprint"))
			_isSprinting = true;
		else if (@event.IsActionReleased("sprint") && _isSprinting == true)
			_isSprinting = false;

		if (@event.IsActionPressed("roll") && _canRoll == true && _isRolling == false)
		{
			Roll();
		}
		
	}

	public override void _PhysicsProcess(double delta)
	{
		_targetVelocity = _moveDirection * MoveSpeed;

		if (_isSprinting == true && _isCrouching == false)
			_targetVelocity = _moveDirection * MoveSpeed * SprintMultiplier;
		else if (_isSprinting == true && _isCrouching == true)
			_targetVelocity = _moveDirection * CrouchSpeed * SprintMultiplier;
		else if (_isSprinting == false && _isCrouching == true)
			_targetVelocity = _moveDirection * CrouchSpeed;

		if (_isRolling == false)
		{
			Vector3 velocity = Velocity;
            velocity.X = (float)Mathf.Lerp(velocity.X, _targetVelocity.Z, Acceleration * delta);
            velocity.Z = (float)Mathf.Lerp(velocity.Z, _targetVelocity.X, Acceleration * delta);
			Velocity = velocity;
		}
		
		MoveAndSlide();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	private void Roll()
	{
		_canRoll = false;
		_isRolling = true;
		_rollCooldown.Start();
		_rollDuration.Start();
		
		Vector3 rollDirection = -GlobalTransform.Basis.Z;
		if (_inputDirection != Vector2.Zero)
		{
			rollDirection = new Vector3(_inputDirection.Y, 0, _inputDirection.X).Normalized();
		}

		if (_rollDuration.TimeLeft != 0)
		{
			Vector3 rollVelocity = rollDirection * RollDistance;
			Velocity = rollVelocity;
		}
	}
}
