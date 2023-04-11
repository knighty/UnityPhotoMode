using UnityEngine;

namespace PhotoMode
{
	public class BlenderFlyCameraInputMode : FlyCameraInputMode
	{
		public void Process(FlyCameraInput input, ref FlyCameraControl control)
		{
			// KB + M
			if (input.MiddleMouse)
			{
				if (input.Shift)
				{
					control.moveAlongOrigin = new Vector2(-input.MouseMove.x, -input.MouseMove.y);
				}
				else
				{
					control.rotateAroundOrigin = new Vector2(-input.MouseMove.x, -input.MouseMove.y);
				}
			}
			else
			{
				if (input.RightMouse)
				{
					control.yaw = input.MouseMove.x;
					control.pitch = -input.MouseMove.y;
					control.adjustFocalLength = input.MouseWheel;
					control.hideCursor = true;
				}
				else
				{
					if (input.Shift) control.adjustFstop = -input.MouseWheel;
					else if (input.Alt) control.adjustExposure = input.MouseWheel;
					else control.moveOrigin = -input.MouseWheel;
				}

				control.fly = input.Move;
				if (input.Shift)
					control.speedMultiplier = 3;

				control.roll = input.Roll;
			}

			// Joystick
			control.fly += input.JoystickMove;
			control.yaw += input.JoystickRightStick.x;
			control.pitch += input.JoystickRightStick.y;
			control.adjustFocalLength = input.JoystickTrigger;
		}
	}
}
