using System;
using UnityEngine;

namespace PhotoMode
{
	public abstract class ApertureShape : ScriptableObject
	{
		public abstract Vector3 GetRandomPoint(int seed);
		public void RenderPreview(Texture2D texture, int samplesPerPixel = 1)
		{
			if (samplesPerPixel == 0)
				throw new ArgumentOutOfRangeException(nameof(samplesPerPixel));

			float[] pixels = new float[texture.width * texture.height];
			float max = 0;
			for (int y = 0; y < texture.height; y++)
			{
				for (int x = 0; x < texture.width; x++)
				{
					float magnitude = 0;
					for (float xx = 0; xx < 1; xx += 1.0f / samplesPerPixel)
					{
						for (float yy = 0; yy < 1; yy += 1.0f / samplesPerPixel)
						{
							magnitude += GetMagnitudeAt((x + xx) / texture.width * 2 - 1, (y + yy) / texture.height * 2 - 1);
						}
					}
					max = Mathf.Max(max, magnitude);
					pixels[y * texture.width + x] = magnitude;
				}
			}
			Color[] colors = new Color[pixels.Length];
			for (int i = 0; i < texture.width * texture.height; i++)
			{
				colors[i] = new Color(1, 1, 1, pixels[i] / max);
			}
			texture.SetPixels(colors);
			texture.Apply();
		}
		public abstract float GetMagnitudeAt(float x, float y);
	}
}