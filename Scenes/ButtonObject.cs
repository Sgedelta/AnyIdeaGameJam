using Godot;
using Godot.Collections;
using System;

public partial class ButtonObject : StaticBody3D
{

	Color onColor = new Color (1f,0,0);
	Color offColor = new Color (0,0,0.3f);
    [Export]
    bool goingRight = true;    //this is whether the button goes left or right

    CameraController camera;

    private string[] directions = ["forward", "right", "backward", "left"];
    int directionIndex = 0;
	bool isOn = false;

    Array<Node> buttons = new Array<Node> ();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        buttons = GetTree().GetNodesInGroup("cameraButtons");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _Input(InputEvent @event)
    {
        if (@event.IsAction("mouse_left") && @event.IsActionPressed("mouse_left"))
        {
            CheckFocus(@event as InputEventMouseButton);
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

        if (result.ContainsKey("collider_id") && ((StaticBody3D)result["collider"]) == this)
        {
            UpdateCameraTween();
        }

    }

    public void UpdateCameraTween()
    {
        camera = (CameraController)GetViewport().GetCamera3D();

        if (camera != null && !camera.tweenIsRunning)
        {
            directionIndex = goingRight ? directionIndex + 1 : directionIndex - 1;
            foreach (var button in buttons)
            {
                ButtonObject buttonObject = button as ButtonObject;
                buttonObject.UpdateDirectionIndex(directionIndex);
            }
            if (directionIndex < 0)
            {
                directionIndex = directions.Length - 1;
            }
            if (directionIndex >= directions.Length)
            {
                directionIndex = 0;
            }
            camera.TweenCameraToLoc(directions[directionIndex]);
        }
    }

    public void UpdateDirectionIndex(int targetIndex)
    {
        if (directionIndex != targetIndex)
        {
            directionIndex = targetIndex;
        }
    }
}


