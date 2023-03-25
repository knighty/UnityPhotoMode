using UnityEngine;

namespace PhotoMode
{
	[RequireComponent(typeof(AccumulationCameraController))]
	public class AccumulationCameraRenderer : MonoBehaviour
	{
		private AccumulationCameraController cameraController;
		private RenderTexture processingRenderTexture;
		private float accumulated = 0;
		private Material processingMaterial;
		private int processingMaterialMagnitude = 0;
		private Material outputMaterial;
		private int outputMaterialTotalAccumulations = 0;

		public int frameOffset = -1;
		[SerializeField] ShaderFactory shaderFactory;

		AccumulationCameraController CameraController => cameraController ??= GetComponent<AccumulationCameraController>();
		AccumulationCameraAccumulator Accumulator => CameraController.accumulator;

		public ShaderFactory ShaderFactory
		{
			get => shaderFactory;
			set {
				shaderFactory = value;
				InitProcesingMaterial();
				InitOutputMaterial();
			}
		}

		private void Awake()
		{
			if (shaderFactory != null)
			{
				InitProcesingMaterial();
				InitOutputMaterial();
			}
		}

		private void InitProcesingMaterial()
		{
			processingMaterial = new Material(shaderFactory.ProcessingShader);
			processingMaterial.mainTextureOffset = Vector2.zero;
			processingMaterial.mainTextureScale = Vector2.one;
			processingMaterialMagnitude = Shader.PropertyToID("_Magnitude");
		}

		private void InitOutputMaterial()
		{
			outputMaterial = new Material(shaderFactory.OutputShader);
			outputMaterial.mainTextureOffset = Vector2.zero;
			outputMaterial.mainTextureScale = Vector2.one;
			outputMaterialTotalAccumulations = Shader.PropertyToID("_TotalAccumulations");
		}

		private void ResetAccumulation()
		{
			RenderTexture rt = RenderTexture.active;
			RenderTexture.active = processingRenderTexture;
			GL.Clear(true, true, Color.clear);
			RenderTexture.active = rt;
			accumulated = 0;
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (processingMaterial == null || outputMaterial == null)
			{
				Debug.Log("Accumulation renderer does not have valid materials");
			}

			if (processingRenderTexture == null || processingRenderTexture.width != source.width || processingRenderTexture.height != source.height)
			{
				if (processingRenderTexture != null)
				{
					processingRenderTexture.Release();
				}
				processingRenderTexture = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.ARGBHalf);
			}

			if (Accumulator.Accumulation == 0)
			{
				ResetAccumulation();
			}

			if (Accumulator.Accumulation < Accumulator.Total && Accumulator.State != null)
			{
				Accumulate(source, Accumulator.State);
			}
			RenderToScreen(destination);
		}

		private void Accumulate(RenderTexture source, AccumulationCameraState cameraState)
		{
			if (cameraState.apertureShape == null)
			{
				Debug.Log("No aperture shape to accumulate");
			}
			Vector3 aperture = cameraState.apertureShape.GetRandomPoint(cameraState.accumulation + frameOffset);
			float magnitude = aperture.z;
			processingMaterial.SetFloat(processingMaterialMagnitude, magnitude);
			accumulated += magnitude;
			Graphics.Blit(source, processingRenderTexture, processingMaterial, 1);
		}

		private void RenderToScreen(RenderTexture destination)
		{
			outputMaterial.SetFloat(outputMaterialTotalAccumulations, accumulated);
			Graphics.Blit(processingRenderTexture, destination, outputMaterial, 1);
		}
	}
}
