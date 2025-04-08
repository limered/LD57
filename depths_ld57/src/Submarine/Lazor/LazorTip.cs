using Godot;
using System;

public partial class LazorTip : Area2D
{
	public CollisionShape2D Collider { get; private set; }

	public override void _Ready()
	{
		Collider = GetNode<CollisionShape2D>("CollisionShape2D");
	}
}
