using Godot;
using Godot.Collections;
using System;

public partial class left_valve_thruster: StaticBody3D, IHandable
{
	private float inputDeadzone = .2f; //you could tie this to the godot method but fuck that for this rn
	private Control _numpadUI;
	private SubViewport _subviewport;
	[Export] private Dictionary<HandType, Dictionary<HandType, NodePath>> _handInputTargets = new Dictionary<HandType, Dictionary<HandType, NodePath>>();
	public Dictionary<HandType, Dictionary<HandType, NodePath>> HandInputTargets { get { return _handInputTargets; } }

	public bool IsActive { get; set; } = false;

    [Export] BatteryReceptical br;
    [Export] Door door;
    public override void _Ready()
	{
		_numpadUI = GetNode<Control>("Screen/SubViewport/NumpadUI");
		_subviewport = GetNode<SubViewport>("Screen/SubViewport");
		_numpadUI.Connect("password_correct", Callable.From(OnPasswordSuccess));
        door.Connect(Door.SignalName.Opened, Callable.From(() => { br.SetAllowInsert(true); }));
        door.Connect(Door.SignalName.Closed, Callable.From(() => { br.SetAllowInsert(false); }));
    }
	
	private void OnPasswordSuccess()
	{
		GD.Print("password correct, unlock valve");
		var valveNode = GetNode<valve>("DoorPivot/valve");
		valveNode.IsLocked = false;
		_numpadUI.Call("hide_ui");
		SetActive(HandType.Mouse, false);
	}

	public override void _Input(InputEvent @event)
	{
		if (!IsActive)
			return;
		
		_subviewport.PushInput(@event);
		if (@event is InputEventJoypadMotion motionEvent)
		{
			if(Mathf.Abs(motionEvent.AxisValue) > inputDeadzone)
			{
				_numpadUI.Call("hide_ui");
				SetActive(HandType.Mouse, false);
			}
			return;
		}
		if ((@event is InputEventJoypadButton joyEvent && joyEvent.Pressed) ||
			(@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed))
		{
			_numpadUI.Call("hide_ui");
			SetActive(HandType.Mouse, false);
			return;
		}
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			Key code = keyEvent.Keycode;
			bool isNumpad = (code >= Key.Kp0 && code <= Key.Kp9) ||
				code == Key.KpEnter ||
				code == Key.KpPeriod;
			if (!isNumpad)
			{
				_numpadUI.Call("hide_ui");
				SetActive(HandType.Mouse, false);
				return;
			}
		}
	}

	public void OnClicked()
	{
		SetActive(HandType.Mouse, true);
		_numpadUI.Call("show_ui");
	}

	public void SetActive(HandType inputHand, bool state)
	{
		IsActive = state;

		if (state)
		{
			_numpadUI.Call("show_ui");
		}
		else
		{
			_numpadUI.Call("hide_ui");
		}

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
}
