using Godot;

// ===========================================================================
// EnvironmentSettingsApplier
// ---------------------------------------------------------------------------
// Attach this to the WorldEnvironment node (or any node) in your actual
// gameplay scene (e.g. Mapa.tscn). It reads the values saved by
// SettingsManager and applies the ones that need a real Environment/Camera
// to exist: SSAO, SSIL and FOV.
//
// Drag the WorldEnvironment and the active Camera3D into the Export slots.
// ===========================================================================
public partial class EnvironmentSettingsApplier : Node
{
	[Export] public WorldEnvironment WorldEnv;
	[Export] public Camera3D PlayerCamera;

	public override void _Ready()
	{
		var settings = SettingsManager.Instance;
		if (settings == null)
		{
			GD.PrintErr("EnvironmentSettingsApplier: SettingsManager autoload not found.");
			return;
		}

		if (WorldEnv != null && WorldEnv.Environment != null)
		{
			WorldEnv.Environment.SsaoEnabled = settings.SsaoEnabled;
			WorldEnv.Environment.SsilEnabled = settings.SsilEnabled;
		}

		if (PlayerCamera != null)
		{
			PlayerCamera.Fov = settings.Fov;
		}
	}
}