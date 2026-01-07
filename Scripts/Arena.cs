using Godot;
using System;

public partial class Arena : Node2D
{
    // The blueprint slot
    [Export] public PackedScene VisualsTemplate;
    [Export] public Texture2D P1Skin; 
    [Export] public Texture2D P2Skin;

    public override void _Ready()
    {
        GD.Print("--- Spawner Started ---");
        var logicNodeP1 = GetNode<PlayerLogic>("Logic World/PlayerLogic1");
        var logicNodeP2 = GetNode<PlayerLogic>("Logic World/PlayerLogic2");
        
        var container = GetNode<Node2D>("Visual World");

        SpawnCharacter(logicNodeP1, container, "Dynamic_Player1", P1Skin);
        SpawnCharacter(logicNodeP2, container, "Dynamic_Player2", P2Skin);
    }

    private void SpawnCharacter(PlayerLogic brain, Node parent, string name, Texture2D skin)
    {
        // Safety Check
        if (VisualsTemplate == null)
        {
            GD.PrintErr("CRITICAL ERROR: VisualsTemplate is empty! Drag your .tscn file into the Inspector.");
            return;
        }

        // A. Create the copy
        // We cast it to 'PlayerVisuals' so we can access the .Brain variable
        PlayerVisuals instance = VisualsTemplate.Instantiate<PlayerVisuals>();
        
        // B. Setup the copy
        instance.Name = "Dynamic_Player1";
        instance.Brain = brain; // <--- This connects the code to the visual!
        
        // C. Add it to the world
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