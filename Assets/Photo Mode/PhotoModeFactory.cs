using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhotoMode
{
	[CreateAssetMenu(fileName = "Factory", menuName = "Photo Mode/Factory")]
	public class PhotoModeFactory : ScriptableObject
	{
		[SerializeField] ComputeShader histogramShader;
		[SerializeField] ShaderFactory shaderFactory;

		public void InstantiateCamera(Camera camera)
		{
			GameObject o = camera.gameObject;

			AccumulationCameraController accumulationCameraController = o.AddComponent<AccumulationCameraController>();

			AccumulationCameraRenderer accumulationCameraRenderer = o.AddComponent<AccumulationCameraRenderer>();
			accumulationCameraRenderer.ShaderFactory = shaderFactory;

			Histogram histogram = o.AddComponent<Histogram>(); 
			histogram.Shader = histogramShader;

			Clarity clarity = o.AddComponent<Clarity>();
			clarity.ShaderFactory = shaderFactory;
		}
	}
}
