using Godot;
using System;

public partial class MainMenu : Control
{
    [Export]
    private Button startButton;
    [Export]
    private Button howToButton;
    [Export]
    private Button quitButton;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        startButton.Pressed += LoadGameplay;
        howToButton.Pressed += HowToPlay;
        quitButton.Pressed += Quit;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void LoadGameplay()
    {
        GetTree().ChangeSceneToFile("res://Scenes/AssetPopWorld.tscn");
    }
    public void HowToPlay()
    {
        GetTree().ChangeSceneToFile("res://Scenes/HowToScene.tscn");
    }
    public void Quit()
    {
        GetTree().Quit();
    }
}
