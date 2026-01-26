using Godot;
using System;

public partial class PlayerLogic : CharacterBody2D
{
    [Export] public float HorizontalSpeed = 525.0f;
    [Export] public float VerticalSpeed = 400.0f;
    [Export] public int PlayerID = 1;
    [Export] public float ReachX = 120.0f;
    [Export] public float ReachY = 80.0f;
    [Export] public float MaxHitHeight = 100.0f; 
    [Export] public float SlideFriction = 0.15f; 

    public Vector2 FieldSize = new Vector2(1000, 500);
    public float Altitude = 0.0f;
    public bool IsCharging = false;
    
    [Signal] public delegate void OnSwingEventHandler(string animName);
    
    private float _swingCooldown = 0.0f;

    public override void _PhysicsProcess(double delta)
{
    string p = "p" + PlayerID + "_";
    float floatDelta = (float)delta;

    // 1. Always update cooldown
    if (_swingCooldown > 0) _swingCooldown -= floatDelta;

    // 2. AI CONTROL PATH
    if (HasNode("AIController"))
    {
        MoveAndSlide();
        ApplyBoundaries();
        return; // EXIT HERE so keyboard code never runs for AI
    }
    
    // 3. PLAYER CONTROL PATH (Put the rest of your logic here)
    if (IsCharging || _swingCooldown > 0.1f)
    {
        Velocity = Velocity.Lerp(Vector2.Zero, SlideFriction);
        MoveAndSlide();

        if (Input.IsActionJustReleased(p + "forehand") && IsCharging)
        {
            IsCharging = false;
            EmitSignal(SignalName.OnSwing, "forehand_swing");
            PerformSwing();
        }
        ApplyBoundaries(); // Added this to keep charging players in bounds
        return; 
    }

    // Movement Input
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

    // Start Charging
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

    public void PerformSwing()
{
    _swingCooldown = 0.4f;
    var ball = GetTree().GetFirstNodeInGroup("ball_logic") as BallLogic;
    if (ball == null) return;

    float diffX = ball.GlobalPosition.X - GlobalPosition.X;
    float diffY = ball.GlobalPosition.Y - GlobalPosition.Y;

    // Use the exact variables we exported
    bool isCloseEnough = Mathf.Abs(diffX) < ReachX && Mathf.Abs(diffY) < ReachY;
    
    // REDUCE THIS: If MaxHitHeight is 100, maybe the "Sweet Spot" is 20-80
    // If the ball is at 150 altitude, this will return false
    bool isHeightRight = ball.Altitude >= 0.0f && ball.Altitude <= MaxHitHeight;

    if (isCloseEnough && isHeightRight)
    {
        ExecuteHit(ball, diffX);
    }
    else
    {
        GD.Print("Swing Missed! Altitude was: " + ball.Altitude);
    }
}

   private void ExecuteHit(BallLogic ball, float diffX)
{
    ball.ForceUnfreeze(); 
    //ball.TriggerImpact(0.08f); 
    ball.IsPhysicsActive = true;
    ball.Altitude = 55.0f; 

    float yDir = (PlayerID == 1) ? -1.0f : 1.0f;

    // 1. CONTROLLED HORIZONTAL: Based strictly on where you hit the ball
    float horizontalPower = Mathf.Clamp(diffX * -5.0f, -300f, 300f);
    
    // 2. STABLE FORWARD POWER: 
    // We set a fixed base speed (750) so it doesn't accelerate forever.
    float baseForwardSpeed = 750.0f; 
    float forwardPower = baseForwardSpeed * yDir; 

    // 3. APPLY VELOCITY (Override the old velocity entirely)
    ball.Velocity = new Vector2(horizontalPower, forwardPower);
    ball.VerticalVelocity = -120.0f; // Constant upward pop
}
}