using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class GameManager : Node
{
	
	public enum GameMode{Exhibition, LocalMP, OnlineMP, Rougelike}
	[Export] public GameMode currentMode = GameMode.Exhibition;
	public SpriteFrames P1SpriteFrames;
	public SpriteFrames P2SpriteFrames;
	public int CurrentRound = 1;
	public bool IsP1Picking = true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
