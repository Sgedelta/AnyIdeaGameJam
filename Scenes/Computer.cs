using Godot;
using Godot.Collections;
using System;

public partial class Computer : StaticBody3D, IHandable
{
	private TypingUiContainer _typingUI;
	private SubViewport _subViewport;
	private float inputDeadzone = .2f; //you could tie this to the godot method but fuck that for this rn

    public bool IsActive { get; set; }

	[Export] private Dictionary<HandType, Node3D> _handTargets = new Dictionary<HandType, Node3D>();

	[Export] private Array<HandType> _handInputs = new Array<HandType>();

    public Dictionary<HandType, Node3D> HandTargets { get { return _handTargets; } }

	public Array<HandType> HandInputs { get { return _handInputs; } }


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

        if(@event.IsAction("mouse_left") && @event.IsActionPressed("mouse_left"))
		{
			CheckFocus((@event as InputEventMouseButton));
		}
		else if(@event is not (InputEventKey or InputEventMouseMotion) && (@event is InputEventJoypadMotion && Math.Abs((@event as InputEventJoypadMotion).AxisValue) > inputDeadzone))
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

	public void SetActive(bool state)
	{
		IsActive = state;

		foreach(HandType t in _handTargets.Keys)
		{
			switch(t)
			{
				case HandType.Mouse:
					GameManager.Instance.HCont.mHandOverride = _handTargets[t];
					GameManager.Instance.HCont.mouseControl = !state;
					break;

                case HandType.KeyL:
                    GameManager.Instance.HCont.kLHandOverride = _handTargets[t];
                    GameManager.Instance.HCont.keyboardControlL = !state;
                    break;

                case HandType.KeyR:
                    GameManager.Instance.HCont.kRHandOverride = _handTargets[t];
                    GameManager.Instance.HCont.keyboardControlR = !state;
                    break;

                case HandType.ContL:
                    GameManager.Instance.HCont.cLHandOverride = _handTargets[t];
                    GameManager.Instance.HCont.controllerControlL = !state;
                    break;

                case HandType.ContR:
                    GameManager.Instance.HCont.cRHandOverride = _handTargets[t];
                    GameManager.Instance.HCont.controllerControlR = !state;
                    break;
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
