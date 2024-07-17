using Godot;

public class PlayerController : KinematicBody2D
{
	private int _speed = 200;
	private int _jumpForce = 200;
	private int _gravity = 400;

	private bool _isDashing, _isDashAvailable, _isClimbing, _isClimbAvailable, _isWallJumping, _isInAir;

	private int _dashSpeed = 600;
	private float _dashTimer = .2f;
	private const float DashTimerReset = .2f;

	private float _climbTimer = 5f;
	private const float ClimbTimerReset = 5f;
	
	private float _wallJumpTimer = .4f;
	private const float WallJumpTimerReset = .4f;

	private int _climbSpeed = 200;

	private float _friction = .1f;
	private float _acceleration = .5f;

	private Vector2 _velocity;

	private RayCast2D _rayCastLeft, _rayCastRight;
	private RayCast2D _rayCastLeftClimb, _rayCastRightClimb;

	private AnimatedSprite animatedSprite;

	[Export] public PackedScene GhostPLayerInstance;

	public override void _Ready()
	{
		_rayCastLeft = GetNode<RayCast2D>("RayCastLeft");
		_rayCastRight = GetNode<RayCast2D>("RayCastRight");
		_rayCastLeftClimb = GetNode<RayCast2D>("RayCastLeftClimb");
		_rayCastRightClimb = GetNode<RayCast2D>("RayCastRightClimb");

		animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
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
				animatedSprite.Play("jump");
				_isInAir = true;
			}
			else
			{
				_isInAir = false;
			}

			_isClimbAvailable = true;
			_isDashAvailable = true;
		}
		else ProcessWallJump(delta);

		if (_isClimbAvailable && !_isWallJumping && Input.IsActionPressed("climb"))
		{
			ProcessClimb(delta);
		}
		else _isClimbing = false;

		if (_isDashAvailable && Input.IsActionJustPressed("dash"))
		{
			ProcessDash();
		}
		
		//States
		if (_isDashing)
		{
			_dashTimer -= delta;

			GhostPlayer ghost = GhostPLayerInstance.Instance() as GhostPlayer;

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
		else
		{
			_climbTimer -= delta;

			if (_climbTimer <= 0)
			{
				_isClimbing = false;
				_isClimbAvailable = false;

				_climbTimer = ClimbTimerReset;
			}
		}
		
		MoveAndSlide(_velocity, Vector2.Up);
	}

	private void ProcessMovement()
	{
		var direction = 0;

		if (Input.IsActionPressed("ui_left"))
		{
			direction -= 1;
			animatedSprite.FlipH = true;
		}

		if (Input.IsActionPressed("ui_right"))
		{
			direction += 1;
			animatedSprite.FlipH = false;
		}

		if (direction != 0)
		{
			_velocity.x = Mathf.Lerp(_velocity.x, direction * _speed, _acceleration);
			if (!_isInAir) animatedSprite.Play("run");
		}
		else
		{
			if (!_isInAir) animatedSprite.Play("idle");
			_velocity.x = Mathf.Lerp(_velocity.x, 0, _friction);
		}
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
		if (_rayCastLeftClimb.IsColliding() || _rayCastRightClimb.IsColliding() || _rayCastLeft.IsColliding()
		    || _rayCastRight.IsColliding())
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
		if (Input.IsActionJustPressed("jump") && _rayCastLeft.IsColliding())
		{
			_velocity.y = -_jumpForce;
			_velocity.x = _jumpForce;
			_isWallJumping = true;
			animatedSprite.FlipH = false;
		}
		else if (Input.IsActionJustPressed("jump") && _rayCastRight.IsColliding())
		{
			_velocity.y = -_jumpForce;
			_velocity.x = -_jumpForce;
			_isWallJumping = true;
			animatedSprite.FlipH = true;
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
