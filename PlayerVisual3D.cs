using Godot;
using System;

public partial class PlayerVisual3D : Node3D
{

    [Export] 
    public CharacterBody2D LogicNode; 
    [Export] 
    public float ScaleFactor = 0.01f;

    // We use _Process because it runs every frame and provides smooth visuals
    public override void _Process(double delta)
    {
        if (LogicNode == null)
        {
            GD.Print("LINK BROKEN: LogicNode is null! Re-assign it in the Inspector.");
            return;
        }

        Vector2 logicPos = LogicNode.GlobalPosition;

        Vector3 new3DPosition = new Vector3(
            logicPos.X * ScaleFactor, 
            0.05f, 
            logicPos.Y * ScaleFactor
        );

        GlobalPosition = new3DPosition;

        GD.Print("LINK ACTIVE: Moving to " + logicPos);
    }
}