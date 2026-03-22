using Godot;
using Godot.Collections;
using System;

public partial class valve : Node, IHandable
{
	[Signal] public delegate void RotationCompletedEventHandler();
	[Signal] public delegate void CounterRotationCompletedEventHandler();

	[Export] private int _numHalfRotsNeeded = 10;
	[Export] private bool _rot_cc = false; //if this rotates counter clockwise, instead of clockwise
	[Export] private bool _is_right_stick = false; //if this uses the right stick, instead of the left stick
	public bool IsLocked = true;
	private bool hasOpened = false;

	private int _halfRotsCompleted = 0;

	private Vector2 _inputVec = Vector2.Zero;
	private Vector2 _lastInputVec = Vector2.Zero;
	private float _lastInputAngle = 0;
	private bool _lastCrossoverLeft = false;

    public bool IsActive { get; set; }

    [Export] private Dictionary<HandType, Dictionary<HandType, NodePath>> _handInputTargets = new Dictionary<HandType, Dictionary<HandType, NodePath>>();

    public Dictionary<HandType, Dictionary<HandType, NodePath>> HandInputTargets { get { return _handInputTargets; } }


    private float _deadzone = .3f;

	[Export] public MeshInstance3D _valveDisplay;

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
		if(!IsActive || IsLocked)
		{
            return;
        }
			
		if(@event is not InputEventJoypadMotion)
		{
			return; //we ONLY care about joypad motion for valves
		}

		InputEventJoypadMotion eventMot = @event as InputEventJoypadMotion;
		float str = eventMot.AxisValue;

		if(Mathf.Abs(str) <= _deadzone )
		{
			return;
		}

		_lastInputAngle = Mathf.RadToDeg(Mathf.Atan2(_inputVec.Y, _inputVec.X));
		_lastInputVec = _inputVec;

		if (!_is_right_stick)
		{
			if(eventMot.Axis == JoyAxis.LeftX )
			{
				_inputVec.X = str;
			}
			else if(eventMot.Axis == JoyAxis.LeftY)
			{
				_inputVec.Y = str;
			}
		} else
		{
			if (eventMot.Axis == JoyAxis.RightX)
			{
				_inputVec.X = str;
			}
			else if (eventMot.Axis == JoyAxis.RightY)
			{
				_inputVec.Y = str;
			}
		}

		_inputVec = _inputVec.Normalized();
		float inputAngle = Mathf.RadToDeg(Mathf.Atan2(_inputVec.Y, _inputVec.X));

		_valveDisplay.RotationDegrees = new Vector3(_valveDisplay.RotationDegrees.X, -inputAngle, _valveDisplay.RotationDegrees.Z);


		//Check input valid
		if(Mathf.Abs(inputAngle) < 90 && Mathf.Abs(_lastInputAngle) < 90 && _lastCrossoverLeft)
		{
			//both right
			if((!_rot_cc && inputAngle > 0 && _lastInputAngle < 0 ) || (_rot_cc && inputAngle < 0 && _lastInputAngle > 0))
			{
				_lastCrossoverLeft = false;
				_halfRotsCompleted = Math.Min(_halfRotsCompleted + 1, 10);
			}
			//going back
			if ((_rot_cc && inputAngle > 0 && _lastInputAngle < 0) || (!_rot_cc && inputAngle < 0 && _lastInputAngle > 0))
			{
				_lastCrossoverLeft = false;
				_halfRotsCompleted = Math.Max(_halfRotsCompleted - 1, 0);
			}
			CheckRotationSignals();
		} 
		else if (Mathf.Abs(inputAngle) > 90 && Mathf.Abs(_lastInputAngle) > 90 && !_lastCrossoverLeft)
		{
			//both left
			if ((_rot_cc && inputAngle > 0 && _lastInputAngle < 0) || (!_rot_cc && inputAngle < 0 && _lastInputAngle > 0))
			{
				_lastCrossoverLeft = true;
				_halfRotsCompleted = Math.Min(_halfRotsCompleted + 1, 10);
			}
			//going back
			if ((!_rot_cc && inputAngle > 0 && _lastInputAngle < 0) || (_rot_cc && inputAngle < 0 && _lastInputAngle > 0))
			{
				_lastCrossoverLeft = true;
				_halfRotsCompleted = Math.Max(_halfRotsCompleted - 1, 0);
			}
			CheckRotationSignals();

		}

	}

	public void CheckRotationSignals()
	{
		if( _halfRotsCompleted == _numHalfRotsNeeded )
		{
			hasOpened = true;
			EmitSignal(SignalName.RotationCompleted);
            SetActive(HandType.Mouse, false);
            SetActive(HandType.KeyL, false);
            SetActive(HandType.KeyR, false);
            SetActive(HandType.ContL, false);
            SetActive(HandType.ContR, false);

			_halfRotsCompleted = _numHalfRotsNeeded / 2;
        }
		else if(_halfRotsCompleted == 0)
		{
			CheckRelock();
			EmitSignal(SignalName.CounterRotationCompleted);
            _halfRotsCompleted = _numHalfRotsNeeded / 2;
        }
	}

	public void CheckRelock()
	{
		if(hasOpened)
		{
			IsLocked = true;
			hasOpened = false;
            SetActive(HandType.Mouse, false);
            SetActive(HandType.KeyL,  false);
            SetActive(HandType.KeyR,  false);
            SetActive(HandType.ContL, false);
            SetActive(HandType.ContR, false);
		}
	}


}
