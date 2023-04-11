using UnityEngine;

namespace MathsUtilities
{
	public class Perlin
	{
		public static float Sample(float t, float seed, int octaves = 4, float persistance = 0.5f)
		{
			float r = 0;
			for (int i = 0; i < octaves; i++)
			{
				r += (Mathf.PerlinNoise(t * (float)Mathf.Pow(2.0f, i), seed + i * 7) * 2 - 1) * (float)Mathf.Pow(persistance, i);
			}
			return r;
		}

		public static Vector2 Sample2D(float t, float seed, int octaves = 4, float persistance = 0.5f)
		{
			return new Vector2(
				Sample(t, 0 + seed, octaves, persistance),
				Sample(t, 13 + seed, octaves, persistance)
			);
		}

		public static Vector3 Sample3D(float t, float seed, int octaves = 4, float persistance = 0.5f)
		{
			return new Vector3(
				Sample(t, 0 + seed, octaves, persistance),
				Sample(t, 13 + seed, octaves, persistance),
				Sample(t, 37 + seed, octaves, persistance)
			);
		}

		public static Vector4 Sample4D(float t, float seed, int octaves = 4, float persistance = 0.5f)
		{
			return new Vector4(
				Sample(t, 0 + seed, octaves, persistance),
				Sample(t, 13 + seed, octaves, persistance),
				Sample(t, 37 + seed, octaves, persistance),
				Sample(t, 53 + seed, octaves, persistance)
			);
		}
	}
}