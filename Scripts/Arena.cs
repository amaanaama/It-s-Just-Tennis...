using Godot;
using System;

public partial class Arena : Node2D
{
    [Export] public PackedScene VisualsTemplate;
    [Export] public PackedScene BallVisualsTemplate; // Drag BallVisuals.tscn here in Inspector

    private SpriteFrames _P1Frames; 
    private SpriteFrames _P2Frames;
    private Camera2D _camera;
    private Node2D _p1Visual;
    
    private BallLogic _ballLogic;
    private BallVisuals _ballVisual;

    public override void _Ready()
    {
        var gameManager = GetNode<GameManager>("/root/GameManager");
        _camera = GetNode<Camera2D>("Camera2D");
        
        

        var logicNodeP1 = GetNode<PlayerLogic>("Logic World/PlayerLogic1");
        var logicNodeP2 = GetNode<PlayerLogic>("Logic World/PlayerLogic2");
        var container = GetNode<Node2D>("Visual World");
        _P1Frames = gameManager.P1SpriteFrames;
        if (logicNodeP2.HasNode("AIController"))
    {
        _P2Frames = gameManager.P1SpriteFrames; // Mirror P1
    }
    else
    {
        _P2Frames = gameManager.P2SpriteFrames; // Use standard P2 selection
    }

        // Spawn P1
        SpawnCharacter(logicNodeP1, container, "Dynamic_Player1", _P1Frames);

        // Spawn P2
        if (gameManager.currentMode != GameManager.GameMode.Rougelike)
        {
            SpawnCharacter(logicNodeP2, container, "Dynamic_Player2", _P2Frames);
        }

        // --- BALL SPAWN ---
        // We call this last so it can find the players we just spawned
        SpawnBall();
    }

    private void SpawnCharacter(PlayerLogic brain, Node parent, string name, SpriteFrames frames)
    {
        PlayerVisuals instance = VisualsTemplate.Instantiate<PlayerVisuals>();
        instance.Name = name;
        instance.Brain = brain;
        parent.AddChild(instance);

        if (instance.AnimSprite != null && frames != null)
        {
            instance.AnimSprite.SpriteFrames = frames;
            // Play the ID-specific idle
            instance.AnimSprite.Play("p" + brain.PlayerID + "idle");
        }

        GD.Print("Success: " + name + " created.");
    }

    private void SpawnBall()
{
    // 1. Get references to containers
    var logicContainer = GetNodeOrNull<Node2D>("Logic World");
    var visualContainer = GetNodeOrNull<Node2D>("Visual World");

    if (logicContainer == null || visualContainer == null)
    {
        GD.PrintErr("CRITICAL: Logic World or Visual World containers are missing in the scene tree!");
        return;
    }

    // 2. Clear any existing ghosts (Defensive)
    var existingBalls = GetTree().GetNodesInGroup("ball_logic");
    foreach (Node b in existingBalls) b.QueueFree();

    // 3. Create the Logic (Math)
    _ballLogic = new BallLogic();
    _ballLogic.Name = "RealBallLogic";
    logicContainer.AddChild(_ballLogic); // Add to tree first
    _ballLogic.AddToGroup("ball_logic");

    // 4. Set Initial Logic State
    _ballLogic.GlobalPosition = new Vector2(500, 400); // Center of your logic court
    _ballLogic.Altitude = 30.0f;
    _ballLogic.IsPhysicsActive = false; // Freeze until hit
    _ballLogic.Velocity = Vector2.Zero;

    // 5. Create the Visuals (Art)
    if (BallVisualsTemplate == null)
    {
        GD.PrintErr("CRITICAL: BallVisualsTemplate is EMPTY in the Arena Inspector! Drag the .tscn file there.");
        return;
    }

    _ballVisual = BallVisualsTemplate.Instantiate<BallVisuals>();
    visualContainer.AddChild(_ballVisual);
    
    // 6. THE LINK: Plug the math into the art
    _ballVisual.LogicNode = _ballLogic;
    _ballVisual.AddToGroup("targets"); // For camera

    GD.Print($"SUCCESS: Ball Spawned. Logic: {_ballLogic.GlobalPosition} | Visual Parent: {_ballVisual.GetParent().Name}");
}
    public override void _Process(double delta)
    {
        if (_p1Visual == null)
        {
            _p1Visual = GetNodeOrNull<Node2D>("Visual World/Dynamic_Player1");
            return;
        }

        Vector2 screenCenter = new Vector2(320, 180);
        Vector2 offset = _p1Visual.GlobalPosition - screenCenter;
        float followStrength = 0.2f; 
        _camera.GlobalPosition = screenCenter + (offset * followStrength);
    }
}