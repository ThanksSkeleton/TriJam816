using Godot;
using System;

public partial class Word : CharacterBody2D
{
	GameManager GameManager;

	// Current State
	public bool IsCute;	
	public bool isAlive;
	public string word;

	public Vector2 Direction;
	double PausedTime; 
	double BoostTime;
	double BoostFraction;
	int LetterIndex; 

	
	// Fields
	string MyString;
	
	// Consts
	[Export]
	double BaseVelocity;
	[Export]
	double BaseBoostFraction;
	[Export]
	double PauseDuration;
	[Export]
	double BoostDuration;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (isAlive) {
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
			// Move
			}
	}
	
	public void OnCollide(Object Hurtbox) 
	{
		GameManager.Hurt();
		this.ExplodeBadEffect();
		this.Destroy();
	}

	public char FirstLetter() {
		return MyString[LetterIndex];
	}
	
	public void TypeStrike(string letter) 
	{
		if (MyString[LetterIndex] == letter.ToLower()[0]) 
		{
			var c_array = MyString.ToCharArray();
			c_array[LetterIndex] = ' ';
			MyString = new string(c_array);
			if (String.IsNullOrEmpty(MyString)) 
			{
				this.ExplodeGoodEffect();
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

	public void WordComplete() {
		if (IsCute) {
			GameManager.Cute();
		}
	}
	
	public void Destroy() {
		this.isAlive = false;
		this.Visible = false;
	}

	private void BadHitEffect()
	{
		throw new NotImplementedException();
	}

	private void GoodHitEffect()
	{
		throw new NotImplementedException();
	}

	private void ExplodeGoodEffect()
	{
		throw new NotImplementedException();
	}

	private void ExplodeBadEffect()
	{
		throw new NotImplementedException();
	}

}
