using Godot;
using System;

public partial class AIController : Node2D
{
    private PlayerLogic _logic;
    private BallLogic _ball;
    private float _targetX;

    [Export] public bool ShowDebug = true;

    public override void _Ready()
    {
        _logic = GetParent<PlayerLogic>();
        // Using group to find the ball initially
        _ball = GetTree().GetFirstNodeInGroup("ball_logic") as BallLogic;
        
        if (_logic != null)
            _targetX = _logic.GlobalPosition.X; 
            
        GD.Print(_ball == null ? "AI: Ball NOT found initially!" : "AI: Ball found and tracking.");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_logic == null) return;

        // 1. SAFETY CHECK: If ball is missing or was deleted (Disposed)
        // This prevents the "Cannot access a disposed object" error
        if (!IsInstanceValid(_ball))
        {
            _ball = GetTree().GetFirstNodeInGroup("ball_logic") as BallLogic;
            if (!IsInstanceValid(_ball)) return; // Stop if still no ball
        }

        // 2. POSITION SYNC
        // Keep the AIController node on top of the Logic node so drawing works
        GlobalPosition = _logic.GlobalPosition;

        // 3. TARGET CALCULATION
        if (_ball.Velocity.Y < -10)
        {
            // Calculate prediction
            float distanceY = Mathf.Abs(_ball.GlobalPosition.Y - 100.0f);
            float timeToReach = distanceY / Mathf.Abs(_ball.Velocity.Y);
            _targetX = _ball.GlobalPosition.X + (_ball.Velocity.X * timeToReach);
        }
        else
        {
            // Track current position if stationary or moving away
            _targetX = _ball.GlobalPosition.X;
        }

        // Clamp within field boundaries
        _targetX = Mathf.Clamp(_targetX, 50, _logic.FieldSize.X - 50);

        // 4. MOVEMENT
        float diffX = _targetX - _logic.GlobalPosition.X;
        float diffY = 100.0f - _logic.GlobalPosition.Y; 

        Vector2 moveDir = Vector2.Zero;
        if (Mathf.Abs(diffX) > 10) moveDir.X = Mathf.Sign(diffX);
        if (Mathf.Abs(diffY) > 10) moveDir.Y = Mathf.Sign(diffY);

        _logic.Velocity = new Vector2(moveDir.X * _logic.HorizontalSpeed, moveDir.Y * _logic.VerticalSpeed);

        // 5. AUTO-SWING (Strike Zone)
        float distToBall = _logic.GlobalPosition.DistanceTo(_ball.GlobalPosition);
        if (distToBall < 85 && !_logic.IsCharging && _ball.GlobalPosition.Y < 400)
        {
            TriggerSwing();
        }

        if (ShowDebug) QueueRedraw();
    }

    public override void _Draw()
    {
        // Safety check for drawing
        if (!ShowDebug || _logic == null || !IsInstanceValid(_ball)) return;

        // Convert global positions to local space so lines start at AI center
        Vector2 localTarget = ToLocal(new Vector2(_targetX, 100.0f));
        Vector2 localBall = ToLocal(_ball.GlobalPosition);

        // Yellow line to predicted target
        DrawLine(Vector2.Zero, localTarget, new Color(1, 1, 0, 0.5f), 2.0f);
        // Red circle at target point
        DrawCircle(localTarget, 8.0f, new Color(1, 0, 0, 0.8f));
        // Blue line tracking the actual ball
        DrawLine(Vector2.Zero, localBall, new Color(0, 0.5f, 1, 0.3f), 1.0f);
    }

    private async void TriggerSwing()
    {
        if (_logic == null) return;

        _logic.IsCharging = true;
        _logic.EmitSignal(PlayerLogic.SignalName.OnSwing, "forehand_charge");
        
        await ToSignal(GetTree().CreateTimer(0.08f), "timeout");

        // Final check before releasing swing
        if (IsInstanceValid(_logic))
        {
            _logic.IsCharging = false;
            _logic.EmitSignal(PlayerLogic.SignalName.OnSwing, "forehand_swing");
            _logic.PerformSwing();
        }
    }
}