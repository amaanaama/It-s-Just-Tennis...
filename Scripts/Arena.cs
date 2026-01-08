using Godot;
using System;

public partial class Arena : Node2D
{
    // The blueprint slot
    [Export] public PackedScene VisualsTemplate;
    private Texture2D _P1Skin; 
    private Texture2D _P2Skin;

    public override void _Ready()
    {

        var gameManager = GetNode<GameManager>("/root/GameManager");
        _P1Skin = gameManager.P1SelectedTexture;
        _P2Skin = gameManager.P2SelectedTexture;

        var logicNodeP1 = GetNode<PlayerLogic>("Logic World/PlayerLogic1");
        var logicNodeP2 = GetNode<PlayerLogic>("Logic World/PlayerLogic2");
        
        var container = GetNode<Node2D>("Visual World");


        SpawnCharacter(logicNodeP1, container, "Dynamic_Player1", _P1Skin);
        if (gameManager.currentMode != GameManager.GameMode.Rougelike)
        {
            SpawnCharacter(logicNodeP2, container, "Dynamic_Player2", _P2Skin);
        }
    }

    private void SpawnCharacter(PlayerLogic brain, Node parent, string name, Texture2D skin)
    {

        PlayerVisuals instance = VisualsTemplate.Instantiate<PlayerVisuals>();
        

        instance.Name = name;
        instance.Brain = brain;
        
        if (instance.CharacterSprite != null && skin != null)
        {
            instance.CharacterSprite.Texture = skin;
        }
        else if (skin == null)
        {
            GD.PrintErr("Warning: No skin texture assigned for " + name);
        }

        parent.AddChild(instance);
        
        GD.Print("Success: " + name + " created.");
        

    }
}