using Godot;
using System;

public partial class Root : Node2D
{
	[Export]
	Node2D Title;
	[Export]
	Node2D Battle;
	[Export]
	Node2D Loss;
	[Export]
	Node2D Win;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
