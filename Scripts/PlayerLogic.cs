using Godot;
using System;


public partial class PlayerLogic : CharacterBody2D
{
    [Export]
    public float Speed { get; set; } = 200.0f; 
    public Vector2 FieldSize = new Vector2(300, 200);
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

    Vector2 pos = GlobalPosition;
    pos.X = Mathf.Clamp(pos.X, 0, FieldSize.X);
    pos.Y = Mathf.Clamp(pos.Y, 0, FieldSize.Y);
    GlobalPosition = pos;
}
}