using UnityEngine;

namespace PhotoMode
{
	public class FlyCameraControl
	{
		public Vector3 fly = Vector3.zero;
		public Vector2 rotateAroundOrigin = Vector2.zero;
		public Vector2 moveAlongOrigin = Vector2.zero;
		public float moveOrigin = 0;
		public float adjustFstop = 0;
		public float adjustExposure = 0;
		public float adjustFocalLength = 0;
		public float yaw = 0;
		public float pitch = 0;
		public float roll = 0;
		public float speedMultiplier = 1;
		public bool hideCursor = false;

		public void Reset()
		{
			fly = Vector3.zero;
			rotateAroundOrigin = Vector2.zero;
			moveAlongOrigin = Vector2.zero;
			moveOrigin = 0;
			adjustFstop = 0;
			adjustExposure = 0;
			adjustFocalLength = 0;
			yaw = 0;
			pitch = 0;
			roll = 0;
			speedMultiplier = 1;
			hideCursor = false;
		}
	}
}
