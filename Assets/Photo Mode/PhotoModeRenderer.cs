using System;
using System.Collections;
using UnityEngine;

namespace PhotoMode
{
	public interface PhotoModeRendererShaderFactory
	{
		Shader Processing { get; }
		Shader Output { get; }
	}

	public class DefaultPhotoModeRendererShaderFactory : PhotoModeRendererShaderFactory
	{
		public Shader Processing => Shader.Find("Shaders/AccumulateFrame");
		public Shader Output => Shader.Find("Hidden/RenderAccumulatedFrame");
	}

	public class PhotoModeRenderer
	{
		RenderTexture processingRenderTexture;
		RenderTexture outputRenderTexture;
		Material processingMaterial;
		int processingMaterialMagnitude = 0;
		Material outputMaterial;
		int outputMaterialTotalAccumulations = 0;

		RenderTextureFormat processingRenderTextureFormat = RenderTextureFormat.ARGBFloat;

		PhotoModeRendererShaderFactory shaderFactory;

		public class Result
		{
			public float Duration;
			public int Frames;
		}

		public PhotoModeRenderer(PhotoModeRendererShaderFactory shaderFactory)
		{
			this.shaderFactory = shaderFactory;
			InitProcesingMaterial();
			InitOutputMaterial();
		}

		private void InitProcesingMaterial()
		{
			processingMaterial = new Material(shaderFactory.Processing);
			processingMaterial.mainTextureOffset = Vector2.zero;
			processingMaterial.mainTextureScale = Vector2.one;
			processingMaterialMagnitude = Shader.PropertyToID("_Magnitude");
		}

		private void InitOutputMaterial()
		{
			outputMaterial = new Material(shaderFactory.Output);
			outputMaterial.mainTextureOffset = Vector2.zero;
			outputMaterial.mainTextureScale = Vector2.one;
			outputMaterialTotalAccumulations = Shader.PropertyToID("_TotalAccumulations");
		}

		int GetAccumulations(PhotoModeQuality quality)
		{
			switch (quality)
			{
				case PhotoModeQuality.Low:
					return 0;
				case PhotoModeQuality.Medium:
					return 64;
				case PhotoModeQuality.High:
					return 128;
				case PhotoModeQuality.VeryHigh:
					return 256;
			}
			return 0;
		}

		public void Release()
		{
			processingRenderTexture?.Release();
			processingRenderTexture = null;
			outputRenderTexture?.Release();
			outputRenderTexture = null;
		}

		public IEnumerator RenderRealtime(Camera camera, PhotoModeSettings settings, PhotoModeOutput output, Action<Result> finished)
		{
			float timeStart = Time.realtimeSinceStartup;

			AccumulationCameraController accumulationCameraController = camera.GetComponent<AccumulationCameraController>();

			int width = settings.Width.IsOverriding ? (int)settings.Width : camera.pixelWidth;
			int height = settings.Height.IsOverriding ? (int)settings.Height : camera.pixelHeight;

			RenderTexture processingRenderTexture = RenderTexture.GetTemporary(width, height, 0, processingRenderTextureFormat);
			RenderTexture outputRenderTexture = RenderTexture.GetTemporary(width, height, 0, output.RenderTextureFormat);
			RenderTexture cameraRenderTexture = RenderTexture.GetTemporary(width, height, 24, processingRenderTextureFormat);

			RenderTexture oldRT = camera.targetTexture;
			int oldCullingMask = camera.cullingMask;
			camera.targetTexture = cameraRenderTexture;
			camera.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));

			float totalAccumulated = 0;
			int numAccumulations = settings.Accumulations;
			AccumulationCameraAccumulator oldAccumulator = accumulationCameraController.accumulator;
			RenderAccumulationCameraAccumulator accumulator = new RenderAccumulationCameraAccumulator(camera, settings);
			accumulationCameraController.SetAccmulator(accumulator);

			for (int i = 0; i < numAccumulations; i++)
			{
				accumulationCameraController.NextAccumulation();

				Vector3 aperturePoint = settings.ApertureShape.Value.GetRandomPoint(accumulationCameraController.accumulator.State.accumulation);
				float magnitude = 1.0f / numAccumulations * aperturePoint.z;
				processingMaterial.SetFloat(processingMaterialMagnitude, magnitude);
				totalAccumulated += magnitude;
				Graphics.Blit(cameraRenderTexture, processingRenderTexture, processingMaterial, 1);
				yield return null;
			}
			accumulationCameraController.SetAccmulator(oldAccumulator);

			outputMaterial.SetFloat(outputMaterialTotalAccumulations, totalAccumulated);
			Graphics.Blit(processingRenderTexture, outputRenderTexture, outputMaterial, 1);

			camera.targetTexture = oldRT;
			camera.cullingMask = oldCullingMask;

			output.Output(outputRenderTexture);

			RenderTexture.ReleaseTemporary(processingRenderTexture);
			RenderTexture.ReleaseTemporary(outputRenderTexture);
			RenderTexture.ReleaseTemporary(cameraRenderTexture);

			float timeEnd = Time.realtimeSinceStartup;

			finished(new Result() { Duration = timeEnd - timeStart, Frames = numAccumulations });
		}

		public void Render(Camera camera, PhotoModeSettings settings, PhotoModeOutput output)
		{
			AccumulationCameraController accumulationCameraController = camera.GetComponent<AccumulationCameraController>();

			int width = settings.Width.IsOverriding ? (int)settings.Width : camera.pixelWidth;
			int height = settings.Height.IsOverriding ? (int)settings.Height : camera.pixelHeight;

			RenderTexture processingRenderTexture = RenderTexture.GetTemporary(width, height, 0, processingRenderTextureFormat);
			RenderTexture outputRenderTexture = RenderTexture.GetTemporary(width, height, 0, output.RenderTextureFormat);
			RenderTexture cameraRenderTexture = RenderTexture.GetTemporary(width, height, 24, processingRenderTextureFormat);

			RenderTexture oldRT = camera.targetTexture;
			int oldCullingMask = camera.cullingMask;
			camera.targetTexture = cameraRenderTexture;
			camera.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));

			float totalAccumulated = 0;
			int numAccumulations = settings.Accumulations;
			AccumulationCameraAccumulator oldAccumulator = accumulationCameraController.accumulator;
			RenderAccumulationCameraAccumulator accumulator = new RenderAccumulationCameraAccumulator(camera, settings);
			accumulationCameraController.SetAccmulator(accumulator);
			for (int i = 0; i < numAccumulations; i++)
			{
				accumulationCameraController.NextAccumulation();
				camera.Render();

				Vector3 aperturePoint = settings.ApertureShape.Value.GetRandomPoint(accumulationCameraController.accumulator.State.accumulation);
				float magnitude = 1.0f / numAccumulations * aperturePoint.z;
				processingMaterial.SetFloat(processingMaterialMagnitude, magnitude);
				totalAccumulated += magnitude;
				Graphics.Blit(cameraRenderTexture, processingRenderTexture, processingMaterial, 1);
			}
			accumulationCameraController.SetAccmulator(oldAccumulator);

			outputMaterial.SetFloat(outputMaterialTotalAccumulations, totalAccumulated);
			Graphics.Blit(processingRenderTexture, outputRenderTexture, outputMaterial, 1);

			camera.targetTexture = oldRT;
			camera.cullingMask = oldCullingMask;

			output.Output(outputRenderTexture);

			RenderTexture.ReleaseTemporary(processingRenderTexture);
			RenderTexture.ReleaseTemporary(outputRenderTexture);
			RenderTexture.ReleaseTemporary(cameraRenderTexture);

			accumulationCameraController.ResetCamera();
		}
	}
}