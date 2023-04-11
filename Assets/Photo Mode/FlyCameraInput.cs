using UnityEngine;

namespace PhotoMode
{
	public interface FlyCameraInput
	{
		Vector3 Move { get; }
		Vector2 MouseMove { get; }
		Vector3 JoystickMove { get; }
		Vector2 JoystickRightStick { get; }
		float MouseWheel { get; }
		float JoystickTrigger { get; }
		bool Shift { get; }
		bool Alt { get; }
		bool LeftMouse { get; }
		bool RightMouse { get; }
		bool MiddleMouse { get; }
		bool Button4 { get; }
		float Roll { get; }
	}
}
