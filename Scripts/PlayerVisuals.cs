using Godot;
using System;

public partial class PlayerVisuals : Node2D
{
	[Export] public PlayerLogic Brain;
	[Export] public AnimatedSprite2D AnimSprite;
	[Export] public Sprite2D ShadowSprite;

	private float screenCenterX = 320;
	private float courtBottomY = 342;
	private float courtTopY = 30;
	private float narrowScale = 350f / 514f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (AnimSprite == null)
            AnimSprite = GetNode<AnimatedSprite2D>("AnimSprite2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Brain == null || AnimSprite == null) return;
		GD.Print($"Velocity: {Brain.Velocity} | Animation: {AnimSprite.Animation}");
		float depthMultiplier = Brain.GlobalPosition.Y / Brain.FieldSize.Y;
		float screenY = Mathf.Lerp(courtTopY, courtBottomY, depthMultiplier);
		float currentSqueeze = Mathf.Lerp(narrowScale, 1.0f, depthMultiplier);

		float logicXCentered = Brain.GlobalPosition.X - (Brain.FieldSize.X / 2);
		float xPixelScale = 600f/ Brain.FieldSize.X;

		float screenX = screenCenterX + (logicXCentered * xPixelScale * currentSqueeze);

		float shadowScaleFactor = Mathf.Remap(Brain.Altitude, 0, 60, 1.0f, 0.6f);

		ShadowSprite.Scale = new Vector2(shadowScaleFactor, shadowScaleFactor);
		
		this.GlobalPosition = new Vector2(screenX, screenY);
		AnimSprite.Position = new Vector2(0, -Brain.Altitude);
		float baseScale = 1.5f; 
		this.Scale = new Vector2(currentSqueeze * baseScale, currentSqueeze * baseScale);

		ZIndex = (int)screenY;

		HandleAnimations();
	}

	private void HandleAnimations()
{
    float vx = Brain.Velocity.X;
    float vy = Brain.Velocity.Y;
    string currentAnim = AnimSprite.Animation.ToString();

    // 1. PRIORITIZE THE SWING
    // If we are swinging, let that animation finish! 
    // Don't let movement or idle interrupt a hit.
    if (currentAnim.Contains("swing") && AnimSprite.IsPlaying())
    {
        return; 
    }

    // 2. CHECK MOVEMENT
    // We use a threshold of 10 to avoid "micro-movements" triggering the run
    if (Mathf.Abs(vx) > 10 || Mathf.Abs(vy) > 10)
    {
        if (vx < 0)
        {
            if (Brain.PlayerID == 2){
                if (currentAnim != "p2run_left") AnimSprite.Play("p2run_left");
            }
            else{
                if (currentAnim != "p1run_left") AnimSprite.Play("p1run_left");
            }
        }
        else
        {
            // If moving right OR moving purely up/down, use run_right
            if (Brain.PlayerID == 2){
                if (currentAnim != "p2run_right") AnimSprite.Play("p2run_right");
            }
            else{
                if (currentAnim != "p1run_right") AnimSprite.Play("p1run_right");
            }
        }
    }
    else
    {
        // 3. STANDING STILL

        if (currentAnim != "p1idle" || currentAnim != "p2idle")
        {
            if (Brain.PlayerID == 2){
                AnimSprite.Play("p2idle");
            }
            else{
                AnimSprite.Play("p1idle");
            }
        }
    }
}
}