using Godot;
using System;

public partial class BallLogic : Node2D
{
    // --- Physics Constants ---
    [Export] public float Gravity = 1500.0f;        // Increased to fall faster
    [Export] public float BounceElasticity = 0.75f; // Good bounce height
    [Export] public float AirDrag = 0.995f;         
    [Export] public float MaxSpeed = 900.0f;        
    [Export] public Vector2 FieldSize = new Vector2(1000, 500);

    // --- State Variables ---
    public Vector2 Velocity = Vector2.Zero;
    public float VerticalVelocity = 0.0f;
    public float Altitude = 30.0f; 
    
    public bool IsPhysicsActive = false; 
    public bool IsAttached = false;
    public PlayerLogic AttachedPlayer;

    private bool _isFrozen = false;

    public override void _PhysicsProcess(double delta)
    {
        // 1. ATTACHMENT LOGIC
        if (IsAttached && AttachedPlayer != null)
        {
            GlobalPosition = AttachedPlayer.GlobalPosition;
            Altitude = 40.0f; 
            return;
        }

        // 2. THE STATIONARY OR FROZEN GATE
        // If frozen, we skip all movement logic entirely for this frame
        if (!IsPhysicsActive || _isFrozen) return;

        float fDelta = (float)delta;

        // 3. ACTIVE PHYSICS
        // Apply Air Drag
        Velocity *= AirDrag;

        // Speed Clamp
        if (Velocity.Length() > MaxSpeed)
        {
            Velocity = Velocity.Normalized() * MaxSpeed;
        }

        // Apply Horizontal Movement
        GlobalPosition += Velocity * fDelta;

        // Apply Gravity and Vertical Movement
        VerticalVelocity += Gravity * fDelta;
        Altitude -= VerticalVelocity * fDelta;

        // 4. GROUND COLLISION (The Bounce)
        if (Altitude <= 0)
        {
            Altitude = 0;
            VerticalVelocity = -VerticalVelocity * BounceElasticity;
            Velocity *= 0.85f; // Friction

            // --- TRUE IMPACT FREEZE ---
            // Stops the ball for roughly 2 frames (0.03s)
            TriggerImpact(0.06f);

            if (Mathf.Abs(VerticalVelocity) < 40) VerticalVelocity = 0;
        }

        ApplyBoundaries();
    }

    /// <summary>
    /// Freezes the ball's movement entirely for a set duration.
    /// Can be called by PlayerLogic during a racket hit.
    /// </summary>
    public async void TriggerImpact(float duration)
{
    if (_isFrozen) return; 

    _isFrozen = true;
    
    // We use await to wait for the timer to finish naturally
    await ToSignal(GetTree().CreateTimer(duration), "timeout");
    
    _isFrozen = false;
}

    private void ApplyBoundaries()
    {
        if (GlobalPosition.X < 0 || GlobalPosition.X > FieldSize.X)
        {
            Velocity = new Vector2(-Velocity.X * 0.8f, Velocity.Y);
            GlobalPosition = new Vector2(Mathf.Clamp(GlobalPosition.X, 0, FieldSize.X), GlobalPosition.Y);
        }

        if (GlobalPosition.Y < 0 || GlobalPosition.Y > FieldSize.Y)
        {
            Velocity = new Vector2(Velocity.X, -Velocity.Y * 0.8f);
            GlobalPosition = new Vector2(GlobalPosition.X, Mathf.Clamp(GlobalPosition.Y, 0, FieldSize.Y));
        }
    }
	public void ForceUnfreeze()
{
    _isFrozen = false;
}
}