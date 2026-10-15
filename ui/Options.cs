using Godot;

// ===========================================================================
// Options
// ---------------------------------------------------------------------------
// Attach this to the root Control of options.tscn.
// IMPORTANT: drag each UI node into the matching [Export] slot in the
// Inspector - do NOT rely on GetNode with string paths, this avoids the
// "Node not found" errors from before.
//
// Suggested node types (rename/create to match your UI):
//   Scaling3DSlider   -> HSlider   (min 0.25, max 2.0, step 0.05)
//   AntiAliasingBox   -> OptionButton (items: Off, MSAA 2x, MSAA 4x, MSAA 8x, FXAA)
//   VSyncBox          -> OptionButton (items: Off, Enabled, Adaptive, Mailbox)
//   FullscreenBox     -> OptionButton (items: Windowed, Fullscreen, Exclusive Fullscreen, Maximized)
//   FpsCapSpin        -> SpinBox   (min 0, max 300, step 1; 0 = unlimited)
//   ShadowQualityBox  -> OptionButton (items: Hard/No Filter, Low, Medium, High, Ultra)
//   SsaoCheck         -> CheckBox
//   SsilCheck         -> CheckBox
//   FovSlider         -> HSlider   (min 30, max 120, step 1)
//   MasterVolumeSlider-> HSlider   (min -80, max 0, step 1)   (dB scale)
//   ReturnButton      -> Button
// ===========================================================================
public partial class Options : Control
{
	[Export] public HSlider Scaling3DSlider;
	[Export] public OptionButton AntiAliasingBox;
	[Export] public OptionButton VSyncBox;
	[Export] public OptionButton FullscreenBox;
	[Export] public OptionButton FpsCapBox;
	[Export] public OptionButton ShadowQualityBox;
	[Export] public OptionButton SsaoCheck;
	[Export] public OptionButton SsilCheck;
	[Export] public HSlider FovSlider;
	[Export] public HSlider MasterVolumeSlider;
	[Export] public Button ReturnButton;

	private SettingsManager _settings;
	private bool _initializing = true;

	public override void _Ready()
	{
		_settings = SettingsManager.Instance;
		if (_settings == null)
		{
			GD.PrintErr("Options: SettingsManager autoload not found. Add SettingsManager.cs as an Autoload named 'Settings'.");
			return;
		}

		LoadValuesIntoUI();
		ConnectSignals();
		_initializing = false;
	}

	// -----------------------------------------------------------------
	// Populate the UI with the currently saved values
	// -----------------------------------------------------------------
	private void LoadValuesIntoUI()
	{
		if (Scaling3DSlider != null) Scaling3DSlider.Value = _settings.Scaling3D;
		if (AntiAliasingBox != null) AntiAliasingBox.Selected = _settings.AntiAliasing;
		if (VSyncBox != null) VSyncBox.Selected = _settings.VSyncMode;
		if (FullscreenBox != null) FullscreenBox.Selected = _settings.FullscreenMode;
		if (FpsCapBox != null) FpsCapBox.Selected = _settings.FpsCap;
		if (ShadowQualityBox != null) ShadowQualityBox.Selected = _settings.ShadowQuality;
		if (SsaoCheck != null) SsaoCheck.ButtonPressed = _settings.SsaoEnabled;
		if (SsilCheck != null) SsilCheck.ButtonPressed = _settings.SsilEnabled;
		if (FovSlider != null) FovSlider.Value = _settings.Fov;
		if (MasterVolumeSlider != null) MasterVolumeSlider.Value = _settings.MasterVolumeDb;
	}

	// -----------------------------------------------------------------
	// Hook up signals in code (avoid double-connecting in the editor too!)
	// -----------------------------------------------------------------
	private void ConnectSignals()
	{
		if (Scaling3DSlider != null) Scaling3DSlider.ValueChanged += OnScaling3DChanged;
		if (AntiAliasingBox != null) AntiAliasingBox.ItemSelected += OnAntiAliasingChanged;
		if (VSyncBox != null) VSyncBox.ItemSelected += OnVSyncChanged;
		if (FullscreenBox != null) FullscreenBox.ItemSelected += OnFullscreenChanged;
		if (FpsCapBox != null) FpsCapBox.ItemSelected += OnFpsCapChanged;
		if (ShadowQualityBox != null) ShadowQualityBox.ItemSelected += OnShadowQualityChanged;
		if (SsaoCheck != null) SsaoCheck.Toggled += OnSsaoToggled;
		if (SsilCheck != null) SsilCheck.Toggled += OnSsilToggled;
		if (FovSlider != null) FovSlider.ValueChanged += OnFovChanged;
		if (MasterVolumeSlider != null) MasterVolumeSlider.ValueChanged += OnMasterVolumeChanged;
		if (ReturnButton != null) ReturnButton.Pressed += ReturnToMenu;
	}

	// -----------------------------------------------------------------
	// Callbacks - apply immediately, then save
	// -----------------------------------------------------------------
	private void OnScaling3DChanged(double value)
	{
		if (_initializing) return;
		_settings.ApplyScaling3D((float)value);
		_settings.Save();
	}

	private void OnAntiAliasingChanged(long index)
	{
		if (_initializing) return;
		_settings.ApplyAntiAliasing((int)index);
		_settings.Save();
	}

	private void OnVSyncChanged(long index)
	{
		if (_initializing) return;
		_settings.ApplyVSync((int)index);
		_settings.Save();
	}

	private void OnFullscreenChanged(long index)
	{
		if (_initializing) return;
		_settings.ApplyFullscreen((int)index);
		_settings.Save();
	}

	private void OnFpsCapChanged(long index)
	{
		if (_initializing) return;
		_settings.ApplyFpsCap((int)index);
		_settings.Save();
	}

	private void OnShadowQualityChanged(long index)
	{
		if (_initializing) return;
		_settings.ApplyShadowQuality((int)index);
		_settings.Save();
	}

	private void OnSsaoToggled(bool pressed)
	{
		if (_initializing) return;
		_settings.SetSsao(pressed);
		_settings.Save();
	}

	private void OnSsilToggled(bool pressed)
	{
		if (_initializing) return;
		_settings.SetSsil(pressed);
		_settings.Save();
	}

	private void OnFovChanged(double value)
	{
		if (_initializing) return;
		_settings.SetFov((float)value);
		_settings.Save();
	}

	private void OnMasterVolumeChanged(double value)
	{
		if (_initializing) return;
		_settings.ApplyMasterVolume((float)value);
		_settings.Save();
	}

	private void ReturnToMenu()
	{
		GetTree().ChangeSceneToFile("res://ui/control.tscn");
	}
}