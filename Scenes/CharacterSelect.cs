using Godot;
using System;
using System.Collections.Generic;

public partial class CharacterSelect : Control
{
    private GameManager _gameManager;
    [Export] public Label StatusLabel;    


    private void MapButtonToFrames(string buttonName, string resourcePath)
{
    var btn = GetNodeOrNull<Button>($"CenterContainer/HBoxContainer/{buttonName}"); 
    if (btn != null)
    {
        btn.Pressed += () => {
            // Load the resource
            var frames = GD.Load<SpriteFrames>(resourcePath);
            
            // Call the handler instead of trying to set a single variable
            HandleSelection(frames);
        };
    }
}
    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>("/root/GameManager");
        _gameManager.P1SpriteFrames = null;
        _gameManager.P2SpriteFrames = null;
        _gameManager.IsP1Picking = true;

        MapButtonToSprite("Red", "res://Resources/Sprites/different colors for sprites/orange.png");
		MapButtonToFrames("Blue", "res://Resources/Sprites/different colors for sprites/BlueSprite.tres");
        MapButtonToSprite("Green", "res://Resources/Sprites/different colors for sprites/lgreen.png");
		MapButtonToSprite("Dark Green", "res://Resources/Sprites/different colors for sprites/green.png");       
        MapButtonToSprite("Pink", "res://Resources/Sprites/different colors for sprites/pink.png");
		MapButtonToSprite("Purple", "res://Resources/Sprites/different colors for sprites/purple.png");
        MapButtonToSprite("Black", "res://Resources/Sprites/different colors for sprites/black.png");
		MapButtonToSprite("Teal", "res://Resources/Sprites/different colors for sprites/teal.png");	
        MapButtonToSprite("White", "res://Resources/Sprites/different colors for sprites/white.png");
		MapButtonToSprite("Orange", "res://Resources/Sprites/different colors for sprites/yellow.png");	
        UpdateUI();
		GD.Print("Character Select System Ready!");
    }

    private void MapButtonToSprite(string buttonName, string texturePath)
    {

        var btn = GetNode<Button>($"CenterContainer/HBoxContainer/{buttonName}"); 
        btn.Pressed += () => HandleSelection(GD.Load<SpriteFrames>(texturePath));
	}
    
    private void HandleSelection(SpriteFrames selectedFrames)
{
    if (_gameManager.IsP1Picking)
    {
        // Use P1SpriteFrames here!
        _gameManager.P1SpriteFrames = selectedFrames;
        GD.Print("P1 Picked!");

        if (_gameManager.currentMode == GameManager.GameMode.LocalMP)
        {
            _gameManager.IsP1Picking = false;
            UpdateUI();
        }
        else
        {
            StartGame();
        }
    }
    else
    {
        // Use P2SpriteFrames here!
        _gameManager.P2SpriteFrames = selectedFrames;
        GD.Print("P2 Picked!");
        StartGame();
    }
}

    private void UpdateUI(){
        if (_gameManager.IsP1Picking){
            StatusLabel.Text = "Player 1: Choose your character";
        }
        else{
            StatusLabel.Text = "Player 2: Choose your character";
        }
    }

    private void StartGame(){
    	GD.Print("All selections complete. Loading Arena...");
    	GetTree().ChangeSceneToFile("res://scenes/Arena.tscn");
	}
}