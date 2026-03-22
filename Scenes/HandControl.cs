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
    [Export] private Node3D kHandLTarget;
    [Export] private Node3D kHandRTarget;
    [Export] private Node3D cHandLTarget;
    [Export] private Node3D cHandRTarget;

    [ExportGroup("Hand Extend Length")]
    [Export] public Vector2 HandExtents = new Vector2(1, 5);

    public float mHandLength = 1f;
    public float kLHandLength = 1f;
    public float cLHandLength = 1f;
    public float kRHandLength = 1f;
    public float cRHandLength = 1f;


    public Vector2 kLHandVel = Vector2.Zero;
    public Vector2 kRHandVel = Vector2.Zero;
    public Vector2 cLHandVel = Vector2.Zero;
    public Vector2 cRHandVel = Vector2.Zero;

    private IHandable mHandable;
    private IHandable kLHandable;
    private IHandable kRHandable;
    private IHandable cLHandable;
    private IHandable cRHandable;

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
                else if(me.ButtonIndex == MouseButton.Right)
                {
                    SetHandExtent(HandType.Mouse, me.Pressed ? 1 : 0);
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

                //these *could* go in switch but... naw

                if(ke.Keycode == Key.Ctrl)
                {
                    SetHandExtent(
                        ke.Location == KeyLocation.Left ? HandType.KeyL : HandType.KeyR,
                        ke.Pressed ? 1 : 0
                        );
                }
                else if(ke.Keycode == Key.Shift)
                {
                    GD.Print($"{ke.Keycode} | {ke.Location}");
                    if(ke.Location == KeyLocation.Left)
                    {
                        KeyLeftCastCheck(ke.Pressed);
                    }
                    else
                    {
                        KeyRightCastCheck(ke.Pressed);
                    }
                }

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
                InputEventJoypadButton jbe = @event as InputEventJoypadButton;

                if (jbe.ButtonIndex == JoyButton.LeftShoulder)
                {
                    ContLeftCastCheck(jbe.Pressed);
                }
                if(jbe.ButtonIndex == JoyButton.RightShoulder)
                {
                    ContRightCastCheck(jbe.Pressed);
                }

                break;

            case InputEventJoypadMotion:
                InputEventJoypadMotion je = @event as InputEventJoypadMotion;

                if(je.Axis == JoyAxis.TriggerLeft)
                {
                    SetHandExtent(HandType.ContL, je.AxisValue);
                } 
                else if(je.Axis == JoyAxis.TriggerRight)
                {
                    SetHandExtent(HandType.ContR, je.AxisValue);
                }

                if ((je.Axis == JoyAxis.LeftX || je.Axis == JoyAxis.LeftY) && !controllerControlL ||
                    (je.Axis == JoyAxis.RightX || je.Axis == JoyAxis.RightY) && !controllerControlR)
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

        var mHandOrigin = cam.GlobalPosition;//cam.ProjectRayOrigin(mHand.Position);
        var mHandEnd = mHandOrigin + cam.ProjectRayNormal(mHand.Position) * mHandLength;
        var mHandQuery = PhysicsRayQueryParameters3D.Create(mHandOrigin, mHandEnd);

        var kLHandOrigin = cam.GlobalPosition;//cam.ProjectRayOrigin(kHandL.Position);
        var kLHandEnd = kLHandOrigin + cam.ProjectRayNormal(kHandL.Position) * kLHandLength;
        var kLHandQuery = PhysicsRayQueryParameters3D.Create(kLHandOrigin, kLHandEnd);

        var kRHandOrigin = cam.GlobalPosition;//cam.ProjectRayOrigin(kHandR.Position);
        var kRHandEnd = kRHandOrigin + cam.ProjectRayNormal(kHandR.Position) * kRHandLength;
        var kRHandQuery = PhysicsRayQueryParameters3D.Create(kRHandOrigin, kRHandEnd);

        var cLHandOrigin = cam.GlobalPosition;//cam.ProjectRayOrigin(cHandL.Position);
        var cLHandEnd = cLHandOrigin + cam.ProjectRayNormal(cHandL.Position) * cLHandLength;
        var cLHandQuery = PhysicsRayQueryParameters3D.Create(cLHandOrigin, cLHandEnd);

        var cRHandOrigin = cam.GlobalPosition;//cam.ProjectRayOrigin(cHandR.Position);
        var cRHandEnd = cRHandOrigin + cam.ProjectRayNormal(cHandR.Position) * cRHandLength;
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
        if(!state && mHandable != null && mHandable.IsActive)
        {
            mHandable.SetActive(HandType.Mouse, false);
            return;
        }


        Camera3D cam = GetViewport().GetCamera3D();
        PhysicsDirectSpaceState3D spState = cam.GetWorld3D().DirectSpaceState;

        var mHandOrigin = cam.GlobalPosition;//cam.ProjectRayOrigin(mHand.Position);
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

        if(mHandable.HandInputTargets.Keys.Contains(HandType.Mouse))
        {
            mHandable.SetActive(HandType.Mouse, state);
        }

    }

    public void KeyLeftCastCheck(bool state)
    {

        if (!state && kLHandable != null && kLHandable.IsActive)
        {
            kLHandable.SetActive(HandType.KeyL, false);
            return;
        }

        Camera3D cam = GetViewport().GetCamera3D();
        PhysicsDirectSpaceState3D spState = cam.GetWorld3D().DirectSpaceState;

        var kLHandOrigin = cam.GlobalPosition;//cam.ProjectRayOrigin(kHandL.Position);
        var kLHandEnd = kLHandOrigin + cam.ProjectRayNormal(kHandL.Position) * kLHandLength;
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

        kLHandable = (IHandable)other;

        if (kLHandable.HandInputTargets.Keys.Contains(HandType.KeyL))
        {
            kLHandable.SetActive(HandType.KeyL, state);
        }
    }

    public void KeyRightCastCheck(bool state)
    {
        if (!state && kRHandable != null && kRHandable.IsActive)
        {
            kRHandable.SetActive(HandType.KeyR, false);
            return;
        }

        Camera3D cam = GetViewport().GetCamera3D();
        PhysicsDirectSpaceState3D spState = cam.GetWorld3D().DirectSpaceState;

        var kRHandOrigin = cam.GlobalPosition;//cam.ProjectRayOrigin(kHandR.Position);
        var kRHandEnd = kRHandOrigin + cam.ProjectRayNormal(kHandR.Position) * kRHandLength;
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

        kRHandable = (IHandable)other;

        if (kRHandable.HandInputTargets.Keys.Contains(HandType.KeyR))
        {
            kRHandable.SetActive(HandType.KeyR, state);
        }
    }

    public void ContLeftCastCheck(bool state)
    {

        if (!state && cLHandable != null && cLHandable.IsActive)
        {
            cLHandable.SetActive(HandType.ContL, false);
            return;
        }

        Camera3D cam = GetViewport().GetCamera3D();
        PhysicsDirectSpaceState3D spState = cam.GetWorld3D().DirectSpaceState;

        var cLHandOrigin = cam.GlobalPosition;//cam.ProjectRayOrigin(cHandL.Position);
        var cLHandEnd = cLHandOrigin + cam.ProjectRayNormal(cHandL.Position) * cLHandLength;
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

        cLHandable = (IHandable)other;

        if (cLHandable.HandInputTargets.Keys.Contains(HandType.ContL))
        {
            cLHandable.SetActive(HandType.ContL, state);
        }
    }

    public void ContRightCastCheck(bool state)
    {
        if (!state && cRHandable != null && cRHandable.IsActive)
        {
            cRHandable.SetActive(HandType.ContR, false);
            return;
        }

        Camera3D cam = GetViewport().GetCamera3D();
        PhysicsDirectSpaceState3D spState = cam.GetWorld3D().DirectSpaceState;

        var cRHandOrigin = cam.GlobalPosition;//cam.ProjectRayOrigin(cHandR.Position);
        var cRHandEnd = cRHandOrigin + cam.ProjectRayNormal(cHandR.Position) * cRHandLength;
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

        cRHandable = (IHandable)other;

        if (cRHandable.HandInputTargets.Keys.Contains(HandType.ContR))
        {
            cRHandable.SetActive(HandType.ContR, state);
        }
    }


    public void SetHandExtent(HandType type, float str)
    {
        float val = Mathf.Lerp(HandExtents.X, HandExtents.Y, str);

        switch (type)
        {
            case HandType.Mouse:
                mHandLength = val;
                break;

            case HandType.KeyL:
                kLHandLength = val;
                break;

            case HandType.KeyR:
                kRHandLength = val;
                break;


            case HandType.ContL:
                cLHandLength = val;
                break;


            case HandType.ContR:
                cRHandLength = val;
                break;
        }
    }

}
