using Godot;

public class PlayerController : KinematicBody2D
{
	private int _speed = 200;
	private int _jumpForce = 200;
	private int _gravity = 400;

	private bool _isDashing, _isDashAvailable, _canClimb, _isClimbing, _isWallJumping;

	private int _dashSpeed = 600;
	private float _dashTimer = .2f;
	private const float DashTimerReset = .2f;
	
	private float _wallJumpTimer = .4f;
	private const float WallJumpTimerReset = .4f;

	private int _climbSpeed = 200;

	private float _friction = .1f;
	private float _acceleration = .5f;

	private Vector2 _velocity;

	private RayCast2D RayCastLeft, RayCastRight;
	private RayCast2D RayCastLeftClimb, RayCastRightClimb;

	public override void _Ready()
	{
		RayCastLeft = GetNode<RayCast2D>("RayCastLeft");
		RayCastRight = GetNode<RayCast2D>("RayCastRight");
		RayCastLeftClimb = GetNode<RayCast2D>("RayCastLeftClimb");
		RayCastRightClimb = GetNode<RayCast2D>("RayCastRightClimb");
	}

	public override void _Process(float delta)
	{
		if (!_isDashing && !_isWallJumping)
		{
			ProcessMovement();
		}

		if (IsOnFloor())
		{
			if (Input.IsActionJustPressed("jump"))
			{
				_velocity.y = -_jumpForce;
			}

			_canClimb = true;
			_isDashAvailable = true;
		}

		if (_canClimb && Input.IsActionPressed("climb"))
		{
			ProcessClimb(delta);
		}
		else _isClimbing = false;

		ProcessWallJump(delta);

		if (_isDashAvailable && Input.IsActionJustPressed("dash"))
		{
			ProcessDash();
		}

		if (_isDashing)
		{
			_dashTimer -= delta;

			if (_dashTimer <= 0)
			{
				_isDashing = false;
				_velocity = new Vector2(0, 0);
			}
		}
		else if (!_isClimbing)
		{
			_velocity.y += _gravity * delta;
		}
		
		MoveAndSlide(_velocity, Vector2.Up);
	}

	private void ProcessMovement()
	{
		var direction = 0;

		if (Input.IsActionPressed("ui_left"))
		{
			direction -= 1;
		}

		if (Input.IsActionPressed("ui_right"))
		{
			direction += 1;
		}

		_velocity.x = direction != 0
			? Mathf.Lerp(_velocity.x, direction * _speed, _acceleration)
			: Mathf.Lerp(_velocity.x, 0, _friction);
	}
	private void ProcessDash()
	{
		_isDashing = true;

		if (Input.IsActionPressed("ui_left")) _velocity.x = -_dashSpeed;
		if (Input.IsActionPressed("ui_right")) _velocity.x = _dashSpeed;
		if (Input.IsActionPressed("ui_up")) _velocity.y = -_dashSpeed;
		if (Input.IsActionPressed("ui_right") && Input.IsActionPressed("ui_up"))
		{
			_velocity.x = _dashSpeed;
			_velocity.y = -_dashSpeed;
		}

		if (Input.IsActionPressed("ui_left") && Input.IsActionPressed("ui_up"))
		{
			_velocity.x = -_dashSpeed;
			_velocity.y = -_dashSpeed;
		}

		_dashTimer = DashTimerReset;
		_isDashAvailable = false;
	}
	private void ProcessClimb(float delta)
	{
		if (RayCastLeftClimb.IsColliding() || RayCastRightClimb.IsColliding() || RayCastLeft.IsColliding()
		    || RayCastRight.IsColliding())
		{
			_isClimbing = true;

			if (Input.IsActionPressed("ui_up"))
			{
				_velocity.y = -_climbSpeed;
			}
			else if (Input.IsActionPressed("ui_down"))
			{
				_velocity.y = _climbSpeed;
			}
			else
			{
				_velocity = new Vector2(0, 0);
			}
		}
		else _isClimbing = false;
	}
	private void ProcessWallJump(float delta)
	{
		if (Input.IsActionJustPressed("jump") && RayCastLeft.IsColliding())
		{
			_velocity.y = -_jumpForce;
			_velocity.x = _jumpForce;
			_isWallJumping = true;
		}
		else if (Input.IsActionJustPressed("jump") && RayCastRight.IsColliding())
		{
			_velocity.y = -_jumpForce;
			_velocity.x = -_jumpForce;
			_isWallJumping = true;
		}

		if (_isWallJumping)
		{
			_wallJumpTimer -= delta;

			if (_wallJumpTimer <= 0)
			{
				_isWallJumping = false;
				_wallJumpTimer = WallJumpTimerReset;
			}
		}
	}
}
