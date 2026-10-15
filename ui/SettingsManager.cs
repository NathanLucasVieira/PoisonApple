using Godot;

// ===========================================================================
// SettingsManager
// ---------------------------------------------------------------------------
// Autoload (singleton). Add this in Project Settings > Autoload as "Settings".
// Holds every option value in memory, saves/loads them to user://settings.cfg,
// and applies the ones that can be applied globally (vsync, fullscreen,
// fps cap, master volume, 3D scaling, anti-aliasing).
//
// Options that depend on the game scene (SSAO, SSIL, Shadow Quality, FOV)
// are only stored here; EnvironmentSettingsApplier.cs reads them and applies
// them inside the actual game scene.
// ===========================================================================
public partial class SettingsManager : Node
{
	public static SettingsManager Instance { get; private set; }

	private const string SavePath = "user://settings.cfg";
	private const string Section = "options";

	// ---- Visuals ----
	public float Scaling3D = 1.0f;          // 0.25 - 2.0
	public int AntiAliasing = 0;            // 0=Off 1=MSAA2x 2=MSAA4x 3=MSAA8x 4=FXAA
	public int VSyncMode = 1;               // 0=Off 1=Enabled 2=Adaptive 3=Mailbox
	public int FullscreenMode = 0;          // 0=Windowed 1=Fullscreen 2=ExclusiveFullscreen 3=Maximized
	public int FpsCap = 0;                  // 0 = unlimited
	public int ShadowQuality = 2;           // 0=HardNoFilter 1=SoftLow 2=SoftMedium 3=SoftHigh 4=SoftUltra
	public bool SsaoEnabled = true;
	public bool SsilEnabled = false;

	// ---- Gameplay ----
	public float Fov = 75.0f;               // 30 - 120

	// ---- Audio ----
	public float MasterVolumeDb = 0.0f;     // -80 (mute) to 0 (max, dB scale)

	public override void _Ready()
	{
		Instance = this;
		Load();
		ApplyGlobalSettings();
	}

	// -----------------------------------------------------------------
	// Persistence
	// -----------------------------------------------------------------
	public void Save()
	{
		var cfg = new ConfigFile();

		cfg.SetValue(Section, "scaling_3d", Scaling3D);
		cfg.SetValue(Section, "anti_aliasing", AntiAliasing);
		cfg.SetValue(Section, "vsync_mode", VSyncMode);
		cfg.SetValue(Section, "fullscreen_mode", FullscreenMode);
		cfg.SetValue(Section, "fps_cap", FpsCap);
		cfg.SetValue(Section, "shadow_quality", ShadowQuality);
		cfg.SetValue(Section, "ssao_enabled", SsaoEnabled);
		cfg.SetValue(Section, "ssil_enabled", SsilEnabled);
		cfg.SetValue(Section, "fov", Fov);
		cfg.SetValue(Section, "master_volume_db", MasterVolumeDb);

		var err = cfg.Save(SavePath);
		if (err != Error.Ok)
			GD.PrintErr($"SettingsManager: failed to save settings ({err})");
	}

	public void Load()
	{
		var cfg = new ConfigFile();
		var err = cfg.Load(SavePath);
		if (err != Error.Ok)
		{
			// No save file yet (first run) - keep defaults.
			return;
		}

		Scaling3D       = (float)cfg.GetValue(Section, "scaling_3d", Scaling3D);
		AntiAliasing    = (int)cfg.GetValue(Section, "anti_aliasing", AntiAliasing);
		VSyncMode       = (int)cfg.GetValue(Section, "vsync_mode", VSyncMode);
		FullscreenMode  = (int)cfg.GetValue(Section, "fullscreen_mode", FullscreenMode);
		FpsCap          = (int)cfg.GetValue(Section, "fps_cap", FpsCap);
		ShadowQuality   = (int)cfg.GetValue(Section, "shadow_quality", ShadowQuality);
		SsaoEnabled     = (bool)cfg.GetValue(Section, "ssao_enabled", SsaoEnabled);
		SsilEnabled     = (bool)cfg.GetValue(Section, "ssil_enabled", SsilEnabled);
		Fov             = (float)cfg.GetValue(Section, "fov", Fov);
		MasterVolumeDb  = (float)cfg.GetValue(Section, "master_volume_db", MasterVolumeDb);
	}

	// -----------------------------------------------------------------
	// Apply - things that can be applied without a game scene reference
	// -----------------------------------------------------------------
	public void ApplyGlobalSettings()
	{
		ApplyScaling3D(Scaling3D);
		ApplyAntiAliasing(AntiAliasing);
		ApplyVSync(VSyncMode);
		ApplyFullscreen(FullscreenMode);
		ApplyFpsCap(FpsCap);
		ApplyMasterVolume(MasterVolumeDb);
		ApplyShadowQuality(ShadowQuality);
	}

	public void ApplyScaling3D(float value)
	{
		Scaling3D = value;
		var vp = GetViewport();
		if (vp != null)
			vp.Scaling3DScale = value;
	}

	public void ApplyAntiAliasing(int mode)
	{
		AntiAliasing = mode;
		var vp = GetViewport();
		if (vp == null) return;

		// Reset both first
		vp.Msaa3D = Viewport.Msaa.Disabled;
		vp.ScreenSpaceAA = Viewport.ScreenSpaceAAEnum.Disabled;

		switch (mode)
		{
			case 1: vp.Msaa3D = Viewport.Msaa.Msaa2X; break;
			case 2: vp.Msaa3D = Viewport.Msaa.Msaa4X; break;
			case 3: vp.Msaa3D = Viewport.Msaa.Msaa8X; break;
			case 4: vp.ScreenSpaceAA = Viewport.ScreenSpaceAAEnum.Fxaa; break;
			// case 0 (Off) already handled by the reset above
		}
	}

	public void ApplyVSync(int mode)
	{
		VSyncMode = mode;
		DisplayServer.WindowSetVsyncMode((DisplayServer.VSyncMode)mode);
	}

	public void ApplyFullscreen(int mode)
	{
		FullscreenMode = mode;
		DisplayServer.WindowSetMode((DisplayServer.WindowMode)mode);
	}

	public void ApplyFpsCap(int fps)
	{
		FpsCap = fps;
		Engine.MaxFps = fps;
	}

	public void ApplyMasterVolume(float db)
	{
		MasterVolumeDb = db;
		int busIndex = AudioServer.GetBusIndex("Master");
		if (busIndex >= 0)
			AudioServer.SetBusVolumeDb(busIndex, db);
	}

	public void ApplyShadowQuality(int quality)
	{
		ShadowQuality = quality;
		var q = (RenderingServer.ShadowQuality)quality;
		RenderingServer.DirectionalSoftShadowFilterSetQuality(q);
		RenderingServer.PositionalSoftShadowFilterSetQuality(q);
	}

	// Scene-dependent settings just get stored; EnvironmentSettingsApplier
	// reads them from Instance when the game scene loads.
	public void SetSsao(bool enabled) { SsaoEnabled = enabled; }
	public void SetSsil(bool enabled) { SsilEnabled = enabled; }
	public void SetFov(float fov) { Fov = fov; }
}