using Godot;
using System;

public partial class PlayerInteract : RayCast3D
{

    public override void _PhysicsProcess(double delta)
    {
        if (IsColliding())
        {
            var hit = GetCollider();

            if (hit is Node node && node.Name == "door")
            {
                if (Input.IsActionJustPressed("interacao"))
                {
                    hit = node.GetParent()?.GetParent()?.GetParent();
                    hit?.Call("toggle_door");
                }
            }
        }
    }

}
