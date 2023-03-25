using System;
using UnityEngine;
using UnityEngine.XR;

namespace PhotoMode
{
	[CreateAssetMenu(fileName = "Round Aperture", menuName = "Photo Mode/Apertures/Round", order = 1)]
	public class RoundAperture : ApertureShape
	{
		[SerializeField] private float noiseMin = 0.5f;
		[SerializeField] private float noiseMax = 1.0f;
		[SerializeField] private float noiseScale = 1.0f;
		[SerializeField] private bool fringeHighlight = true;
		[SerializeField] private float fringeExponent = 1;
		[SerializeField] private float fringeMultiplier = 1;

		public override float GetMagnitudeAt(float x, float y)
		{
			float m = new Vector2(x, y).magnitude;
			float noise = Mathf.PerlinNoise(x * noiseScale + 100, y * noiseScale + 100) * (noiseMax - noiseMin) + noiseMin;
			float fringe = 0;
			if (fringeHighlight)
			{
				float mag = Mathf.Clamp(m, 0, 1);
				fringe = -2 * Mathf.Pow(mag, 3) + 3 * Mathf.Pow(mag, 2);
				fringe = Mathf.Pow(fringe, fringeExponent) * fringeMultiplier;
			}
			float t = Mathf.Clamp((m - 0.99f) / (1.01f - 0.99f), 0, 1);
			t = t * t * (3.0f - 2.0f * t);
			return (noise + fringe) * (1 - t);
		}

		public override Vector3 GetRandomPoint(int seed)
		{
			UnityEngine.Random.InitState(seed);
			Vector2 rand = UnityEngine.Random.insideUnitCircle;
			//Debug.Log($"Called with seed: {seed}");
			return new Vector3(rand.x, rand.y, GetMagnitudeAt(rand.x, rand.y));
		}
	}
}