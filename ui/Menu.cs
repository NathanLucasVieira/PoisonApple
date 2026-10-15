using Godot;
using System;

public partial class Menu : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Button>("Play").Pressed += Play;
		GetNode<Button>("Options").Pressed += Options;
		GetNode<Button>("Credits").Pressed += Credits;
		GetNode<Button>("Quit").Pressed += Quit;
	}

	private void Play()
	{	
		GetTree().ChangeSceneToFile("res://mapas/Mapa.tscn");
	}

	private void Credits()
	{
		GetTree().ChangeSceneToFile("res://ui/credits.tscn");
	}
	private void Options()
	{	
		GetTree().ChangeSceneToFile("res://ui/options.tscn");
	}
	
	public void ReturnToMenu()
	{	
		GetTree().ChangeSceneToFile("res://ui/control.tscn");
	}
	private void Quit()
	{
		GetTree().Quit();
	}
}
