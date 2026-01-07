using Godot;
using System;

public partial class Arena : Node2D
{
    private PackedScene _playerScene;

    public override void _Ready()
    {
        // 1. Load the Player Template
        _playerScene = GD.Load<PackedScene>("res://Scenes/PlayerLogic.tscn");

        // 2. Get the GameManager
        var gameManager = GetNode<GameManager>("/root/GameManager");

        // 3. Spawn Player 1 (Left Side)
        // x: 80 (inner left), y: 180 (vertical center)
        SpawnPlayer(1, new Vector2(80, 180)); 

        // 4. Handle Player 2 / AI
        if (gameManager.currentMode == GameManager.GameMode.LocalMP)
        {
            // Spawn Player 2 (Right Side)
            // x: 560 (inner right), y: 180 (vertical center)
            SpawnPlayer(2, new Vector2(560, 180));
        }
        else if (gameManager.currentMode == GameManager.GameMode.Rougelike)
        {
            GD.Print("Roguelite Mode: AI logic will go here!");
        }
    }

    private void SpawnPlayer(int id, Vector2 spawnPosition)
    {
        var playerInstance = _playerScene.Instantiate<PlayerLogic>();
        AddChild(playerInstance);

        playerInstance.PlayerID = id;
        playerInstance.GlobalPosition = spawnPosition;
        
        GD.Print($"Player {id} spawned at {spawnPosition}");
    }
}