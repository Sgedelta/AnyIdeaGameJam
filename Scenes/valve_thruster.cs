using Godot;
using Godot.Collections;
using System;

public partial class valve_thruster: StaticBody3D, IHandable
{
	private float inputDeadzone = .2f; //you could tie this to the godot method but fuck that for this rn
	private Control _arrowsUI;
	private SubViewport _subviewport;
	[Export] private Dictionary<HandType, Dictionary<HandType, NodePath>> _handInputTargets = new Dictionary<HandType, Dictionary<HandType, NodePath>>();
	public Dictionary<HandType, Dictionary<HandType, NodePath>> HandInputTargets { get { return _handInputTargets; } }

	public bool IsActive { get; set; } = false;

	public override void _Ready()
	{
		_arrowsUI = GetNode<Control>("Screen/SubViewport/ArrowsUI");
		_subviewport = GetNode<SubViewport>("Screen/SubViewport");
	}

	public override void _Input(InputEvent @event)
	{
		if (!IsActive)
			return;
		
		_subviewport.PushInput(@event);
		
		if (@event is InputEventJoypadButton joyEvent && joyEvent.Pressed)
		{
			switch ((JoyButton)joyEvent.ButtonIndex)
			{
				case JoyButton.DpadUp:
				case JoyButton.DpadDown:
				case JoyButton.DpadLeft:
				case JoyButton.DpadRight:
					return;
			};
		}
		if((@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed) ||
			(@event is InputEventKey keyEvent && keyEvent.Pressed))
		{
			if(_arrowsUI.HasMethod("hide_ui"))
				_arrowsUI.Call("hide_ui");
			IsActive = false;
		}
	}

	public void OnClicked()
	{
		IsActive = true;
		if(_arrowsUI.HasMethod("show_ui"))
			_arrowsUI.Call("show_ui");
	}

	public void SetActive(HandType inputHand, bool state)
	{
		IsActive = state;

		if (_handInputTargets.ContainsKey(inputHand))
		{
			//for every input hand that could go on this
			foreach(HandType controlledHand in _handInputTargets[inputHand].Keys)
			{
				//get all of the possible hands its controlling and set them correctly
				switch(controlledHand)
				{
					case HandType.Mouse:
						GameManager.Instance.HCont.mHandOverride = GetNode<Node3D>(_handInputTargets[inputHand][controlledHand]);
						GameManager.Instance.HCont.mouseControl = !state;
						break;
				
					case HandType.KeyL:
						GameManager.Instance.HCont.kLHandOverride = GetNode<Node3D>(_handInputTargets[inputHand][controlledHand]);
						GameManager.Instance.HCont.keyboardControlL = !state;
						if(state)
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
