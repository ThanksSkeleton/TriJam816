using Godot;
using System;

public partial class Word : Area2D
{
	public GameManager GameManager;

	// Current State
	public bool IsCute;	
	public Vector2 Direction;
	public Vector2 Velocity;
	double PausedTime; 
	double BoostTime;
	double BoostFraction;
	int LetterIndex; 
	bool deadFailure;

	private WordString innerWord;

	// Consts
	[Export]
	double BaseVelocity;
	[Export]
	double BaseBoostFraction;
	[Export]
	double PauseDuration;
	[Export]
	double BoostDuration;
	[Export]
	GpuParticles2D greenSplat;
	[Export]
	CpuParticles2D heartBurst;
	[Export]
	RichTextLabel textBox;
	[Export]
	Area2D area;
	[Export]
	AudioStreamPlayer2D normal;
	[Export]
	AudioStreamPlayer2D cute;
	[Export]
	AudioStreamPlayer2D fail;
	[Export]
	AnimationPlayer ani;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (IsCute) {
			this.textBox.AddThemeColorOverride("default_color", Color.FromHtml("f19ee5"));
		}
		if (Direction == Vector2.Left) {
			this.textBox.HorizontalAlignment = HorizontalAlignment.Left;
		} 
		else
		{
			this.textBox.HorizontalAlignment = HorizontalAlignment.Right;
		}
		this.Monitoring = true;
		this.AreaEntered += Collide;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (isAlive()) {
			this.textBox.Text = innerWord.Inner();
			if (PausedTime > 0) 
			{
				this.Velocity = Vector2.Zero;
				this.PausedTime = Math.Max(this.PausedTime - delta, 0);	
			} else if (BoostTime > 0) {
				this.Velocity = Direction * (float) (BaseVelocity * BoostFraction);
				this.BoostTime = Math.Max(this.BoostTime - delta, 0);	
			} else {
				this.Velocity = Direction * (float) BaseVelocity;
			}
			this.Position = this.Position + (Velocity * (float)delta);
		}
	}
	
	public void Collide(Area2D otherArea)
	{
		if (otherArea.IsInGroup("Player") && this.isAlive())
		{
			this.deadFailure = true;
			ExplodeBadEffect();
			GameManager.Hurt();
		}
	}
	
	public void TypeStrike(char letter) 
	{
		if (innerWord.ApplyHit(letter)) 
		{
			if (innerWord.IsFinished()) 
			{
				this.ExplodeGoodEffect(this.IsCute);
				this.WordComplete();
			} else {
				this.GoodHitEffect();
			}
		} else {
			this.BadHitEffect();
			this.PausedTime = PauseDuration;
			this.BoostFraction = this.BoostFraction + BaseBoostFraction;
			this.BoostTime = this.BoostTime  + BoostDuration;
		}
	}

	public bool isAlive()
	{
		return !this.innerWord.IsFinished() && !this.deadFailure;
	}

	public void WordComplete() 
	{
		if (IsCute) {
			GameManager.Cute();
		}
	}

	private void BadHitEffect()
	{
		ani.Stop();
		ani.Play("BadHit");
	}

	private void GoodHitEffect()
	{
		ani.Stop();
		ani.Play("GoodHit");
	}

	private void ExplodeGoodEffect(bool cute)
	{		
		ani.Stop();
		if (cute) {
			ani.Play("SuccessCute");
		} else {
			ani.Play("SuccessNormal");
		}
	}

	private void ExplodeBadEffect()
	{
		ani.Stop();
		ani.Play("fail");
	}

	internal char FirstLetter()
	{
		return innerWord.FirstLetter();
	}

	internal void SetInner(string word)
	{
		this.innerWord = new WordString(word);
	}
}
