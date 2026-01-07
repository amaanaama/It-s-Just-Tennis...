using Godot;
using System;

public partial class GameManager : Node
{
	
	public enum GameMode
	{
		Exhibition,
		LocalMP,
		OnlineMP,
		Rougelike
	}

	[Export] public GameMode currentMode = GameMode.Exhibition;
	public int CurrentRound = 1;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
