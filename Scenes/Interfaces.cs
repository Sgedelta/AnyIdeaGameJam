using Godot;
using System;


public enum HandType
{
    Mouse,
    KeyL,
    KeyR,
    ContL,
    ContR
}

public interface IHandable
{
    bool IsActive { get; set; }

    Godot.Collections.Dictionary<HandType, Godot.Collections.Dictionary<HandType, NodePath>> HandInputTargets { get; } 

    public void SetActive(HandType inputHand, bool state);



}
