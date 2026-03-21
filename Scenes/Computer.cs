using Godot;
using System;

public partial class Computer : StaticBody3D
{
	private TypingUiContainer _typingUI;
	private SubViewport _subViewport;


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
        if(@event.IsAction("mouse_left") && @event.IsActionReleased("mouse_left"))
		{
			CheckFocus((@event as InputEventMouseButton));
		}
		else if(@event is not (InputEventKey or InputEventMouseMotion))
		{
			GD.Print("UNFOCUS");
			_typingUI.FocusTyping(false);
		}

		_subViewport.PushInput(@event);
		
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
