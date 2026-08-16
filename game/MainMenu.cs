using Godot;
using System;

public partial class MainMenu : Control
{
	[Export]
	TextureButton Start;

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Start.Pressed += StartGame;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void StartGame() 
	{
		GetTree().ChangeSceneToFile("res://howToPlay.tscn");		
	}
	
	
}
