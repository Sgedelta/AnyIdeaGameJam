using Godot;
using System;

public partial class WinLose : Node
{
	public bool Win { get; set; } = false;
	public bool Lose { get; set; } = false;

	private Label titleText;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		titleText = GetNode<Label>("Label");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void DisplayWin()
	{
		Win = true;
		titleText.Text = "YIPPEE!!! YOU WIN!";
	}

	public void DisplayLose()
	{
		Lose = true;
		titleText.Text = "WOMP... YOU LOSE!";
	}

	public void on_button_pressed()
	{
        GetTree().ChangeSceneToFile("res://Scenes/main_menu.tscn");
    }
}
