using Godot;
using System;

public partial class BallVisuals : Node2D
{
    public BallLogic LogicNode;
    [Export] public Sprite2D MainSprite;
    [Export] public Sprite2D ShadowSprite;

    private float courtWidthLogic = 1000f;
    private float courtHeightLogic = 500f;

    public override void _Process(double delta)
{
    if (LogicNode == null) return;

    // 1. SIMPLEST MAPPING (No perspective squeeze yet)
    // This assumes your screen center is 320. 
    // If logic is 500, screenX will be exactly 320.
    float screenX = 320.0f + (LogicNode.GlobalPosition.X - 500.0f);
    
    // Map Logic Y (0-500) to Screen Y (30-340)
    float screenY = 30.0f + (LogicNode.GlobalPosition.Y * (310.0f / 500.0f));

    // 2. APPLY POSITION
    this.GlobalPosition = new Vector2(screenX, screenY);

    // 3. FORCE VISIBILITY
    if (MainSprite != null)
    {
        MainSprite.Position = new Vector2(0, -LogicNode.Altitude);
        MainSprite.Visible = true;
        // Make the ball big so we can't miss it
        MainSprite.Scale = new Vector2(1 ,1);
    }
    
    // 4. LOG THE SCREEN POSITION
    if (LogicNode.IsPhysicsActive)
    {
        GD.Print($"VISUAL CHECK: Logic({LogicNode.GlobalPosition}) -> Drawing at Screen({screenX}, {screenY})");
    }
}

    
}