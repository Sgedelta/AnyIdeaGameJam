using Godot;
using Godot.Collections;
using System;

public partial class AsteroidField : Node3D
{
	[Export]
	public float shipSpeed = 20f;
	public Vector3 shipVelocity = Vector3.Zero;

	private Area3D goalCollision;

	public Area3D shipCollider;

	right_valve_thruster rightThruster;
	left_valve_thruster leftThruster;

	bool alive = true;

    public bool rightThrust = false;
	public bool leftThrust = false;
	public int forwardThrust = 0;
    int thrust1 = 0;
    int thrust2 = 0;

    public Array<Area3D> asteroidColliders = new Array<Area3D>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		alive = true;
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

    public override void _Input(InputEvent @event)
    {
		if (@event is InputEventKey)
		{
			InputEventKey mover = (InputEventKey)@event;
			if (mover.Keycode == Key.Left && mover.IsPressed())
			{
				leftThrust = true;
			}
			else if (mover.Keycode == Key.Left && mover.IsReleased())
                {
                    leftThrust = false;
                }

            if (mover.Keycode == Key.Right && mover.IsPressed())
			{
				rightThrust = true;
			}
			else if (mover.Keycode == Key.Right && mover.IsReleased())
			{
				rightThrust = false;
			}



			
            if (mover.Keycode == Key.Space && mover.IsPressed())
            {
                thrust1 = 1;
            }
            else if (mover.Keycode == Key.Space && mover.IsReleased())
            {
				thrust1 = 0;
            }

            if (mover.Keycode == Key.B && mover.IsPressed())
            {
                thrust2 = 1;
            }
            else if (mover.Keycode == Key.B && mover.IsReleased())
            {
                thrust2 = 0;
            }

			forwardThrust = thrust1 + thrust2;
			GD.Print(forwardThrust);
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!alive) return;

		GameManager.Instance.timer -= (float)delta;
		if(GameManager.Instance.timer <= 0)
		{
            alive = false;
            GetTree().ChangeSceneToFile("res://Scenes/UI/LoseUI.tscn");
        }

		float zVel = forwardThrust;
		

		float xVel = 0;
		if (rightThrust)
		{
			xVel++;
		}
		else
		{
			xVel--;
		}

        if (!leftThrust)
        {
            xVel++;
        }
        else
        {
            xVel--;
        }

		shipVelocity = new Vector3(-xVel * shipSpeed, 0, zVel * shipSpeed);

		//GD.Print(shipVelocity.ToString());
        GlobalPosition += shipVelocity * (float)delta;
		GlobalPosition = new Vector3(Mathf.Clamp(GlobalPosition.X, -20, 20), 0, GlobalPosition.Z);
	}

	
	public void HandleGoalCollision( Area3D areaChungus)
    {
        alive = false;
        GetTree().ChangeSceneToFile("res://Scenes/UI/WinUI.tscn");
    }

	public void HandleAsteroidCollision(Area3D areaChungus)
    {
        alive = false;
        GetTree().ChangeSceneToFile("res://Scenes/UI/LoseUI.tscn");
    }
}
