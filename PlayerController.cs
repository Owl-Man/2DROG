using Godot;

public class PlayerController : KinematicBody2D
{
	public int health = 3;
	private int _speed = 200;
	private int _jumpForce = 200;
	private int _gravity = 400;

	private bool _isDashing, _isDashAvailable, _isClimbing, _isClimbAvailable, _isWallJumping, _isInAir,
		_isTakingDamage;

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
	private int facingDirection = 0;

	private Vector2 _velocity;

	private RayCast2D _rayCastLeft, _rayCastRight;
	private RayCast2D _rayCastLeftClimb, _rayCastRightClimb;

	private AnimatedSprite animatedSprite;

	[Export] public PackedScene GhostPLayerInstance;

	public override void _Ready()
	{
		GD.Randomize();
		
		_rayCastLeft = GetNode<RayCast2D>("RayCastLeft");
		_rayCastRight = GetNode<RayCast2D>("RayCastRight");
		_rayCastLeftClimb = GetNode<RayCast2D>("RayCastLeftClimb");
		_rayCastRightClimb = GetNode<RayCast2D>("RayCastRightClimb");

		animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
	}

	public override void _Process(float delta)
	{
		if (health == 0) return;
		
		if (!_isDashing && !_isWallJumping)
		{
			ProcessMovement();
		}

		if (IsOnFloor())
		{
			if (Input.IsActionJustPressed("jump"))
			{
				_velocity.y = -_jumpForce;
				_isInAir = true;
				animatedSprite.Play("jump");
			}
			else
			{
				_isInAir = false;
			}

			_isClimbAvailable = true;
			_isDashAvailable = true;
		}
		else
		{
			ProcessWallJump(delta);
		}

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
			Owner.AddChild(ghost);
			ghost.GlobalPosition = this.GlobalPosition;
			ghost.GetNode<Sprite>("Sprite").FlipH = animatedSprite.FlipH;

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
		facingDirection = 0;

		if (!_isTakingDamage)
		{
			if (Input.IsActionPressed("ui_left"))
			{
				facingDirection -= 1;
				animatedSprite.FlipH = true;
			}

			if (Input.IsActionPressed("ui_right"))
			{
				facingDirection += 1;
				animatedSprite.FlipH = false;
			}
		}

		if (facingDirection != 0)
		{
			_velocity.x = Mathf.Lerp(_velocity.x, facingDirection * _speed, _acceleration);

			if (!_isInAir) animatedSprite.Play("run");
		}
		else
		{
			_velocity.x = Mathf.Lerp(_velocity.x, 0, _friction);
			
			if (_velocity.x < 5 && _velocity.x > -5)
			{
				if (!_isInAir) animatedSprite.Play("idle");
				
				_isTakingDamage = false;
			} 
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
			animatedSprite.Play("jump");
		}

		if (Input.IsActionPressed("ui_left") && Input.IsActionPressed("ui_up"))
		{
			_velocity.x = -_dashSpeed;
			_velocity.y = -_dashSpeed;
			animatedSprite.Play("jump");
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

	public void TakeDamage()
	{
		health -= 1;
		_velocity = MoveAndSlide(new Vector2(700 * -facingDirection, -80), Vector2.Up);
		_isTakingDamage = true;
		animatedSprite.Play("takeDamage");
		GD.Print(health);

		if (health <= 0)
		{
			health = 0;
			GD.Print("Player has die");
			animatedSprite.Play("death");
		}
	}

	private void _on_AnimatedSprite_animation_finished()
	{
		if (animatedSprite.Animation == "death")
		{
			GetTree().ReloadCurrentScene();
		}
	}
}
