using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

namespace PhotoMode
{
	public class FlyCamera : MonoBehaviour
	{
		[SerializeField] protected float friction = 0.1f;
		[SerializeField] protected float frictionTime = 0.3f;
		[SerializeField] private float speed = 0.4f;
		[SerializeField] protected float shiftSpeed = 3f;
		[SerializeField] private PhotoModeSettings settings;

		protected Vector3 position = new Vector3(0, 0, 0);
		protected Quaternion rotation = Quaternion.identity;
		protected Vector3 yawPitchRoll = Vector3.zero;
		protected Vector3 velocity = Vector3.zero;

		public bool Enabled { get; set; } = false;
		public PhotoModeSettings Settings { get => settings; set => settings = value; }
		public float Speed { get => speed; set => speed = value; }

		private void Start()
		{
			this.enabled = false;
		}

		private void OnEnable()
		{
			position = transform.position;
		}

		void Update()
		{
			/*if (Input.GetKeyDown(KeyCode.P))
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
				return;*/ 

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
				yawPitchRoll.z += dt * 50;
			if (Input.GetKey(KeyCode.E))
				yawPitchRoll.z -= dt * 50;

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
				float mult = GetComponent<Camera>().fieldOfView / 45.0f;
				yawPitchRoll.x += mouse.x * mult;
				yawPitchRoll.y -= mouse.y * mult;
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
			else
			{
				if (settings != null)
				{
					if (Mathf.Abs(Input.mouseScrollDelta.y) > 0)
					{
						if (Input.GetKey(KeyCode.LeftShift))
						{
							settings.FStop.Value = Mathf.Clamp(settings.FStop * (1 - Input.mouseScrollDelta.y * 0.1f), 1, 16);
						}
						else if (Input.GetKey(KeyCode.LeftAlt))
						{
							settings.Exposure.Value = Mathf.Clamp(settings.Exposure + Input.mouseScrollDelta.y * 0.1f, -4, 4);
						}
						else
						{
							settings.FocalLength.Value = Mathf.Clamp(settings.FocalLength * (1 + Input.mouseScrollDelta.y * 0.1f), 1, 400);
						}
					}
				}
			}

			Quaternion rotate = Quaternion.Euler(0f, yawPitchRoll.x, 0f) * Quaternion.Euler(yawPitchRoll.y, 0f, 0f) * Quaternion.Euler(0f, 0f, yawPitchRoll.z);

			rotation = rotate;
			position += velocity;

			gameObject.transform.position = position;
			gameObject.transform.rotation = rotation;
		}
	}
}
