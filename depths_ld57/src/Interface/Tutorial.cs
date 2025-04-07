using Godot;
using System;

public partial class Tutorial : Control
{
	[Export]
	public float showTime = 5;

	public void Start()
	{
		GetTree().CreateTimer(showTime).Timeout += () => {
			throw new NotImplementedException("show gameplay");
		};
	}
}
