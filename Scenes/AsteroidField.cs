using Godot;
using Godot.Collections;
using System;

public partial class AsteroidField : Node3D
{
	[Export]
	public float shipSpeed = 3f;
	public Vector3 shipVelocity = Vector3.Zero;

	private Area3D goalCollision;

	public Area3D shipCollider;

	right_valve_thruster rightThruster;
	left_valve_thruster leftThruster;

    public bool rightThrust = false;
	public bool leftThrust = false;

	public Array<Area3D> asteroidColliders = new Array<Area3D>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		shipVelocity = new Vector3(0, 0, 3f);
        shipCollider = GetTree().GetFirstNodeInGroup("ship").GetNode<Area3D>("Area3D");
		//if (shipCollider != null) GD.Print("Shippa!");
        goalCollision = GetNode<Area3D>("Goal");
        //if (goalCollision != null) GD.Print("Goala!");

        goalCollision.AreaEntered += HandleGoalCollision;
        Array<Node> tempAsteroidList = GetTree().GetNodesInGroup("asteroid");
		//if (tempAsteroidList.Count > 0) GD.Print("aseroi 1");
        for (int i = 0; i < tempAsteroidList.Count; i++)
		{
			asteroidColliders.Add(tempAsteroidList[i] as Area3D);
            asteroidColliders[i].AreaEntered += HandleAsteroidCollision;
		}
		//if (asteroidColliders.Count > 0) GD.Print("aseroi 2");


		//Thruster get
		rightThruster = GetTree().GetFirstNodeInGroup("rightThruster") as right_valve_thruster;
		leftThruster = GetTree().GetFirstNodeInGroup("leftThruster") as left_valve_thruster;


    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition += shipVelocity * (float)delta;
	}

	
	public void HandleGoalCollision( Area3D areaChungus)
	{
        GD.Print("YIPEE");
    }

	public void HandleAsteroidCollision(Area3D areaChungus)
	{
        GD.Print("oof");
    }
}
