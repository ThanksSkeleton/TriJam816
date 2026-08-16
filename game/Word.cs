using Godot;
using System;

public partial class Word : Area2D
{
	public GameManager GameManager;

	// Current State
	public bool IsCute;	
	public bool isAlive;
	public string word;

	public Vector2 Direction;
	public Vector2 Velocity;
	double PausedTime; 
	double BoostTime;
	double BoostFraction;
	int LetterIndex; 
	
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
			this.textBox.AddThemeColorOverride("cute", Color.FromHtml("f19ee5"));
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
		if (isAlive) {
			this.textBox.Text = word;
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
			this.Position = Velocity * (float)delta;
			}
	}
	
	public void Collide(Area2D otherArea)
	{
		if (otherArea.IsInGroup("Player"))
		{
			ExplodeBadEffect();
			GameManager.Hurt();
		}
	}

	public char FirstLetter() {
		return word[LetterIndex];
	}
	
	public void TypeStrike(string letter) 
	{
		if (word[LetterIndex] == letter.ToLower()[0]) 
		{
			var c_array = word.ToCharArray();
			c_array[LetterIndex] = ' ';
			word = new string(c_array);
			if (String.IsNullOrEmpty(word)) 
			{
				this.ExplodeGoodEffect(this.IsCute);
				this.WordComplete();
				this.Destroy();
			} else {
				this.GoodHitEffect();
				this.LetterIndex = LetterIndex+1;
			}
		} else {
			this.BadHitEffect();
			this.PausedTime = PauseDuration;
			this.BoostFraction = this.BoostFraction + BaseBoostFraction;
			this.BoostTime = this.BoostTime  + BoostDuration;
		}
	}

	public void WordComplete() 
	{
		if (IsCute) {
			GameManager.Cute();
		}
	}
	
	public void Destroy() {
		this.isAlive = false;
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
		this.Destroy(); 
	}

	private void ExplodeBadEffect()
	{
		ani.Stop();
		ani.Play("fail");
		this.Destroy(); 
	}
}
