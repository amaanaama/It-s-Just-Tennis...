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

	
	
	public override void _Ready()
	{
		if (AnimSprite == null) AnimSprite = GetNode<AnimatedSprite2D>("AnimSprite2D");
		Brain.Connect(PlayerLogic.SignalName.OnSwing, Callable.From<string>(OnSwingRequested));
	}

	public override void _Process(double delta)
	{
	
		if (Brain == null || AnimSprite == null) return;

		// Perspective Math
		float depthMultiplier = Brain.GlobalPosition.Y / Brain.FieldSize.Y;
		float screenY = Mathf.Lerp(courtTopY, courtBottomY, depthMultiplier);
		float currentSqueeze = Mathf.Lerp(narrowScale, 1.0f, depthMultiplier);
		float logicXCentered = Brain.GlobalPosition.X - (Brain.FieldSize.X / 2);
		float xPixelScale = 600f / Brain.FieldSize.X;
		float screenX = screenCenterX + (logicXCentered * xPixelScale * currentSqueeze);

		// Visual Updates
		this.GlobalPosition = new Vector2(screenX, screenY);
		AnimSprite.Position = new Vector2(0, -Brain.Altitude);
		
		float shadowScale = Mathf.Remap(Brain.Altitude, 0, 60, 1.0f, 0.6f);
		if (ShadowSprite != null) ShadowSprite.Scale = new Vector2(shadowScale, shadowScale);

		float baseScale = 1.5f;
		this.Scale = new Vector2(currentSqueeze * baseScale, currentSqueeze * baseScale);
		ZIndex = (int)screenY;

		HandleAnimations();
	}

	private void HandleAnimations()
	{
		string currentAnim = AnimSprite.Animation.ToString();

		// If charging or the swing is playing, don't change animation
		if (Brain.IsCharging || (currentAnim.Contains("forehand") && AnimSprite.IsPlaying()))
		{
			return;
		}

		float vx = Brain.Velocity.X;
		float vy = Brain.Velocity.Y;
		string p = "p" + Brain.PlayerID;

		if (Mathf.Abs(vx) > 10 || Mathf.Abs(vy) > 10)
		{
			string side = (vx < 0) ? "run_left" : "run_right";
			if (currentAnim != p + side) AnimSprite.Play(p + side);
		}
		else
		{
			if (currentAnim != p + "idle") AnimSprite.Play(p + "idle");
		}
	}

	private void OnSwingRequested(string actionType)
	{
		string fullAnimName = "p" + Brain.PlayerID + "_forehand";

		if (actionType.Contains("charge"))
		{
			AnimSprite.Play(fullAnimName);
			AnimSprite.SetFrameAndProgress(0, 0);
			AnimSprite.Pause();
		}
		else
		{
			AnimSprite.Play(); // Release!
			if (!AnimSprite.IsConnected("animation_finished", Callable.From(OnAnimFinished)))
				AnimSprite.AnimationFinished += OnAnimFinished;
		}
	}

	private void OnAnimFinished()
	{
		AnimSprite.AnimationFinished -= OnAnimFinished;
		AnimSprite.Play("p" + Brain.PlayerID + "idle");
	}
}
