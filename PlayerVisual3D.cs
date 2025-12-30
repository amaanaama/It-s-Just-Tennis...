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

        float centeredX = (logicPos.X - 960) * ScaleFactor;
        float centeredZ = (logicPos.Y - 540) * ScaleFactor;

        this.GlobalPosition = new Vector3(
            centeredX, 
            0.05f, 
            centeredZ
        );

        GD.Print("LINK ACTIVE: Moving to " + logicPos);
    }
}