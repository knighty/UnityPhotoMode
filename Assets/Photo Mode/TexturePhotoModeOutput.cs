using UnityEngine;

namespace PhotoMode
{
	public class TexturePhotoModeOutput : PhotoModeOutput
	{
		private Texture2D texture;
		private RenderTextureFormat format;

		public RenderTextureFormat RenderTextureFormat => format;

		public TexturePhotoModeOutput(Texture2D texture = null, RenderTextureFormat format = RenderTextureFormat.Default)
		{
			this.texture = texture;
			this.format = format;
		}

		public void Output(RenderTexture renderTexture)
		{
			if (texture == null)
			{
				texture = new Texture2D(renderTexture.width, renderTexture.height);
			}

			var oldRt = RenderTexture.active;
			RenderTexture.active = renderTexture;
			texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
			texture.Apply();
			RenderTexture.active = oldRt;
		}
	}
}