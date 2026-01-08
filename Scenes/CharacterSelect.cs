using Godot;
using System;
using System.Collections.Generic;

public partial class CharacterSelect : Control
{
    private GameManager _gameManager;
    [Export] public Label StatusLabel;    
    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>("/root/GameManager");
        _gameManager.P1SelectedTexture = null;
        _gameManager.P2SelectedTexture = null;
        _gameManager.IsP1Picking = true;

        MapButtonToSprite("Red", "res://Resources/Sprites/different colors for sprites/orange.png");
		MapButtonToSprite("Blue", "res://Resources/Sprites/different colors for sprites/blue.png");
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
        btn.Pressed += () => HandleSelection(GD.Load<Texture2D>(texturePath));
	}
    
    private void HandleSelection(Texture2D selectedTex){
    if (_gameManager.IsP1Picking){
        _gameManager.P1SelectedTexture = selectedTex;
        GD.Print("P1 Picked!");

        if (_gameManager.currentMode == GameManager.GameMode.LocalMP){
            _gameManager.IsP1Picking = false;
            UpdateUI();
        }
        else{
            StartGame();
        }
    }
    else
    {
        _gameManager.P2SelectedTexture = selectedTex;
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