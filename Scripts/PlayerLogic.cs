using Godot;
using System;


public partial class PlayerLogic : CharacterBody2D
{
    [Export]
    public float Speed { get; set; } = 350.0f; 
    public Vector2 FieldSize = new Vector2(700, 400);
    public float Altitude = 0.0f;
    public bool isJumping = false;
    public float VerticalVelocity;
    public float JumpImpulse = 350.0f;
    public float Gravity = 1000f;
    [Export] public int PlayerID = 1;

   public override void _PhysicsProcess(double delta){
    string p = "p" + PlayerID + "_";
    Vector2 inputVelocity = Vector2.Zero;
    float floatDelta = (float)delta;
    
    if (Input.IsActionPressed(p + "jump") && isJumping == false){
        isJumping = true;
        VerticalVelocity = -JumpImpulse;
    }

    if (isJumping)
    {
        VerticalVelocity += Gravity *floatDelta;
        Altitude -= VerticalVelocity *floatDelta;

        if (Altitude <= 0)
            {
                Altitude = 0;
                isJumping = false;
                VerticalVelocity = 0;
            }
    }

    if (Input.IsActionPressed(p +"right")) inputVelocity.X += 1;
    if (Input.IsActionPressed(p +"left"))  inputVelocity.X -= 1;
    if (Input.IsActionPressed(p +"down"))  inputVelocity.Y += 1;
    if (Input.IsActionPressed(p + "up"))    inputVelocity.Y -= 1;

    if (inputVelocity.Length() > 0){
        inputVelocity = inputVelocity.Normalized();
    }
    Velocity = inputVelocity * Speed;

    if (inputVelocity != Vector2.Zero)
    {
        GD.Print("Input detected! Direction: ", inputVelocity);
        Velocity = inputVelocity.Normalized() * Speed;
    }
    else
    {
        Velocity = Vector2.Zero;
    }

    MoveAndSlide();

    // 2. Define our boundaries
    Vector2 pos = GlobalPosition;
    
    // X-Axis: Full width of the field (Widened)
    pos.X = Mathf.Clamp(pos.X, 0, FieldSize.X);

    // Y-Axis: Halfway Line Logic
    float halfPoint = FieldSize.Y / 2;
    float netBuffer = 2.0f; // Small gap so they don't touch the net perfectly

    if (PlayerID == 1)
    {
        // Player 1 (Bottom side): Can move from the middle to the bottom edge
        pos.Y = Mathf.Clamp(pos.Y, halfPoint + netBuffer, FieldSize.Y);
    }
    else
    {
        // Player 2 (Top side): Can move from the top edge to the middle
        pos.Y = Mathf.Clamp(pos.Y, 0, halfPoint - netBuffer);
    }

    GlobalPosition = pos;
}

}