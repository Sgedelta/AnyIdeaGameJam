using Godot;
using Godot.Collections;
using System;


public enum ThrusterLocation
{
	left, right, frontLeft , frontRight
}

public partial class AsteroidField : Node3D
{
	[Export]
	public float shipSpeed = 2f;
	public Vector3 shipVelocity = Vector3.Zero;

    [Export] Node3D forwardLines;

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



	public bool leftBatteryPowered = false;
	public bool rightBatteryPowered = false;
	public bool frontLeftBatteryPowered = false;
	public bool frontRightBatteryPowered = false;

	public bool leftValveLocked = false;
	public bool rightValveLocked = false;
	public bool frontLeftValveLocked = false;
	public bool frontRightValveLocked = false;

	public bool leftDoorOpen = false;
	public bool rightDoorOpen = false;
	public bool frontLeftDoorOpen = false;
	public bool frontRightDoorOpen = false;






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
		//if (@event is InputEventKey)
		//{
		//	InputEventKey mover = (InputEventKey)@event;
		//	if (mover.Keycode == Key.Left && mover.IsPressed())
		//	{
		//		leftThrust = true;
		//	}
		//	else if (mover.Keycode == Key.Left && mover.IsReleased())
  //              {
  //                  leftThrust = false;
  //              }

  //          if (mover.Keycode == Key.Right && mover.IsPressed())
		//	{
		//		rightThrust = true;
		//	}
		//	else if (mover.Keycode == Key.Right && mover.IsReleased())
		//	{
		//		rightThrust = false;
		//	}



			
  //          if (mover.Keycode == Key.Space && mover.IsPressed())
  //          {
  //              thrust1 = 1;
  //          }
  //          else if (mover.Keycode == Key.Space && mover.IsReleased())
  //          {
		//		thrust1 = 0;
  //          }

  //          if (mover.Keycode == Key.B && mover.IsPressed())
  //          {
  //              thrust2 = 1;
  //          }
  //          else if (mover.Keycode == Key.B && mover.IsReleased())
  //          {
  //              thrust2 = 0;
  //          }

		//	forwardThrust = thrust1 + thrust2;
		//	GD.Print(forwardThrust);
  //      }
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

        UpdateThrustfunctionality(ThrusterLocation.left);

        UpdateThrustfunctionality(ThrusterLocation.right);

        UpdateThrustfunctionality(ThrusterLocation.frontLeft);

        UpdateThrustfunctionality(ThrusterLocation.frontRight);


        //left
        if (leftBatteryPowered && !leftDoorOpen && leftValveLocked)
        {
            leftThrust = true;
        }
        else
        {
            leftThrust = false;
        }

        if (rightBatteryPowered && !rightDoorOpen && rightValveLocked)
        {
            rightThrust = true;
        }
        else
        {
            rightThrust = false;
        }


        GD.Print($"{frontLeftBatteryPowered} {frontLeftDoorOpen} {frontLeftValveLocked}");

        if (frontLeftBatteryPowered && !frontLeftDoorOpen && frontLeftValveLocked)
        {
            thrust1 = 1;
        }
        else
        {
            thrust1 = 0;
        }

        if (frontRightBatteryPowered && !frontRightDoorOpen && frontRightValveLocked)
        {
            thrust2 = 1;
        }
        else
        {
            thrust2 = 0;
        }

        forwardThrust = thrust1 + thrust2;
        GD.Print(forwardThrust);


        float zVel = forwardThrust;

        forwardLines.Visible = zVel > 0;
		

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



	public void UpdateThrustfunctionality(ThrusterLocation thrusterLocation)
	{
		BatteryReceptical tempBattery = new BatteryReceptical();
		Door tempDoor = new Door();
		valve tempValve = new valve();

        switch (thrusterLocation)
		{
			case ThrusterLocation.left:
				tempBattery = (BatteryReceptical)(GetTree().GetFirstNodeInGroup("LRecep"));
                leftBatteryPowered = tempBattery.Powered;
                tempDoor = (Door)(GetTree().GetFirstNodeInGroup("LDoor"));
                leftDoorOpen = tempDoor.IsOpen;
                tempValve = (valve)(GetTree().GetFirstNodeInGroup("LValve"));
                leftValveLocked = tempValve.IsLocked;
                break;
			case ThrusterLocation.right:
                tempBattery = (BatteryReceptical)(GetTree().GetFirstNodeInGroup("RRecep"));
                rightBatteryPowered = tempBattery.Powered;
                tempDoor = (Door)(GetTree().GetFirstNodeInGroup("RDoor"));
                rightDoorOpen = tempDoor.IsOpen;
                tempValve = (valve)(GetTree().GetFirstNodeInGroup("RValve"));
                rightValveLocked = tempValve.IsLocked;
                break;
			case ThrusterLocation.frontLeft:
                tempBattery = (BatteryReceptical)(GetTree().GetFirstNodeInGroup("FLRecep"));
                frontLeftBatteryPowered = tempBattery.Powered;
                tempDoor = (Door)(GetTree().GetFirstNodeInGroup("FLDoor"));
                frontLeftDoorOpen = tempDoor.IsOpen;
                tempValve = (valve)(GetTree().GetFirstNodeInGroup("FLValve"));
                frontLeftValveLocked = tempValve.IsLocked;
                break;
			case ThrusterLocation.frontRight:
                tempBattery = (BatteryReceptical)(GetTree().GetFirstNodeInGroup("FRRecep"));
                frontRightBatteryPowered = tempBattery.Powered;
                tempDoor = (Door)(GetTree().GetFirstNodeInGroup("FRDoor"));
                frontRightDoorOpen = tempDoor.IsOpen;
                tempValve = (valve)(GetTree().GetFirstNodeInGroup("FRValve"));
                frontRightValveLocked = tempValve.IsLocked;
                break;
		}
	}
}
