using UnityEngine;
using UnityEngine.UIElements;

namespace PhotoMode
{
	public class StandardFlyCameraInput : FlyCameraInput
	{
		public Vector3 Move
		{
			get
			{
				Vector3 move = Vector3.zero;
				//Vector3.forward * Input.GetAxis("Vertical");
				//move += Vector3.right * Input.GetAxis("Horizontal");

				if (Input.GetKey(KeyCode.W))
					move += Vector3.forward;
				if (Input.GetKey(KeyCode.S))
					move -= Vector3.forward;
				if (Input.GetKey(KeyCode.A))
					move -= Vector3.right;
				if (Input.GetKey(KeyCode.D))
					move += Vector3.right;

				if (Input.GetKey(KeyCode.Space))
					move += Vector3.up;
				if (Input.GetKey(KeyCode.LeftControl))
					move -= Vector3.up;

				return move;
			}
		}

		public Vector3 JoystickMove
		{
			get
			{
				Vector3 move = Vector3.zero;
				move += Vector3.forward * -Input.GetAxis("Vertical");
				move += Vector3.right * Input.GetAxis("Horizontal");

				if (Input.GetButton("Jump"))
					move += Vector3.up;

				return move;
			}
		}

		public Vector2 MouseMove
		{
			get
			{
				return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
			}
		}

		public Vector2 JoystickRightStick
		{
			get
			{
				return new Vector2(Input.GetAxis("Joystick Yaw"), Input.GetAxis("Joystick Pitch"));
			}
		}

		public float MouseWheel
		{
			get
			{
				return Input.GetAxis("Mouse ScrollWheel");
			}
		}

		public bool Shift
		{
			get
			{
				return Input.GetKey(KeyCode.LeftShift);
			}
		}

		public bool Alt
		{
			get
			{
				return Input.GetKey(KeyCode.LeftAlt);
			}
		}

		public bool LeftMouse
		{
			get
			{
				return Input.GetMouseButton((int)MouseButton.LeftMouse);
			}
		}

		public bool RightMouse
		{
			get
			{
				return Input.GetMouseButton((int)MouseButton.RightMouse);
			}
		}

		public bool MiddleMouse
		{
			get
			{
				return Input.GetMouseButton((int)MouseButton.MiddleMouse);
			}
		}

		public bool Button4
		{
			get
			{
				return false;
			}
		}

		public float Roll
		{
			get
			{
				float r = 0;
				if (Input.GetKey(KeyCode.Q))
					r += 1;
				if (Input.GetKey(KeyCode.E))
					r -= 1;

				return r;
			}
		}

		public float JoystickTrigger
		{
			get
			{
				return Input.GetAxis("FlyCamera Zoom");
			}
		}
	}
}
