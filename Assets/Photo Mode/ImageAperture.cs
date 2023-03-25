using System;
using UnityEngine;
using UnityEngine.XR;

namespace PhotoMode
{
	[CreateAssetMenu(fileName = "Image Aperture", menuName = "Photo Mode/Apertures/Image", order = 1)]
	public class ImageAperture : ApertureShape
	{
		[SerializeField] private Texture2D texture;
		[SerializeField, Range(1, 20)] private int resampleEmptyPixels = 5;

		public override float GetMagnitudeAt(float x, float y)
		{
			Color color = texture.GetPixel((int)((x * 0.5f + 0.5f) * texture.width), (int)((y * 0.5f + 0.5f) * texture.height));
			return color.r;
		}

		public override Vector3 GetRandomPoint(int seed)
		{
			UnityEngine.Random.InitState(seed);
			if (texture == null)
			{
				return new Vector3(UnityEngine.Random.value * 2 - 1, UnityEngine.Random.value * 2 - 1, 1);
			}

			float magnitude;
			int i = 0;
			Vector2 rand;
			do
			{
				rand = new Vector2(UnityEngine.Random.value * 2 - 1, UnityEngine.Random.value * 2 - 1);
				magnitude = GetMagnitudeAt(rand.x, rand.y);
				i++;
			} while (i < resampleEmptyPixels && magnitude == 0);
			return new Vector3(rand.x, rand.y, magnitude);
		}
	}
}