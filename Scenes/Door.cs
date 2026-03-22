using Godot;
using System;

public partial class Door : Node
{
	[Signal] public delegate void RotationCompletedEventHandler();
	[Signal] public delegate void CounterRotationCompletedEventHandler();
	
	private bool _isOpen = false;
	public bool IsLocked { get; set; } = true;

	private Tween _rotateTween;
	private float _rotateAnimationSpeed = 0.5f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("mouse-scrolldown"))
		{
			// If valve door is unlocked, animate to open
			if (!IsLocked)
			{
				if(_rotateTween != null && _rotateTween.IsRunning())
				{
					_rotateTween.Kill();
				}

				_rotateTween = CreateTween();

				_rotateTween.TweenProperty(this, "rotation", new Vector3(0, Mathf.DegToRad(117.2f), 0), _rotateAnimationSpeed);
				_isOpen = true;
			}
		}

		if (@event.IsActionPressed("mouse-scrollup"))
		{
			// If valve door is open, animate to close
			if (_isOpen)
			{
				if (_rotateTween != null && _rotateTween.IsRunning())
				{
					_rotateTween.Kill();
				}

				_rotateTween = CreateTween();

				_rotateTween.TweenProperty(this, "rotation", new Vector3(0, 0, 0), _rotateAnimationSpeed);
				_isOpen = false;
			}
		}
	}

	public void _on_valve_rotation_completed()
	{
		// If valve door is already open, do nothing
		if (_isOpen) return;

		// If valve door is locked, unlock it
		if (IsLocked) IsLocked = false;
	}

	public void _on_valve_counter_rotation_completed()
	{
		// Can't lock valve door if door is still open
		if (_isOpen) return;

		// If valve door is unlocked, lock it
		if (!IsLocked) IsLocked = true;
	}
}
