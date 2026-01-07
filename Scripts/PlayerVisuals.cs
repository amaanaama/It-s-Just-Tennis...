using Godot;
using System;

public partial class PlayerVisuals : Node2D
{
	[Export] public PlayerLogic Brain;
	[Export] public Sprite2D CharacterSprite;
	[Export] public Sprite2D ShadowSprite;

	private float screenCenterX = 320;
	private float courtBottomY = 332;
	private float courtTopY = 52;
	private float narrowScale = 350f / 514f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Brain == null) return;

		float depthMultiplier = Brain.GlobalPosition.Y / Brain.FieldSize.Y;
		float screenY = Mathf.Lerp(courtTopY, courtBottomY, depthMultiplier);
		float currentSqueeze = Mathf.Lerp(narrowScale, 1.0f, depthMultiplier);

		float logicXCentered = Brain.GlobalPosition.X - (Brain.FieldSize.X / 2);
		float xPixelScale = 514f/ Brain.FieldSize.X;

		float screenX = screenCenterX + (logicXCentered * xPixelScale * currentSqueeze);

		float shadowScaleFactor = Mathf.Remap(Brain.Altitude, 0, 60, 1.0f, 0.6f);

		ShadowSprite.Scale = new Vector2(shadowScaleFactor, shadowScaleFactor);
		
		this.GlobalPosition = new Vector2(screenX, screenY);
		CharacterSprite.Position = new Vector2(0, -Brain.Altitude);
		this.Scale = new Vector2(currentSqueeze, currentSqueeze);

		ZIndex = (int)screenY;
	}
}