using Godot;

public partial class FpCam : Node3D
{
	[Export] public float MouseSensitivity = 0.002f;
	[Export] public float VerticalLimit = Mathf.DegToRad(89f);

	private Node3D _player;
	private Node3D _head; // é o próprio "this"

	public override void _Ready()
	{
		// Script está no head; o Player é o pai do head
		_head   = this;
		_player = GetParentOrNull<Node3D>();

		if (_player == null)
		{
			GD.PushError("FpCam: esperado player/head. 'head' não tem pai Node3D.");
			return;
		}

		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (_player == null) return;

		// ---- Mouse look ----
		if (@event is InputEventMouseMotion mm
			&& Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			// Yaw no Player
			_player.RotateY(-mm.Relative.X * MouseSensitivity);

			// Pitch no próprio head
			float pitch = Mathf.Clamp(
				_head.Rotation.X - mm.Relative.Y * MouseSensitivity,
				-VerticalLimit,
				VerticalLimit
			);
			_head.Rotation = new Vector3(pitch, 0f, 0f);
		}

		// ---- ESC ----
		if (@event is InputEventKey k && k.Pressed && !k.Echo && k.Keycode == Key.Escape)
			Input.MouseMode = Input.MouseModeEnum.Visible;

		// ---- Clique recaptura ----
		if (@event is InputEventMouseButton mb && mb.Pressed
			&& Input.MouseMode == Input.MouseModeEnum.Visible)
			Input.MouseMode = Input.MouseModeEnum.Captured;
	}
}
