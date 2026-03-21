using Godot;
using System;

public partial class GameManager : Node
{

	private static GameManager _instance;
	public static GameManager Instance { get { return _instance; } }
	public GameManager GDInstance { get { return _instance; } }


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//singleton
		if(Instance == null)
		{
			_instance = this;
			this.ProcessMode = ProcessModeEnum.Always;
		}
		else
		{
			GD.PrintErr($"Two Game Managers Created. Deleting {Name}");
			QueueFree();
			return;
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
