using Godot;
using Godot.Collections;
using System;

public partial class Battery : RigidBody3D, IHandable
{

    public bool Inserted = false;
    public bool Dropped = false;

    public BatteryReceptical InsertLoc;

    public int HandCount = 0;

    public float TimeToDrop = 5;
    public float TimeLeft = 5;

    private float _dropImpulseStrength = 5;

    private RandomNumberGenerator rng;

    private Array<HandType> _heldHands;


    public bool IsActive { get; set; }

    [Export] private Dictionary<HandType, Dictionary<HandType, NodePath>> _handInputTargets = new Dictionary<HandType, Dictionary<HandType, NodePath>>();

    public Dictionary<HandType, Dictionary<HandType, NodePath>> HandInputTargets { get { return _handInputTargets; } }

    public override void _Ready()
    {
        rng = new RandomNumberGenerator();
        _heldHands = new Array<HandType>();
    }

    public override void _Process(double delta)
    {
        if(!Inserted)
        {
            if(HandCount == 1)
            {
                TimeLeft -= (float)delta;
                if(TimeLeft < 0)
                {
                    Drop();
                }
            }
            else
            {
                TimeLeft += Mathf.Min((float)delta * (HandCount - 2), TimeToDrop);
            }


            if(HandCount > 0)
            {
                //get hand forwards
                int forwardHands = 0;
                for(int i = 0; i < _heldHands.Count; i++)
                {
                    switch(_heldHands[i])
                    {
                        case HandType.Mouse:
                            forwardHands += GameManager.Instance.HCont.mHandLength > GameManager.Instance.HCont.HandExtents.Length() / 2 + GameManager.Instance.HCont.HandExtents.X
                                ? 1 : 0;
                            break;

                        case HandType.KeyL:
                            forwardHands += GameManager.Instance.HCont.kLHandLength > GameManager.Instance.HCont.HandExtents.Length() / 2 + GameManager.Instance.HCont.HandExtents.X
                                 ? 1 : 0;
                            break;

                        case HandType.KeyR:
                            forwardHands += GameManager.Instance.HCont.kRHandLength > GameManager.Instance.HCont.HandExtents.Length() / 2 + GameManager.Instance.HCont.HandExtents.X
                                ? 1 : 0;
                            break;

                        case HandType.ContL:
                            forwardHands += GameManager.Instance.HCont.cLHandLength > GameManager.Instance.HCont.HandExtents.Length() / 2 + GameManager.Instance.HCont.HandExtents.X
                                ? 1 : 0;
                            break;

                        case HandType.ContR:
                            forwardHands += GameManager.Instance.HCont.cRHandLength > GameManager.Instance.HCont.HandExtents.Length() / 2 + GameManager.Instance.HCont.HandExtents.X
                                ? 1 : 0;
                            break;
                    }
                    
                }

                if(forwardHands > _heldHands.Count / 2f)
                {
                    Node3D batteryPos = GetViewport().GetCamera3D().GetNode<Node3D>("BatteryPushLoc");
                    GlobalPosition = batteryPos.Position;
                    RotationDegrees = batteryPos.RotationDegrees;
                }
                else
                {
                    Node3D batteryPos = GetViewport().GetCamera3D().GetNode<Node3D>("BatteryHoldLoc");
                    GlobalPosition = batteryPos.Position;
                    RotationDegrees = batteryPos.RotationDegrees;
                }

                
            }
        }
        else
        {
            TimeLeft = TimeToDrop;
            if(InsertLoc != null)
            {
                GlobalPosition = InsertLoc.BatterySnapLoc.GlobalPosition;
                RotationDegrees = InsertLoc.BatterySnapLoc.RotationDegrees;
            }
        }
    }

    private void Drop()
    {
        if(Dropped)
        {
            return;
        }
        Dropped = true;
        SetActive(HandType.Mouse, false);
        SetActive(HandType.KeyL, false);
        SetActive(HandType.KeyR, false);
        SetActive(HandType.ContL, false);
        SetActive(HandType.ContR, false);
        HandCount = 0;


        this.ApplyImpulse(new Vector3(rng.RandfRange(-1, 1), rng.RandfRange(-1, 1), rng.RandfRange(-1, 1)).Normalized() * _dropImpulseStrength);
    }

    private void CheckGrabState(bool added)
    {
        if(HandCount == 0 && !Inserted)
        {
            Drop();
        }

        if(HandCount == 1 && added)
        {
            TimeLeft = 1;
            Dropped = false;
        }
    }


    public void SetActive(HandType inputHand, bool state)
    {
        IsActive = state;

        if(_handInputTargets.ContainsKey(inputHand))
        {
            //for every input hand that could go on this
            foreach (HandType controlledHand in _handInputTargets[inputHand].Keys)
            {
                if(state)
                {
                    _heldHands.Add(controlledHand);
                }
                else if(_heldHands.Contains(controlledHand))
                {
                     _heldHands.Remove(controlledHand);
                }
                HandCount += state ? 1 : -1;
                CheckGrabState(state);
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
