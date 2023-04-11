using System.Windows.Forms;
using UnityEngine;
using UnityEngine.EventSystems;
using Cursor = UnityEngine.Cursor;

namespace PhotoMode
{
	public class FlyCamera : MonoBehaviour
	{
		public enum FlyControlMode
		{
			Blender,
			Unity
		}

		[SerializeField] protected float friction = 0.1f;
		[SerializeField] protected float frictionTime = 0.3f;
		[SerializeField] private float speed = 0.4f;
		[SerializeField] protected float shiftSpeed = 3f;
		[SerializeField] private PhotoModeSettings settings;

		protected Vector3 position = new Vector3(0, 0, 0);
		protected Quaternion rotation = Quaternion.identity;
		protected Vector3 yawPitchRoll = Vector3.zero;
		protected Vector3 velocity = Vector3.zero;

		private FlyCameraControl flyCameraControl = new FlyCameraControl();
		private FlyCameraInput flyCameraInput;
		private FlyCameraInputMode flyCameraInputMode;
		private FlyControlMode controlMode = FlyControlMode.Blender;
		protected Vector3 originPosition = new Vector3(0, 0, 10);

		private BlenderFlyCameraInputMode blenderFlyCameraInputMode = new BlenderFlyCameraInputMode();
		private UnityFlyCameraInputMode unityFlyCameraInputMode = new UnityFlyCameraInputMode();

		public bool Enabled { get; set; } = false;
		public PhotoModeSettings Settings { get => settings; set => settings = value; }
		public float Speed { get => speed; set => speed = value; }

		public FlyControlMode ControlMode { get => controlMode; set => controlMode = value; }
		public FlyCameraInput FlyCameraInput { get => flyCameraInput; set => flyCameraInput = value; }

		private void Start()
		{
			this.enabled = false;

			if (flyCameraInput == null)
			{
				flyCameraInput = new StandardFlyCameraInput();
			}
		}

		private void OnEnable()
		{
			position = transform.position;
		}

		void Update()
		{
			float dt = Time.unscaledDeltaTime;

			flyCameraControl.Reset();

			switch (controlMode)
			{
				case FlyControlMode.Blender:
					flyCameraInputMode = blenderFlyCameraInputMode; break;
				case FlyControlMode.Unity:
					flyCameraInputMode = unityFlyCameraInputMode; break;
			}

			if (flyCameraInputMode != null)
				flyCameraInputMode.Process(flyCameraInput, ref flyCameraControl);

			Vector3 forward = rotation * Vector3.forward;
			Vector3 up = rotation * Vector3.up;
			Vector3 right = rotation * Vector3.right;

			Vector3 move = Vector3.zero;

			FlyCameraControl control = flyCameraControl;

			move += forward * control.fly.z;
			move += right * control.fly.x;
			move += up * control.fly.y;
			float speedMultiplier = control.speedMultiplier;

			yawPitchRoll.z += control.roll * dt * 50;

			// Moving on origin
			position += (right * control.moveAlongOrigin.x + up * control.moveAlongOrigin.y) * originPosition.magnitude * 0.01f;

			// Rotating on origin
			float mult = GetComponent<Camera>().fieldOfView / 45.0f;
			Quaternion r1 = Quaternion.Euler(yawPitchRoll.y, yawPitchRoll.x, 0f);
			yawPitchRoll.x += control.rotateAroundOrigin.x * mult;
			yawPitchRoll.y += control.rotateAroundOrigin.y * mult;
			yawPitchRoll.y = Mathf.Clamp(yawPitchRoll.y, -89, 89);
			Quaternion r2 = Quaternion.Euler(yawPitchRoll.y, yawPitchRoll.x, 0f);
			position = position + (r2 * -originPosition - r1 * -originPosition);

			// Zoom origin
			Vector3 o = originPosition;
			originPosition.z *= (1 + control.moveOrigin * 0.1f);
			originPosition.z = Mathf.Clamp(originPosition.z, 0.1f, 100.0f);
			position += rotation * -originPosition - rotation * -o;

			// Fly move
			if (move.sqrMagnitude > 0)
				move.Normalize();

			float frictionFactor = Mathf.Pow(friction, dt / frictionTime);
			velocity = velocity * frictionFactor;
			velocity += move * dt * speed * speedMultiplier;

			// Cursor state
			Cursor.lockState = control.hideCursor ? CursorLockMode.Locked : CursorLockMode.None;
			Cursor.visible = !control.hideCursor;

			// Rotate
			float yprMagnitude = GetComponent<Camera>().fieldOfView / 45.0f;
			yawPitchRoll.x += control.yaw * yprMagnitude;
			yawPitchRoll.y += control.pitch * yprMagnitude;

			// Settings
			if (settings != null)
			{
				if (control.adjustFocalLength != 0)
					settings.FocalLength.Value = Mathf.Clamp(settings.FocalLength * (1 + control.adjustFocalLength * 0.1f), 1, 400);
				if (control.adjustFstop != 0)
					settings.FStop.Value = Mathf.Clamp(settings.FStop * (1 + control.adjustFstop * 0.1f), 1, 16);
				if (control.adjustExposure != 0)
					settings.Exposure.Value = Mathf.Clamp(settings.Exposure + control.adjustExposure * 0.1f, -4, 4);
			}

			Quaternion rotate = Quaternion.Euler(0f, yawPitchRoll.x, 0f) * Quaternion.Euler(yawPitchRoll.y, 0f, 0f) * Quaternion.Euler(0f, 0f, yawPitchRoll.z);

			rotation = rotate;
			position += velocity;

			gameObject.transform.position = position;
			gameObject.transform.rotation = rotation;
		}
	}
}
