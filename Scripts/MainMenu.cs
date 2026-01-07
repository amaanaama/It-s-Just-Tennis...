using Godot;
using System;

public partial class MainMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_local_mp_button_pressed()
	{
		GD.Print("SIGNAL RECEIVED: Button is working!");
		var gameManager = GetNode<GameManager>("/root/GameManager");
		gameManager.currentMode = GameManager.GameMode.LocalMP;
		GetTree().ChangeSceneToFile("res://Scenes/Arena.tscn");
	}
}
