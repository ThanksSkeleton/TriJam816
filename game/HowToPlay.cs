using Godot;
using System;

public partial class HowToPlay : Node2D
{
	[Export]
	public TextureButton Start;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Start.Pressed += StartBattle;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void StartBattle() 
	{
		GetTree().ChangeSceneToFile("res://battle.tscn");		
	}
}
