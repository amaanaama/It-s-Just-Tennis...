using Godot;
using System;

public partial class BallLogic : Node2D
{
    // --- Physics Constants ---
    [Export] public float Gravity = 1200.0f;
    [Export] public float BounceElasticity = 0.75f;
    [Export] public Vector2 FieldSize = new Vector2(1000, 500);

    // --- State Variables ---
    public Vector2 Velocity = Vector2.Zero;
    public float VerticalVelocity = 0.0f;
    public float Altitude = 30.0f; 
    
    // The "Freeze" Switch
    // Set this to false in Arena.cs to keep the ball stationary for testing
    public bool IsPhysicsActive = false; 

    public bool IsAttached = false;
    public PlayerLogic AttachedPlayer;

    public override void _PhysicsProcess(double delta)
    {
        // 1. ATTACHMENT LOGIC
        // If a player is holding the ball, it follows them exactly
        if (IsAttached && AttachedPlayer != null)
        {
            GlobalPosition = AttachedPlayer.GlobalPosition;
            Altitude = 40.0f; 
            return;
        }

        // 2. THE STATIONARY GATE
        // If physics are disabled, we stop right here. 
        // No gravity, no movement, no bouncing.
        if (!IsPhysicsActive)
        {
            return;
        }

        // 3. ACTIVE PHYSICS
        // This only runs once IsPhysicsActive is set to true (during the hit)
        float fDelta = (float)delta;

        // Apply Horizontal Movement
        GlobalPosition += Velocity * fDelta;

        // Apply Gravity and Vertical Movement
        VerticalVelocity += Gravity * fDelta;
        Altitude -= VerticalVelocity * fDelta;

        // Ground Collision (The Bounce)
        if (Altitude <= 0)
        {
            Altitude = 0;
            VerticalVelocity = -VerticalVelocity * BounceElasticity;
            
            // Friction: Slow down horizontal speed on every bounce
            Velocity *= 0.9f; 

            // Kill tiny bounces to prevent jitter
            if (Mathf.Abs(VerticalVelocity) < 50) 
                VerticalVelocity = 0;
        }

        // Keep ball inside the court lines
        ApplyBoundaries();
    }

    private void ApplyBoundaries()
    {
        // Bounce off Left/Right walls
        if (GlobalPosition.X < 0 || GlobalPosition.X > FieldSize.X)
        {
            Velocity = new Vector2(-Velocity.X, Velocity.Y);
            GlobalPosition = new Vector2(Mathf.Clamp(GlobalPosition.X, 0, FieldSize.X), GlobalPosition.Y);
        }

        // Bounce off Top/Bottom walls
        if (GlobalPosition.Y < 0 || GlobalPosition.Y > FieldSize.Y)
        {
            Velocity = new Vector2(Velocity.X, -Velocity.Y);
            GlobalPosition = new Vector2(GlobalPosition.X, Mathf.Clamp(GlobalPosition.Y, 0, FieldSize.Y));
        }
    }
}