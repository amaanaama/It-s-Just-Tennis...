using Godot;
using System;

public partial class GameManager : Node
{
    public enum GameMode { Exhibition, LocalMP, OnlineMP, Rougelike }
    [Export] public GameMode currentMode = GameMode.Exhibition;
    
    public SpriteFrames P1SpriteFrames;
    public SpriteFrames P2SpriteFrames;
    public int CurrentRound = 1;
    public bool IsP1Picking = true;

    public override void _Ready()
    {
    }

    // --- NEW: Reset Logic ---
    public override void _Input(InputEvent @event)
    {
        // Check if the "reset" action was pressed
        // Make sure you have "reset" defined in Project Settings -> Input Map (assigned to 'R')
        if (@event.IsActionPressed("reset"))
        {
            ResetScene();
        }
    }

    private void ResetScene()
    {
        GD.Print("Resetting Scene...");

        // Crucial: Reset TimeScale to 1.0
        // If you reset during a hitstop/freeze, the game will stay frozen without this!
        Engine.TimeScale = 1.0f;

        // Reload the current active scene
        GetTree().ReloadCurrentScene();
    }

    public override void _Process(double delta)
    {
    }
}