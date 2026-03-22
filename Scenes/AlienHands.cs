using Godot;
using System;

public partial class AlienHands : Node3D
{
	[Export] public Node3D mHandTarget;
	[Export] public Node3D kLHandTarget;
	[Export] public Node3D kRHandTarget;
	[Export] public Node3D cLHandTarget;
	[Export] public Node3D cRHandTarget;

    public override void _Ready()
    {
		GameManager.Instance.Hands = this;
    }

}
