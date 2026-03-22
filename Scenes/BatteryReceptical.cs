using Godot;
using System;

public partial class BatteryReceptical : Area3D
{

    [Signal] public delegate void OnPoweredChangedEventHandler(bool powered);

    public bool AllowInsert = false;

	[Export] public Node3D BatterySnapLoc;

	public bool Powered = false;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetAllowInsert(bool val)
	{
		AllowInsert = val;
	}

	public void CheckObjIn(Node3D body)
	{

		if(body is not Battery || !AllowInsert)
		{
			return;
		}

		Battery batt = (Battery)body;

        if (batt.HandCount < 2)
        {
            return;
        }

        InsertOrExtract(batt, true);

	}

	public void CheckObjOut(Node3D body)
	{
        if (body is not Battery || !AllowInsert)
        {
            return;
        }

        Battery batt = (Battery)body;

        if (batt.HandCount < 2)
        {
            return;
        }

        InsertOrExtract(batt, false);
    }

	public void InsertOrExtract(Battery batt, bool insert)
	{

        batt.Inserted = insert;
        batt.InsertLoc = this;
		if (insert)
		{
			batt.Drop(false);
		}
        Powered = insert;
		EmitSignal(SignalName.OnPoweredChanged, insert);
    }
	

}
