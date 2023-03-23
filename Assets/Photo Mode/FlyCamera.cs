using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

namespace PhotoMode
{
	public class FlyCamera : MonoBehaviour
	{
		public bool Enabled { get; set; } = false;

		protected Vector3 position = new Vector3(0, 0, 0);
		protected Quaternion rotation = Quaternion.identity;
		protected Vector3 yawPitchRoll = Vector3.zero;

		protected Vector3 velocity = Vector3.zero;
		[SerializeField] protected float friction = 0.1f;
		[SerializeField] protected float frictionTime = 0.3f;
		[SerializeField] protected float speed = 0.4f;
		[SerializeField] protected float shiftSpeed = 3f;

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.P))
			{
				if (!Enabled)
				{
					position = transform.position;
					rotation = transform.rotation;
					velocity = Vector3.zero;
					yawPitchRoll = Vector3.zero;
				}
				Enabled = !Enabled;
			}

			if (!Enabled)
				return;

			float dt = Time.unscaledDeltaTime;

			Vector3 forward = rotation * Vector3.forward;
			Vector3 up = rotation * Vector3.up;
			Vector3 right = rotation * Vector3.right;

			Vector3 move = Vector3.zero;

			float speedMultiplier = 1;

			if (Input.GetKey(KeyCode.W))
				move += forward;
			if (Input.GetKey(KeyCode.S))
				move -= forward;
			if (Input.GetKey(KeyCode.A))
				move -= right;
			if (Input.GetKey(KeyCode.D))
				move += right;
			if (Input.GetKey(KeyCode.Space))
				move += up;
			if (Input.GetKey(KeyCode.LeftControl))
				move -= up;
			if (Input.GetKey(KeyCode.LeftShift))
				speedMultiplier = shiftSpeed;

			if (Input.GetKey(KeyCode.Q))
				yawPitchRoll.z -= dt * 50;
			if (Input.GetKey(KeyCode.E))
				yawPitchRoll.z += dt * 50;

			if (Input.GetMouseButtonDown((int)MouseButton.MiddleMouse))
			{
				yawPitchRoll.y = 0;
				yawPitchRoll.z = 0;
			}

			if (move.sqrMagnitude > 0)
				move.Normalize();

			float frictionFactor = Mathf.Pow(friction, dt / frictionTime);
			velocity = velocity * frictionFactor;
			velocity += move * dt * speed * speedMultiplier;

			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;

			if (Input.GetMouseButton((int)MouseButton.RightMouse))
			{
				Vector2 mouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
				yawPitchRoll.x += mouse.x;
				yawPitchRoll.y -= mouse.y;
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
			Quaternion rotate = Quaternion.Euler(0f, yawPitchRoll.x, 0f) * Quaternion.Euler(yawPitchRoll.y, 0f, 0f) * Quaternion.Euler(0f, 0f, yawPitchRoll.z);

			rotation = rotate;
			position += velocity;

			gameObject.transform.position = position;
			gameObject.transform.rotation = rotation;
		}
	}
}
