using Godot;
using Godot.Collections;
using System;
using System.Runtime.CompilerServices;

public partial class Door : StaticBody3D, IHandable
{
	


	[Signal] public delegate void RotationCompletedEventHandler();
	[Signal] public delegate void CounterRotationCompletedEventHandler();
	
	public bool IsOpen = false;
	public bool IsLocked { get; set; } = true;

	private Tween _rotateTween;
	private float _rotateAnimationSpeed = 0.5f;

	private Tween _popOpenTween;
	private float _popOpenAnimationSpeed = 0.3f;

	[Export] int rotDir = 1;

    public bool IsActive { get; set; }

    [Export] private Dictionary<HandType, Dictionary<HandType, NodePath>> _handInputTargets = new Dictionary<HandType, Dictionary<HandType, NodePath>>();

    public Dictionary<HandType, Dictionary<HandType, NodePath>> HandInputTargets { get { return _handInputTargets; } }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void SetActive(HandType inputHand, bool state)
    {
        IsActive = state;

        if (_handInputTargets.ContainsKey(inputHand))
        {
            //for every input hand that could go on this
            foreach (HandType controlledHand in _handInputTargets[inputHand].Keys)
            {
                //get all of the possible hands its controlling and set them correctly
                switch (controlledHand)
                {
                    case HandType.Mouse:
                        GameManager.Instance.HCont.mHandOverride = GetNode<Node3D>(_handInputTargets[inputHand][controlledHand]);
                        GameManager.Instance.HCont.mouseControl = !state;
                        break;

                    case HandType.KeyL:
                        GameManager.Instance.HCont.kLHandOverride = GetNode<Node3D>(_handInputTargets[inputHand][controlledHand]);
                        GameManager.Instance.HCont.keyboardControlL = !state;
                        if (state)
                        {
                            GameManager.Instance.HCont.kLHandVel = Vector2.Zero;

                        }
                        break;

                    case HandType.KeyR:
                        GameManager.Instance.HCont.kRHandOverride = GetNode<Node3D>(_handInputTargets[inputHand][controlledHand]);
                        GameManager.Instance.HCont.keyboardControlR = !state;
                        if (state)
                        {
                            GameManager.Instance.HCont.kRHandVel = Vector2.Zero;

                        }
                        break;

                    case HandType.ContL:
                        GameManager.Instance.HCont.cLHandOverride = GetNode<Node3D>(_handInputTargets[inputHand][controlledHand]);
                        GameManager.Instance.HCont.controllerControlL = !state;
                        if (state)
                        {
                            GameManager.Instance.HCont.cLHandVel = Vector2.Zero;

                        }
                        break;

                    case HandType.ContR:
                        GameManager.Instance.HCont.cRHandOverride = GetNode<Node3D>(_handInputTargets[inputHand][controlledHand]);
                        GameManager.Instance.HCont.controllerControlR = !state;
                        if (state)
                        {
                            GameManager.Instance.HCont.cRHandVel = Vector2.Zero;

                        }
                        break;
                }
            }

        }
    }

    public override void _Input(InputEvent @event)
	{
		if(!IsActive)
		{
			return;
		}

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

				_rotateTween.TweenProperty(this, "rotation", new Vector3(0, Mathf.DegToRad(117.2f), 0) * rotDir, _rotateAnimationSpeed);
				IsOpen = true;
			}
		}

		if (@event.IsActionPressed("mouse-scrollup"))
		{
			// If valve door is open, animate to close
			if (IsOpen)
			{
				if (_rotateTween != null && _rotateTween.IsRunning())
				{
					_rotateTween.Kill();
				}

				_rotateTween = CreateTween();

				_rotateTween.TweenProperty(this, "rotation", new Vector3(0, 0, 0), _rotateAnimationSpeed);
				IsOpen = false;
			}
		}
	}

	public void _on_valve_rotation_completed()
	{
		GD.Print("OPEN");

		// If valve door is already open, do nothing
		if (IsOpen) return;

        _popOpenTween = CreateTween();

        _popOpenTween.TweenProperty(this, "rotation", new Vector3(0, Mathf.DegToRad(20.0f), 0), _popOpenAnimationSpeed);

        // If valve door is locked, unlock it
        if (IsLocked) IsLocked = false;
	}

	public void _on_valve_counter_rotation_completed()
	{
		// Can't lock valve door if door is still open
		if (IsOpen) return;

		// If valve door is unlocked, lock it
		if (!IsLocked) IsLocked = true;
	}
}
