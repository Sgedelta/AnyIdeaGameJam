using Godot;
using Godot.Collections;
using System;
using static Godot.XRHandTracker;

public partial class ButtonObject : StaticBody3D, IHandable
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


    //IHandable things
    public bool IsActive { get; set; }
    [Export] private Dictionary<HandType, Dictionary<HandType, NodePath>> _handInputTargets = new Dictionary<HandType, Dictionary<HandType, NodePath>>();

    public Dictionary<HandType, Dictionary<HandType, NodePath>> HandInputTargets { get { return _handInputTargets; } }


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
        if (!IsActive)
        {
            return;
        }

        //if (@event.IsAction("mouse_left") && @event.IsActionPressed("mouse_left"))
        //{
        //    CheckFocus(@event as InputEventMouseButton);
        //}

    }

    public void SetActive(HandType inputHand, bool state)
    {
        IsActive = state;

        if (state)
        {
            UpdateCameraTween();
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



    public void CheckFocus(InputEventMouseButton e)
    {
        PhysicsDirectSpaceState3D spState = GetWorld3D().DirectSpaceState;
        Vector2 mPos = e.Position;
        Camera3D cam = GetViewport().GetCamera3D();

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
