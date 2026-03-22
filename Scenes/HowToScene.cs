using Godot;
using System;

public partial class HowToScene : Control
{
	[Export]
	private Button contButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		contButton.Pressed += BackToMenu;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void BackToMenu()
    {
        GetTree().ChangeSceneToFile("res://Scenes/main_menu.tscn");
    }
}
