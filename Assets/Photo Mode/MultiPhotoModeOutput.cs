using System.Collections.Generic;
using UnityEngine;

namespace PhotoMode
{
	public class MultiPhotoModeOutput : PhotoModeOutput
	{
		private List<PhotoModeOutput> outputs = new List<PhotoModeOutput>();

		public RenderTextureFormat RenderTextureFormat
		{
			get => outputs.Count == 0 ? RenderTextureFormat.Default : outputs[0].RenderTextureFormat;
		}

		public void AddOutput(PhotoModeOutput output)
		{
			outputs.Add(output);
		}

		public void Output(RenderTexture texture)
		{
			foreach (var output in outputs)
			{
				output.Output(texture);
			}
		}
	}
}