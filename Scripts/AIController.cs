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
        _ball = GetTree().GetFirstNodeInGroup("ball_logic") as BallLogic;
        
        if (_logic != null)
            _targetX = _logic.GlobalPosition.X; 
            
        GD.Print(_ball == null ? "AI: Ball NOT found initially!" : "AI: Ball found and tracking.");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_logic == null) return;

        // 1. SAFETY CHECK
        if (!IsInstanceValid(_ball))
        {
            _ball = GetTree().GetFirstNodeInGroup("ball_logic") as BallLogic;
            if (!IsInstanceValid(_ball)) return;
        }

        // 2. POSITION SYNC
        GlobalPosition = _logic.GlobalPosition;

        // 3. TARGET CALCULATION
        if (_ball.Velocity.Y < -10)
        {
            // Predict where the ball will be when it reaches AI's Y-line (100)
            float distanceY = Mathf.Abs(_ball.GlobalPosition.Y - 100.0f);
            float timeToReach = distanceY / Mathf.Abs(_ball.Velocity.Y);
            _targetX = _ball.GlobalPosition.X + (_ball.Velocity.X * timeToReach);
        }
        else
        {
            _targetX = _ball.GlobalPosition.X;
        }

        _targetX = Mathf.Clamp(_targetX, 50, _logic.FieldSize.X - 50);

        // 4. MOVEMENT
        float diffX = _targetX - _logic.GlobalPosition.X;
        float diffY = 100.0f - _logic.GlobalPosition.Y; 

        Vector2 moveDir = Vector2.Zero;
        if (Mathf.Abs(diffX) > 10) moveDir.X = Mathf.Sign(diffX);
        if (Mathf.Abs(diffY) > 10) moveDir.Y = Mathf.Sign(diffY);

        _logic.Velocity = new Vector2(moveDir.X * _logic.HorizontalSpeed, moveDir.Y * _logic.VerticalSpeed);

        // 5. AUTO-SWING (Synchronized with PlayerLogic Reach)
        float hitDiffX = Mathf.Abs(_ball.GlobalPosition.X - _logic.GlobalPosition.X);
        float hitDiffY = Mathf.Abs(_ball.GlobalPosition.Y - _logic.GlobalPosition.Y);

        // NEW: Strict height check. AI only swings if ball is below MaxHitHeight
        bool isBallHittable = _ball.Altitude <= _logic.MaxHitHeight;
        bool isWithinReach = hitDiffX < _logic.ReachX && hitDiffY < _logic.ReachY;

        if (isWithinReach && isBallHittable && !_logic.IsCharging)
        {
            // Extra safety: only swing if ball is on AI side of court
            if (_ball.GlobalPosition.Y < 300)
            {
                TriggerSwing();
            }
        }

        if (ShowDebug) QueueRedraw();
    }

    public override void _Draw()
    {
        if (!ShowDebug || _logic == null || !IsInstanceValid(_ball)) return;

        Vector2 localTarget = ToLocal(new Vector2(_targetX, 100.0f));
        Vector2 localBall = ToLocal(_ball.GlobalPosition);

        DrawLine(Vector2.Zero, localTarget, new Color(1, 1, 0, 0.5f), 2.0f);
        DrawCircle(localTarget, 8.0f, new Color(1, 0, 0, 0.8f));
        DrawLine(Vector2.Zero, localBall, new Color(0, 0.5f, 1, 0.3f), 1.0f);
    }

    private async void TriggerSwing()
    {
        if (_logic == null) return;

        _logic.IsCharging = true;
        _logic.EmitSignal(PlayerLogic.SignalName.OnSwing, "forehand_charge");
        
        // Wait a tiny bit (simulates human reaction/wind-up)
        await ToSignal(GetTree().CreateTimer(0.08f), "timeout");

        if (IsInstanceValid(_logic))
        {
            _logic.IsCharging = false;
            _logic.EmitSignal(PlayerLogic.SignalName.OnSwing, "forehand_swing");
            _logic.PerformSwing();
        }
    }
}