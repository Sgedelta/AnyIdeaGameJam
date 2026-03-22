using Godot;
using Godot.Collections;
using System;

public partial class Computer : StaticBody3D, IHandable
{
	private TypingUiContainer _typingUI;
	private SubViewport _subViewport;
	private float inputDeadzone = .2f; //you could tie this to the godot method but fuck that for this rn

	public bool IsActive { get; set; }

	[Export] private Dictionary<HandType, Dictionary<HandType, NodePath>> _handInputTargets = new Dictionary<HandType, Dictionary<HandType, NodePath>>();

	public Dictionary<HandType, Dictionary<HandType, NodePath>> HandInputTargets { get { return _handInputTargets; } }



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_typingUI = GetNode<TypingUiContainer>("Screen/SubViewport/TypingUIContainer");
		_subViewport = GetNode<SubViewport>("Screen/SubViewport");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	public override void _Input(InputEvent @event)
	{
		if(!IsActive)
		{
			return;
		}

		if(@event is not (InputEventKey or InputEventMouseMotion) && (@event is InputEventJoypadMotion && Math.Abs((@event as InputEventJoypadMotion).AxisValue) > inputDeadzone))
		{
			GD.Print(@event);
			if(@event is InputEventJoypadMotion)
			{
				GD.Print((@event as InputEventJoypadMotion).AxisValue);
			}
			_typingUI.FocusTyping(false);
		}

		_subViewport.PushInput(@event);
		
	}

	public void SetActive(HandType inputHand, bool state)
	{
        GD.Print(inputHand + " " + state);
		IsActive = state;

		//computer only
		_typingUI.FocusTyping(state);

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

	public void CheckFocus(InputEventMouseButton e)
	{
		PhysicsDirectSpaceState3D spState = GetWorld3D().DirectSpaceState;
		Vector2 mPos = e.Position;
		Camera3D cam = GetViewport().GetCamera3D();

		var origin = cam.ProjectRayOrigin(mPos);
		var end = origin + cam.ProjectRayNormal(mPos) * 100;
		var query = PhysicsRayQueryParameters3D.Create(origin, end);

		var result = spState.IntersectRay(query);

		if(result.ContainsKey("collider_id") && ((StaticBody3D)result["collider"]) == this)
		{
			GD.Print("trying to grab control");
			_typingUI.FocusTyping(true);
		}

	}

}
