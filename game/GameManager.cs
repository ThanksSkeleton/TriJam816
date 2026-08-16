using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameManager : Node2D
{
	[Export]
	public AudioStreamPlayer mainMusic;
	
	[Export]
	public PackedScene ZombieScene; 

	[Export]
	public PackedScene WordScene;

	// State
	int ZombieLevel;

	int HealthLevel;
	double elapsedTime;
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
	Node2D Right_Word_1;
	[Export]
	Node2D Right_Word_2;
	[Export]
	Node2D Right_Word_3;
	[Export]
	Node2D Right_Word_4;
	[Export]
	Node2D Right_Word_5;
	
	
	// Const
	const int cuteRequired = 12; 
	
	const double graceTime = 10.0;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Start Music

		elapsedTime = 0;
		GD.Print("Song Time"+ mainMusic.Stream.GetLength());
		this.SpawnTimes = GenerateSpawns();
		ZombieSpawn();
		this.mainMusic.Play();
		HealthLevel = 3;
	}

	const int nonCuteMultiplier = 2;

	// public List<(bool, double)> GenerateSpawnsSimple()
	// {
	// 	return new List<(bool, double)>
	// 	{
	// 		(true, 0.0),
	// 		(false, 1.0)
	// 	};
	// }

	public List<(bool, double)> GenerateSpawns() 
	{
		var toReturn = new List<(bool, double)>();

		var firstWord = (false, 0.0);

		var batch1 = GenerateBatch(0.1 * mainMusic.Stream.GetLength(), mainMusic.Stream.GetLength(), 4);
		var batch2 = GenerateBatch(0.4 * mainMusic.Stream.GetLength(), mainMusic.Stream.GetLength(), 4);
		var batch3 = GenerateBatch(0.8 * mainMusic.Stream.GetLength(), mainMusic.Stream.GetLength(), 5);

		toReturn.Add(firstWord);
		toReturn.AddRange(batch1);
		toReturn.AddRange(batch2);
		toReturn.AddRange(batch3);

		toReturn = toReturn.OrderBy(a => a.Item2).ToList();
		return toReturn;
	}

	public List<(bool, double)> GenerateBatch(double floor, double end, int numCute)
	{
		var toReturn = new List<(bool, double)>();
		for (int i = 0; i < numCute; i++) 
		{
			toReturn.Add((true, GenerateRandomTime(floor, end)));
		}
		for (int i = 0; i < numCute * nonCuteMultiplier; i++) 
		{
			toReturn.Add((false, GenerateRandomTime(floor, end)));
		}
		return toReturn;
	}

	public double GenerateRandomTime(double floor, double end)
	{
		return floor + ((end-floor) * new Random().NextDouble());	
	}


	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			var s = char.ConvertFromUtf32((int)keyEvent.Unicode);
			if (s.Length == 1 && char.IsLetter(s[0])) {
				AcceptInput(s[0]);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		elapsedTime = elapsedTime + delta;
		if ((graceTime + mainMusic.Stream.GetLength()) - elapsedTime <= 0) 
		{
			if (myZombies.Where(z => z.isAlive).Any()) 
			{
				GetTree().ChangeSceneToFile("res://game_over.tscn");		
			} else {
				GetTree().ChangeSceneToFile("res://you_win.tscn");		
			}			
		} else {
			DrawZombieLevel();
			DrawHealthLevel();
			if (SpawnTimes.Any() && elapsedTime > SpawnTimes[0].Item2)
			{
				SpawnWord(SpawnTimes[0].Item1);
				SpawnTimes.RemoveAt(0);
			}
		}
	}

	public void Hurt() 
	{
		HealthLevel = HealthLevel -1;
		if (HealthLevel < 1) {
			GetTree().ChangeSceneToFile("res://game_over.tscn");		
		}	
	}
	
	public void ZombieSpawn()
	{
		var Left_zombies = new List<Node2D> { Left_Zombie_1, Left_Zombie_2, Left_Zombie_3, Left_Zombie_4, Left_Zombie_5 };
		foreach (var z_spot in Left_zombies)
		{
			myZombies.Add(MakeZombie(Vector2.Right, z_spot));
		}


		var Right_zombies = new List<Node2D> { Right_Zombie_1, Right_Zombie_2, Right_Zombie_3, Right_Zombie_4, Right_Zombie_5 };
		foreach (var z_spot in Right_zombies)
		{
			myZombies.Add(MakeZombie(Vector2.Left, z_spot));
		}
	}

	private Zombie MakeZombie(Vector2 facingDirection, Node2D parent)
	{
		Zombie z = (Zombie) ZombieScene.Instantiate();
		z.isAlive = true;
		z.facing = facingDirection;
		parent.AddChild(z);
		return z;
	}

	public void Cute() 
	{
		// Random Zombie
		if (myZombies.Where(z => z.isAlive).Any()) 
		{
			Zombie z = Helper.PickRandom(myZombies.Where(z => z.isAlive).ToList());
			z.Heal();
		}
		GD.Print("No Zombies");

	}
	
	public void SpawnWord(bool cute) 
	{
		var Left_Words = new List<Node2D> { Left_Word_1, Left_Word_2, Left_Word_3, Left_Word_4, Left_Word_5 };
		var Right_Words = new List<Node2D> { Right_Word_1, Right_Word_2, Right_Word_3, Right_Word_4, Right_Word_5 };

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
		w.GameManager = this;
		w.IsCute = isCute;
		w.Direction = direction;
		w.SetInner(word);
		parent.AddChild(w);
		return w;
	}

	public void AcceptInput(char c) 
	{
		if (TargetedWord != null && TargetedWord.isAlive()) 
		{
			GD.Print("Targeting Word");
			HitTargetedWord(c);
		} else {
			GD.Print("Finding New Word");
			var liveWords = myWords.Where(w => w.isAlive());
			if (liveWords.Any()) {
				GD.Print("Live Words:" + liveWords.Count());
				var goodTargets = liveWords.Where(w => w.FirstLetter() == c);
				if (goodTargets.Any()) 
				{
					GD.Print("Good Words");
					TargetedWord = Helper.PickRandom(goodTargets.ToList());
					HitTargetedWord(c);	
				} else {
					GD.Print("Bad Words");
					TargetedWord = Helper.PickRandom(liveWords.ToList());
					HitTargetedWord(c);
				}
			} else {
			// Do nothing
			}
		}
	}
	
	public void HitTargetedWord(char c) {
		TargetedWord.TypeStrike(c);
		if (!TargetedWord.isAlive()) {
			TargetedWord = null;
		} 	
	}

	private void DrawHealthLevel()
	{
		//throw new NotImplementedException();
	}

	private void DrawZombieLevel()
	{
		//throw new NotImplementedException();
	}
}
