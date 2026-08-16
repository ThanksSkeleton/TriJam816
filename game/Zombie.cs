using System.Collections.Generic;
using Godot;

public partial class Zombie : Node2D
{
	[Export]
	Sprite2D sprite;
	[Export]
	AudioStreamPlayer2D thanks1;
	[Export]
	AudioStreamPlayer2D thanks2;
	[Export]
	CpuParticles2D hearts;
	[Export]
	AnimationPlayer player;
	
	public bool isAlive;
	public Vector2 facing;

		// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	public void Create(Vector2 facing)
	{
		if (facing == Vector2.Left)
		{
			sprite.FlipH = true;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void Heal()
	{
		player.Play("Heal");
	}

	public void RandomThankYou()
	{
		var player = Helper.PickRandom(new List<AudioStreamPlayer2D>() { thanks1, thanks2 });
		player.Play();
	}

	public void Die()
	{
		isAlive = false;
	}
}
