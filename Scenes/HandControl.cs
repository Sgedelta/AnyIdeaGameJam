using Godot;
using System;
using System.Diagnostics;
using static Godot.HttpRequest;

public partial class HandControl : Node2D
{

	[ExportGroup("Hand and Target")]
	[Export] private Node2D mHand;
    [Export] private Node2D kHandL;
    [Export] private Node2D kHandR;
    [Export] float kHandSpeed = 300;
    [Export] private Node2D cHandL;
    [Export] private Node2D cHandR;
    [Export] float cHandSpeed = 300;
    [Export] private Node3D mHandTarget;
    [Export] private float mHandLength = 1.5f;
    [Export] private Node3D kHandLTarget;
    [Export] private Node3D kHandRTarget;
    [Export] private float kHandLength = 1.5f;
    [Export] private Node3D cHandLTarget;
    [Export] private Node3D cHandRTarget;
    [Export] private float cHandLength = 1.5f;

    private Vector2 kLHandVel = Vector2.Zero;
    private Vector2 kRHandVel = Vector2.Zero;
    private Vector2 cLHandVel = Vector2.Zero;
    private Vector2 cRHandVel = Vector2.Zero;

    private IHandable mHandable;

    public Node3D mHandOverride;
    public Node3D kLHandOverride;
    public Node3D kRHandOverride;
    public Node3D cLHandOverride;
    public Node3D cRHandOverride;

    public bool mouseControl = true;
    public bool keyboardControlL = true;
    public bool keyboardControlR = true;
    public bool controllerControlL = true;
    public bool controllerControlR = true;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{

        GameManager.Instance.HCont = this;
	}


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        kHandL.Position += kLHandVel * kHandSpeed * (float)delta;
        kHandR.Position += kRHandVel * kHandSpeed * (float)delta;

        cHandL.Position += cLHandVel * cHandSpeed * (float)delta;
        cHandR.Position += cRHandVel * cHandSpeed * (float)delta;

        CastTargetsIntoWorld();
        if (!mouseControl)
        {
            mHandTarget.Position = mHandOverride.GlobalPosition;
        }
        if (!keyboardControlL)
        {
            kHandLTarget.Position = kLHandOverride.GlobalPosition;
        }
        if (!keyboardControlR)
        {
            kHandRTarget.Position = kRHandOverride.GlobalPosition;
        }
        if(!controllerControlL)
        {
            cHandLTarget.Position = cLHandOverride.GlobalPosition;
        }
        if (!controllerControlR)
        {
            cHandRTarget.Position = cRHandOverride.GlobalPosition;
        }

    }


    public override void _Input(InputEvent @event)
    {
        //TODO: update input to call other CastChecks correctly
        Vector2 inControl = Vector2.Zero;
        switch (@event)
        {
            case InputEventMouseButton:
                InputEventMouseButton me = @event as InputEventMouseButton;

                if (me.ButtonIndex == MouseButton.Left )
                {
                    MouseCastCheck(me.Pressed);
                }


                break;

            case InputEventMouseMotion:

                if (!mouseControl)
                {
                    return;
                }
                mHand.Position = (@event as InputEventMouseMotion).Position;
                break;

            case InputEventKey:
                InputEventKey ke = @event as InputEventKey;


                if (ke.IsEcho())
                {
                    return;
                }

                //TODO: probably put grab check here

                switch (ke.Keycode)
                {
                    case Key.W:
                        if (!keyboardControlL)
                        {
                            return;
                        }
                        inControl = Vector2.Up;
                        break;
                    case Key.Up:
                        if (!keyboardControlR)
                        {
                            return;
                        }
                        inControl = Vector2.Up;
                        break;

                    case Key.S:
                        if (!keyboardControlL)
                        {
                            return;
                        }
                        inControl = Vector2.Down;
                        break;
                    case Key.Down:
                        if (!keyboardControlR)
                        {
                            return;
                        }
                        inControl = Vector2.Down;
                        break;

                    case Key.A:
                        if (!keyboardControlL)
                        {
                            return;
                        }
                        inControl = Vector2.Left;
                        break;
                    case Key.Left:
                        if (!keyboardControlR)
                        {
                            return;
                        }
                        inControl = Vector2.Left;
                        break;

                    case Key.D:
                        if (!keyboardControlL)
                        {
                            return;
                        }
                        inControl = Vector2.Right;
                        break;
                    case Key.Right:
                        if (!keyboardControlR)
                        {
                            return;
                        }
                        inControl = Vector2.Right;
                        break;


                }

                if (ke.IsReleased())
                {
                    inControl *= -1; //do the opposite
                }

                if (ke.Keycode == Key.W || ke.Keycode == Key.A || ke.Keycode == Key.S || ke.Keycode == Key.D)
                {
                    kLHandVel += inControl;
                }
                else if (ke.Keycode == Key.Up || ke.Keycode == Key.Left || ke.Keycode == Key.Down || ke.Keycode == Key.Right)
                {
                    kRHandVel += inControl;
                }


                break;

            case InputEventJoypadButton:
                //TODO: fill out like mouse button
                break;

            case InputEventJoypadMotion:
                InputEventJoypadMotion je = @event as InputEventJoypadMotion;

                if ((je.Axis == JoyAxis.LeftX || je.Axis == JoyAxis.LeftY) && !keyboardControlL ||
                    (je.Axis == JoyAxis.RightX || je.Axis == JoyAxis.RightY) && !keyboardControlR)
                {
                    return;
                }
                

                if(je.Axis == JoyAxis.LeftX || je.Axis == JoyAxis.RightX)
                {
                    inControl.X = je.AxisValue;
                }
                if (je.Axis == JoyAxis.LeftY || je.Axis == JoyAxis.RightY)
                {
                    inControl.Y = je.AxisValue;
                }



                switch(je.Axis)
                {
                    case JoyAxis.LeftY:
                        cLHandVel.Y = inControl.Y;
                        break;

                    case JoyAxis.LeftX:
                        cLHandVel.X = inControl.X;
                        break;

                    case JoyAxis.RightY:
                        cRHandVel.Y = inControl.Y;
                        break;

                    case JoyAxis.RightX:
                        cRHandVel.X = inControl.X;
                        break;
                }


                break;
        }
        
    }

    private void CastTargetsIntoWorld()
    {
        Camera3D cam = GetViewport().GetCamera3D();
        PhysicsDirectSpaceState3D spState = cam.GetWorld3D().DirectSpaceState;

        var mHandOrigin = cam.ProjectRayOrigin(mHand.Position);
        var mHandEnd = mHandOrigin + cam.ProjectRayNormal(mHand.Position) * mHandLength;
        var mHandQuery = PhysicsRayQueryParameters3D.Create(mHandOrigin, mHandEnd);

        var kLHandOrigin = cam.ProjectRayOrigin(kHandL.Position);
        var kLHandEnd = kLHandOrigin + cam.ProjectRayNormal(kHandL.Position) * kHandLength;
        var kLHandQuery = PhysicsRayQueryParameters3D.Create(kLHandOrigin, kLHandEnd);

        var kRHandOrigin = cam.ProjectRayOrigin(kHandR.Position);
        var kRHandEnd = kRHandOrigin + cam.ProjectRayNormal(kHandR.Position) * kHandLength;
        var kRHandQuery = PhysicsRayQueryParameters3D.Create(kRHandOrigin, kRHandEnd);

        var cLHandOrigin = cam.ProjectRayOrigin(cHandL.Position);
        var cLHandEnd = cLHandOrigin + cam.ProjectRayNormal(cHandL.Position) * cHandLength;
        var cLHandQuery = PhysicsRayQueryParameters3D.Create(cLHandOrigin, cLHandEnd);

        var cRHandOrigin = cam.ProjectRayOrigin(cHandR.Position);
        var cRHandEnd = cRHandOrigin + cam.ProjectRayNormal(cHandR.Position) * cHandLength;
        var cRHandQuery = PhysicsRayQueryParameters3D.Create(cRHandOrigin, cRHandEnd);

        var result = spState.IntersectRay(mHandQuery);

        mHandTarget.Position = result.Count > 0 ? (Vector3)result["position"] : mHandEnd;

        result = spState.IntersectRay(kLHandQuery);

        kHandLTarget.Position = result.Count > 0 ? (Vector3)result["position"] : kLHandEnd;

        result = spState.IntersectRay(kRHandQuery);

        kHandRTarget.Position = result.Count > 0 ? (Vector3)result["position"] : kRHandEnd;

        result = spState.IntersectRay(cLHandQuery);

        cHandLTarget.Position = result.Count > 0 ? (Vector3)result["position"] : cLHandEnd;

        result = spState.IntersectRay(cRHandQuery);

        cHandRTarget.Position = result.Count > 0 ? (Vector3)result["position"] : cRHandEnd;


    }

    //TODO: make other cast checks like this guy
    public void MouseCastCheck(bool state)
    {
        if(!state && mHandable != null)
        {
            mHandable.SetActive(false);
        }


        Camera3D cam = GetViewport().GetCamera3D();
        PhysicsDirectSpaceState3D spState = cam.GetWorld3D().DirectSpaceState;

        var mHandOrigin = cam.ProjectRayOrigin(mHand.Position);
        var mHandEnd = mHandOrigin + cam.ProjectRayNormal(mHand.Position) * mHandLength;
        var mHandQuery = PhysicsRayQueryParameters3D.Create(mHandOrigin, mHandEnd);

        var result = spState.IntersectRay(mHandQuery);

        if(result.Count == 0)
        {
            //nothing found
            return;
        }

        var other = (Node)result["collider"];

        if(other is not IHandable)
        {
            return; 
        }
        mHandable = (IHandable)other;

        if(mHandable.HandInputs.Contains(HandType.Mouse))
        {
            mHandable.SetActive(state);
        }

    }

    public void KeyLeftCastCheck()
    {
        Camera3D cam = GetViewport().GetCamera3D();
        PhysicsDirectSpaceState3D spState = cam.GetWorld3D().DirectSpaceState;

        var kLHandOrigin = cam.ProjectRayOrigin(kHandL.Position);
        var kLHandEnd = kLHandOrigin + cam.ProjectRayNormal(kHandL.Position) * kHandLength;
        var kLHandQuery = PhysicsRayQueryParameters3D.Create(kLHandOrigin, kLHandEnd);

        var result = spState.IntersectRay(kLHandQuery);

        if (result.Count == 0)
        {
            //nothing found
            return;
        }

        var other = (Node)result["collider"];

        if (other is not IHandable)
        {
            return;
        }
    }

    public void KeyRightCastCheck()
    {
        Camera3D cam = GetViewport().GetCamera3D();
        PhysicsDirectSpaceState3D spState = cam.GetWorld3D().DirectSpaceState;

        var kRHandOrigin = cam.ProjectRayOrigin(kHandR.Position);
        var kRHandEnd = kRHandOrigin + cam.ProjectRayNormal(kHandR.Position) * kHandLength;
        var kRHandQuery = PhysicsRayQueryParameters3D.Create(kRHandOrigin, kRHandEnd);

        var result = spState.IntersectRay(kRHandQuery);

        if (result.Count == 0)
        {
            //nothing found
            return;
        }

        var other = (Node)result["collider"];

        if (other is not IHandable)
        {
            return;
        }
    }

    public void ContLeftCastCheck()
    {
        Camera3D cam = GetViewport().GetCamera3D();
        PhysicsDirectSpaceState3D spState = cam.GetWorld3D().DirectSpaceState;

        var cLHandOrigin = cam.ProjectRayOrigin(cHandL.Position);
        var cLHandEnd = cLHandOrigin + cam.ProjectRayNormal(cHandL.Position) * cHandLength;
        var cLHandQuery = PhysicsRayQueryParameters3D.Create(cLHandOrigin, cLHandEnd);

        var result = spState.IntersectRay(cLHandQuery);

        if (result.Count == 0)
        {
            //nothing found
            return;
        }

        var other = (Node)result["collider"];

        if (other is not IHandable)
        {
            return;
        }
    }

    public void ContRightCastCheck()
    {
        Camera3D cam = GetViewport().GetCamera3D();
        PhysicsDirectSpaceState3D spState = cam.GetWorld3D().DirectSpaceState;

        var cRHandOrigin = cam.ProjectRayOrigin(cHandR.Position);
        var cRHandEnd = cRHandOrigin + cam.ProjectRayNormal(cHandR.Position) * cHandLength;
        var cRHandQuery = PhysicsRayQueryParameters3D.Create(cRHandOrigin, cRHandEnd);

        var result = spState.IntersectRay(cRHandQuery);

        if (result.Count == 0)
        {
            //nothing found
            return;
        }

        var other = (Node)result["collider"];

        if (other is not IHandable)
        {
            return;
        }
    }

}
