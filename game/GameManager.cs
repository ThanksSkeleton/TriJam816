using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameManager : Node2D
{
	[Export]
	public PackedScene ZombieScene; 

	[Export]
	public PackedScene WordScene;

	// State
	int ZombiesWaiting;
	int ZombieLevel;

	int HealthLevel;
	double SongTimeRemaining;
	List<Word> myWords = new List<Word>();
	List<Zombie> myZombies = new List<Zombie>();
	Word TargetedWord;
	List<(bool, double)> SpawnTimes = new List<(bool, double)>();
	[Export]
	Node2D Left_Zombie_1;
	[Export]
	Node2D Left_Zombie_2;
	[Export]
	Node2D Left_Zombie_3;
	[Export]
	Node2D Left_Zombie_4;
	[Export]
	Node2D Left_Zombie_5;
	[Export]
	Node2D Left_Zombie_6;
	[Export]
	Node2D Right_Zombie_1;
	[Export]
	Node2D Right_Zombie_2;
	[Export]
	Node2D Right_Zombie_3;
	[Export]
	Node2D Right_Zombie_4;
	[Export]
	Node2D Right_Zombie_5;
	[Export]
	Node2D Right_Zombie_6;

	[Export]
	Node2D Left_Word_1;
	[Export]
	Node2D Left_Word_2;
	[Export]
	Node2D Left_Word_3;
	[Export]
	Node2D Left_Word_4;
	[Export]
	Node2D Left_Word_5;
	[Export]
	Node2D Left_Word_6;
	[Export]
	Node2D Right_Word_1;
	[Export]
	Node2D Right_Word_2;
	[Export]
	Node2D Right_Word_3;
	[Export]
	Node2D Right_Word_4;
	[Export]
	Node2D Right_Word_5;
	[Export]
	Node2D Right_Word_6;
	
	// Const
	const int cuteRequired = 7; 
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Start Music
		// Set SongTimeRemaining
		this.SpawnTimes = GenerateSpawns();
	}

	const int nonCuteMultiplier = 3;

	public List<(bool, double)> GenerateSpawns() 
	{
		var toReturn = new List<(bool, double)>();
		for (int i = 0; i < cuteRequired+1; i++) 
		{
			toReturn.Add((true, new Random().NextDouble() * SongTimeRemaining));
		}
		for (int i = 0; i < cuteRequired * nonCuteMultiplier; i++) 
		{
			toReturn.Add((false, new Random().NextDouble() * SongTimeRemaining));
		}
		toReturn = toReturn.OrderBy(a => -a.Item2).ToList();
		return toReturn;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		SongTimeRemaining = Math.Max(SongTimeRemaining-delta, 0);
		if (SongTimeRemaining == 0) 
		{
			if (ZombieLevel > 0) 
			{
				Die();
			} else {
				Win();
			}			
		} else {
			DrawZombieLevel();
			DrawHealthLevel();
			if (SongTimeRemaining > SpawnTimes[0].Item2)
			{
				SpawnWord(SpawnTimes[0].Item1);
			}
		}
	}

	public void Hurt() 
	{
		HealthLevel = HealthLevel -1;
		if (HealthLevel < 1) {
			Die();
		}	
	}
	
	public void ZombieSpawn()
	{
		var Left_zombies = new List<Node2D> { Left_Zombie_1, Left_Zombie_2, Left_Zombie_3, Left_Zombie_4, Left_Zombie_5, Left_Zombie_6 };
		var Right_zombies = new List<Node2D> { Right_Zombie_1, Right_Zombie_2, Right_Zombie_3, Right_Zombie_4, Right_Zombie_5, Right_Zombie_6 };
	}

	public void Cute() 
	{
		ZombieLevel = ZombieLevel -1;
		// Random Zombie
		Zombie z = Helper.PickRandom(myZombies);
		z.Heal();
		if (ZombiesWaiting > 0) {
			ZombieSpawn();
			ZombiesWaiting = ZombiesWaiting - 1; 
		}	
	}
	
	public void SpawnWord(bool cute) 
	{
		var Left_Words = new List<Node2D> { Left_Word_1, Left_Word_2, Left_Word_3, Left_Word_4, Left_Word_5, Left_Word_6 };
		var Right_Words = new List<Node2D> { Right_Word_1, Right_Word_2, Right_Word_3, Right_Word_4, Right_Word_5, Right_Word_6 };

		// pick left or right
		// pick spawn location
		// spawn word and add it to Word List;
		Word word = null;


		var sd = new List<(Node2D, Vector2)> { (Helper.PickRandom(Left_Words), Vector2.Right), (Helper.PickRandom(Right_Words), Vector2.Left) };
		var selected_sd = Helper.PickRandom(sd);
		if (cute)
		{
			word = MakeWord(Helper.PickRandom(WordsList.CuteWords), true, selected_sd.Item1, selected_sd.Item2);
		} else {
			word = MakeWord(Helper.PickRandom(WordsList.NormalWords), false, selected_sd.Item1, selected_sd.Item2);
		}
		
		this.myWords.Add(word);
	}

	private Word MakeWord(string word, bool isCute, Node2D parent, Vector2 direction) 
	{
		Word w =  (Word) WordScene.Instantiate();
		w.isAlive = true;
		w.IsCute = isCute;
		w.Direction = direction;
		w.word = word;
		parent.AddChild(w);
		return w;
	}

	private Zombie MakeZombie(Vector2 facingDirection, Node2D parent)
	{
		Zombie z = (Zombie) ZombieScene.Instantiate();
		parent.AddChild(z);
		return z;
	}

	
	public void AcceptInput(char c) 
	{

		if (TargetedWord != null) 
		{
			HitTargetedWord(c);
		} else {
			var liveWords = myWords.Where(w => w.isAlive);
			if (liveWords.Any()) {
				var goodTargets = liveWords.Where(w => w.FirstLetter() == c);
				if (goodTargets.Any()) 
				{
					var w = Helper.PickRandom(goodTargets.ToList());
					TargetedWord = w;
					HitTargetedWord(c);	
				} else {
					var w = // Get random live word
					TargetedWord = Helper.PickRandom(liveWords.ToList());
					HitTargetedWord(c);
				}
			} else {
			// Do nothing
			}
		}
	}
	
	public void HitTargetedWord(char c) {
		TargetedWord.TypeStrike("" + c);
		if (!TargetedWord.isAlive) {
			TargetedWord = null;
		} 	
	}
	
	public void Die() 
	{
		GamePause();
		LosePopup();
	}

	public void Win() 
	{
		GamePause();
		WinPopup();
	}

	private void WinPopup()
	{
		throw new NotImplementedException();
	}

	private void LosePopup()
	{
		throw new NotImplementedException();
	}

	private void GamePause()
	{
		throw new NotImplementedException();
	}

	private void DrawHealthLevel()
	{
		throw new NotImplementedException();
	}

	private void DrawZombieLevel()
	{
		throw new NotImplementedException();
	}
}
