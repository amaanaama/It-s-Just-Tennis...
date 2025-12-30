using Godot;
using System;


public partial class Player : CharacterBody2D
{
    [Export]
    public int Speed { get; set; } = 1000; 

   public override void _PhysicsProcess(double delta)
{
    Vector2 inputVelocity = Vector2.Zero;

    if (Input.IsActionPressed("right")) inputVelocity.X += 1;
    if (Input.IsActionPressed("left"))  inputVelocity.X -= 1;
    if (Input.IsActionPressed("down"))  inputVelocity.Y += 1;
    if (Input.IsActionPressed("up"))    inputVelocity.Y -= 1;

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
}
}