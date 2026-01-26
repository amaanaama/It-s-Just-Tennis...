using Godot;
using System;

public partial class PlayerLogic : CharacterBody2D
{
    [Export] public float HorizontalSpeed = 525.0f;
    [Export] public float VerticalSpeed = 400.0f;
    [Export] public int PlayerID = 1;
    [Export] public float ReachX = 120.0f;
    [Export] public float ReachY = 80.0f;
    [Export] public float MaxHitHeight = 60.0f;
    [Export] public float SlideFriction = 0.15f; // Lower = more slide

    public Vector2 FieldSize = new Vector2(1000, 500);
    public float Altitude = 0.0f;
    public bool IsCharging = false;
    
    [Signal] public delegate void OnSwingEventHandler(string animName);
    
    private float _swingCooldown = 0.0f;

    public override void _PhysicsProcess(double delta)
    {
        string p = "p" + PlayerID + "_";
        float floatDelta = (float)delta;

        if (_swingCooldown > 0) _swingCooldown -= floatDelta;

        // --- 1. CHARGING & SWINGING STATE ---
        if (IsCharging || _swingCooldown > 0.1f)
        {
            // Apply Slide: Gradually slow down velocity to zero
            Velocity = Velocity.Lerp(Vector2.Zero, SlideFriction);
            MoveAndSlide();

            // Handle the Release
            if (Input.IsActionJustReleased(p + "forehand") && IsCharging)
            {
                IsCharging = false;
                PerformSwing();
            }
            return; // Lock movement input
        }

        // --- 2. MOVEMENT INPUT ---
        Vector2 inputDir = Vector2.Zero;
        if (Input.IsActionPressed(p + "right")) inputDir.X += 1;
        if (Input.IsActionPressed(p + "left"))  inputDir.X -= 1;
        if (Input.IsActionPressed(p + "down"))  inputDir.Y += 1;
        if (Input.IsActionPressed(p + "up"))    inputDir.Y -= 1;

        if (inputDir != Vector2.Zero)
        {
            inputDir = inputDir.Normalized();
            Velocity = new Vector2(inputDir.X * HorizontalSpeed, inputDir.Y * VerticalSpeed);
        }
        else
        {
            Velocity = Vector2.Zero;
        }

        // --- 3. START CHARGE CHECK ---
        if (Input.IsActionPressed(p + "forehand") && _swingCooldown <= 0)
        {
            IsCharging = true;
            EmitSignal(SignalName.OnSwing, "forehand_charge");
            return;
        }

        MoveAndSlide();
        ApplyBoundaries();
    }

    private void ApplyBoundaries()
    {
        Vector2 pos = GlobalPosition;
        pos.X = Mathf.Clamp(pos.X, 0, FieldSize.X);

        float netPosition = 195.0f;
        float netBuffer = 25.0f;

        if (PlayerID == 1)
            pos.Y = Mathf.Clamp(pos.Y, netPosition + netBuffer + 10, FieldSize.Y);
        else
            pos.Y = Mathf.Clamp(pos.Y, 0, netPosition - netBuffer);

        GlobalPosition = pos;
    }

    private void PerformSwing()
{
    _swingCooldown = 0.4f;
    var ball = GetTree().GetFirstNodeInGroup("ball_logic") as BallLogic;
    if (ball == null) return;

    float diffX = ball.GlobalPosition.X - GlobalPosition.X;
    float diffY = ball.GlobalPosition.Y - GlobalPosition.Y;

    // --- REALISTIC HIT BOX ---
    // Horizontal reach: 70 units (the length of the racket)
    // Vertical depth: 40 units (how far in front/behind you can hit)
    // Height: Only hit if ball is between ankle (0) and head (80) height
    bool isCloseEnough = Mathf.Abs(diffX) < 70 && Mathf.Abs(diffY) < 40;
    bool isHeightRight = ball.Altitude > 0 && ball.Altitude < 80;

    if (isCloseEnough && isHeightRight)
    {
        ExecuteHit(ball, diffX);
    }
    else
    {
        GD.Print($"Missed! Dist: {Mathf.Abs(diffX)},{Mathf.Abs(diffY)} Alt: {ball.Altitude}");
    }
}

    private void ExecuteHit(BallLogic ball, float diffX)
{
    GD.Print("CLACK!");
    ball.IsPhysicsActive = true;

    // Determine direction based on PlayerID (P1 hits up, P2 hits down)
    float yDir = (PlayerID == 1) ? -1.0f : 1.0f;

    // X Velocity: Based on how "off-center" you hit the ball (Cross-court shots)
    float launchX = diffX * 10.0f; 

    // Y Velocity: A steady drive toward the other side
    float launchY = 600.0f * yDir;

    // Vertical Velocity: This makes the ball "arc" into the air
    float launchVertical = -400.0f; 

    ball.Velocity = new Vector2(launchX, launchY);
    ball.VerticalVelocity = launchVertical;
}
}