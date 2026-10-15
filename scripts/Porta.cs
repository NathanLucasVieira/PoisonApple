using Godot;
using System;

public partial class Porta : Node
{
    private bool abrir;
    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        _animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");

        if (_animationPlayer == null)
        {
            GD.PrintErr("Porta: AnimationPlayer não encontrado em 'AnimationPlayer'.");
        }
    }

    public void toggle_door()
    {
        if (_animationPlayer == null)
        {
            return;
        }

        var animacao = abrir ? "closeDoor" : "openDoor";

        if (!_animationPlayer.HasAnimation(animacao))
        {
            GD.PrintErr($"Porta: animação '{animacao}' não foi encontrada no AnimationPlayer.");
            return;
        }

        abrir = !abrir;
        _animationPlayer.Play(animacao);
    }
}
