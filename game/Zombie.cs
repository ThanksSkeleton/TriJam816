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
	
	
	public bool isAlive;

		// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void Heal()
	{
		// Play Heal animation
		// Set Invisible
	}
}
