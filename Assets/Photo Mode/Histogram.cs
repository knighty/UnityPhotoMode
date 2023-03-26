using UnityEngine;
using System.Linq;
using System;

namespace PhotoMode
{
	public class Histogram : MonoBehaviour
	{
		[SerializeField] ComputeShader shader;

		int handleMain;
		int handleInitialize;
		int handleMax;
		int handleMaxInitialize;
		ComputeBuffer histogramBuffer;
		ComputeBuffer histogramMaxBuffer;		

		public ComputeBuffer HistogramBuffer { get => histogramBuffer; }
		public ComputeBuffer HistogramMaxBuffer { get => histogramMaxBuffer; }
		
		void Start()
		{
			if (null == shader)
			{
				Debug.Log("Shader missing.");
				return;
			}

			handleInitialize = shader.FindKernel("HistogramInitialize");
			handleMain = shader.FindKernel("HistogramMain");
			handleMax = shader.FindKernel("HistogramMax");
			handleMaxInitialize = shader.FindKernel("HistogramMaxInitialize");

			histogramBuffer = new ComputeBuffer(256, sizeof(uint) * 4);
			histogramMaxBuffer = new ComputeBuffer(1, sizeof(uint));

			if (handleInitialize < 0 || handleMain < 0 ||
			   null == histogramBuffer)
			{
				Debug.Log("Initialization failed.");
				return;
			}

			shader.SetBuffer(handleMain, "HistogramBuffer", histogramBuffer);

			shader.SetBuffer(handleInitialize, "HistogramBuffer", histogramBuffer);

			shader.SetBuffer(handleMaxInitialize, "HistogramMaxBuffer", histogramMaxBuffer);
			
			shader.SetBuffer(handleMax, "HistogramBuffer", histogramBuffer);
			shader.SetBuffer(handleMax, "HistogramMaxBuffer", histogramMaxBuffer);
		}

		void OnDestroy()
		{
			if (null != histogramBuffer)
			{
				histogramBuffer.Release();
				histogramBuffer = null;
			}
			if (null != histogramMaxBuffer)
			{
				histogramMaxBuffer.Release();
				histogramMaxBuffer = null;
			}
		}

		void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (null == shader ||
			   0 > handleInitialize || 0 > handleMain ||
			   null == histogramBuffer)
			{
				Debug.Log("Cannot compute histogram");
				return;
			}

			shader.SetTexture(handleMain, "InputTexture", source);

			shader.Dispatch(handleInitialize, 256 / 64, 1, 1);
			shader.Dispatch(handleMain, (source.width + 7) / 8, (source.height + 7) / 8, 1);

			shader.Dispatch(handleMaxInitialize, 1, 1, 1);
			shader.Dispatch(handleMax, 256 / 64, 1, 1);

			//histogramBuffer.GetData(histogramData);

			Graphics.Blit(source, destination);
		}
	}
}