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

    Godot.Collections.Dictionary<HandType, Node3D> HandTargets { get; } 

    Godot.Collections.Array<HandType> HandInputs { get; }

    public void SetActive(bool state);



}
