using Godot;
using System;


public partial class PlayerLogic : CharacterBody2D
{
    [Export] public float HorizontalSpeed = 525.0f; // Increased for the wider court
    [Export] public float VerticalSpeed = 400.0f;   // Decreased to feel more weighted
    public Vector2 FieldSize = new Vector2(1000, 500);
    public float Altitude = 0.0f;
    public float VerticalVelocity;
    public float JumpImpulse = 350.0f;
    public float Gravity = 1000f;
    [Export] public int PlayerID = 1;

   public override void _PhysicsProcess(double delta){
    string p = "p" + PlayerID + "_";
    Vector2 inputVelocity = Vector2.Zero;
    float floatDelta = (float)delta;
    
    
    if (Input.IsActionPressed(p +"right")) inputVelocity.X += 1;
    if (Input.IsActionPressed(p +"left"))  inputVelocity.X -= 1;
    if (Input.IsActionPressed(p +"down"))  inputVelocity.Y += 1;
    if (Input.IsActionPressed(p + "up"))    inputVelocity.Y -= 1;

    if (inputVelocity.Length() > 0){
        inputVelocity = inputVelocity.Normalized();
    }
    Velocity = new Vector2(
        inputVelocity.X * HorizontalSpeed,
        inputVelocity.Y * VerticalSpeed
    );

    if (inputVelocity != Vector2.Zero)
    {
        GD.Print("Input detected! Direction: ", inputVelocity);
        //Velocity = inputVelocity.Normalized() * Speed;
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
    float netPosition = 195.0f; 
    float netBuffer = 25.0f;

if (PlayerID == 1)
{
    // Player 1 (Bottom): Can move from the Net down to the bottom (400)
    pos.Y = Mathf.Clamp(pos.Y, netPosition + netBuffer + 10, FieldSize.Y);
}
else
{
    // Player 2 (Top): Can move from the Top (0) down to the Net
    pos.Y = Mathf.Clamp(pos.Y, 0, netPosition - netBuffer);
}

    GlobalPosition = pos;
}

}