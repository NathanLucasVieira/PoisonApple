using Godot;
using System;

public partial class Credits : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Button>("Return").Pressed += ReturnToMenu;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
   	public void ReturnToMenu()
    {
        GetTree().ChangeSceneToFile("res://ui/control.tscn");
    }
}
