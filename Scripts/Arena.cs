using Godot;
using System;

public partial class Arena : Node2D
{
    [Export] public PackedScene VisualsTemplate;
    
    // Changed these from Texture2D to SpriteFrames
    private SpriteFrames _P1Frames; 
    private SpriteFrames _P2Frames;
    private Camera2D _camera;
    private Node2D _p1Visual;

    public override void _Ready()
    {
        var gameManager = GetNode<GameManager>("/root/GameManager");
        _camera = GetNode<Camera2D>("Camera2D");
        
        // Grab the SpriteFrames we saved in the selection menu
        _P1Frames = gameManager.P1SpriteFrames;
        _P2Frames = gameManager.P2SpriteFrames;

        var logicNodeP1 = GetNode<PlayerLogic>("Logic World/PlayerLogic1");
        var logicNodeP2 = GetNode<PlayerLogic>("Logic World/PlayerLogic2");
        
        var container = GetNode<Node2D>("Visual World");

        // Spawn P1
        SpawnCharacter(logicNodeP1, container, "Dynamic_Player1", _P1Frames);

        // Spawn P2 (Skip if in Roguelike mode, assuming AI is handled elsewhere or not needed)
        if (gameManager.currentMode != GameManager.GameMode.Rougelike)
        {
            SpawnCharacter(logicNodeP2, container, "Dynamic_Player2", _P2Frames);
        }
    }

    private void SpawnCharacter(PlayerLogic brain, Node parent, string name, SpriteFrames frames)
    {
        // 1. Instantiate the visuals template
        PlayerVisuals instance = VisualsTemplate.Instantiate<PlayerVisuals>();

        instance.Name = name;
        instance.Brain = brain;
        
        // 2. Add it to the tree BEFORE trying to access nodes if using GetNode internally
        parent.AddChild(instance);

        // 3. Apply the animations to the AnimatedSprite2D
        // We access AnimSprite which is the Exported variable in your PlayerVisuals script
        if (instance.AnimSprite != null && frames != null)
        {
            instance.AnimSprite.SpriteFrames = frames;
            instance.AnimSprite.Play("idle");
        }
        else if (frames == null)
        {
            GD.PrintErr("Warning: No SpriteFrames assigned for " + name);
        }

        GD.Print("Success: " + name + " created with animations.");
    }
    public override void _Process(double delta)
{
    // 1. Find P1 if we haven't yet (since they are spawned dynamically)
    if (_p1Visual == null)
    {
        _p1Visual = GetNodeOrNull<Node2D>("Visual World/Dynamic_Player1");
        return;
    }

    // 2. Target Position (The center of the screen is roughly 320, 180)
    // We only want the camera to move SLIGHTLY, so we lerp toward the player
    Vector2 screenCenter = new Vector2(320, 180);
    
    // Calculate how far the player is from the center
    Vector2 offset = _p1Visual.GlobalPosition - screenCenter;

    // 3. Apply a "weight" so the camera only follows 20% of the way
    // This prevents the camera from leaving the court boundaries
    float followStrength = 0.2f; 
    _camera.GlobalPosition = screenCenter + (offset * followStrength);
}
}