using UnityEngine;

namespace PhotoMode
{
	public class CameraCalculator : MonoBehaviour
	{
		[SerializeField, Range(10,300)] private float focalLength;
		[SerializeField, Range(2,50)] private float sensorSize;
		[SerializeField, Range(0.01f, 1000)] private float objectDistance;
		
		public void OnValidate()
		{
			Calculate(focalLength / 1000.0f, sensorSize / 1000.0f, objectDistance);
		}

		public void Calculate(float focalLength, float sensorSize, float objectDistance)
		{
			// Helpers
			float halfSensorSize = sensorSize / 2;

			// 1/f = 1/d + 1/i
			// f = focal distance
			// d = object (focus) distance
			// i = image distance
			float imageDistance = 1.0f / (1.0f / focalLength - 1.0f / objectDistance);
			float delta = imageDistance - focalLength;

			// fov = atan(sensor size / focal length)
			float fov = Mathf.Atan(halfSensorSize / imageDistance) * 2.0f;
			float halfFov = fov / 2.0f;

			float spread = Mathf.Tan(halfFov) * delta * 2;
			float spreadRatio = spread / sensorSize;

			float x = spread / focalLength * objectDistance;
			Debug.Log($"Image Distance: {imageDistance * 1000}mm, Field of View: {fov * Mathf.Rad2Deg}deg, Spread: {spread * 1000}mm, Spread ratio: {spreadRatio}, Aperture: {x * 1000}");

		}
	}
}
