using UnityEngine;

namespace PhotoMode
{
	public class UnityFlyCameraInputMode : FlyCameraInputMode
	{
		public void Process(FlyCameraInput input, ref FlyCameraControl control)
		{
			if (input.MiddleMouse)
			{
				control.moveAlongOrigin = new Vector2(-input.MouseMove.x, -input.MouseMove.y);
			}
			else if (input.LeftMouse)
			{
				if (input.Alt)
				{
					control.rotateAroundOrigin = new Vector2(input.MouseMove.x, -input.MouseMove.y);
				}
			}
			else
			{
				if (input.MiddleMouse)
				{
					control.yaw = input.MouseMove.x;
					control.pitch = -input.MouseMove.y;
					control.adjustFocalLength = input.MouseWheel;
					control.hideCursor = true;
				}


				control.fly = input.Move;
				control.roll = input.Roll;

				if (input.Shift) control.adjustFstop = -input.MouseWheel;
				else if (input.Alt) control.adjustExposure = input.MouseWheel;
				else control.moveOrigin = -input.MouseWheel;
			}
		}
	}
}
