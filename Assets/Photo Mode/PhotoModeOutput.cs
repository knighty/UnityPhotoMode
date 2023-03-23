using UnityEngine;

namespace PhotoMode
{
	public interface PhotoModeOutput
	{
		RenderTextureFormat RenderTextureFormat { get; }
		void Output(RenderTexture texture);
	}
}