using System;
using UnityEngine;

namespace PhotoMode
{
	[CreateAssetMenu(fileName = "Square Aperture", menuName = "Photo Mode/Apertures/Square", order = 1)]
	public class SquareAperture : ApertureShape
	{
		public override float GetMagnitudeAt(float x, float y)
		{
			return 1;
		}

		public override Vector3 GetRandomPoint(int seed, int total)
		{
			UnityEngine.Random.InitState(seed);
			return new Vector3(UnityEngine.Random.value * 2 - 1, UnityEngine.Random.value * 2 - 1, 1);
		}
	}
}