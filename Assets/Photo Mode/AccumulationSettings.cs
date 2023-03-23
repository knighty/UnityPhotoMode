using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(AccumulationRenderer), PostProcessEvent.AfterStack, "Custom/Accumulation")]
public sealed class AccumulationSettings : PostProcessEffectSettings
{
	public static AccumulationCameraAccumulator accumulator = null;
}

public sealed class AccumulationRenderer : PostProcessEffectRenderer<AccumulationSettings>
{
	private RenderTexture processingRenderTexture;
	private float accumulated = 0;

	private void Accumulate(PostProcessRenderContext context, AccumulationCameraState cameraState)
	{
		// Some care should be made here for floating point precision with large sample counts
		// 0.00000001 is how much precision we have approximately between 0 and 1
		// This means an 8bit buffer (256 values) can be sampled about 400,000 times I think
		// So that's ample for any normal situation but it should be made note of
		// For a half float it's 0.001 which means we get about 1000 values between 0 and 1
		// That's nowhere near enough so would need some careful multiplication per sample
		Vector3 aperture = cameraState.apertureShape.GetRandomPoint(cameraState.accumulation + 1);
		float magnitude = aperture.z;
		if (magnitude == 0)
			return;
		float thisAcc = magnitude;// (1.0f / AccumulationSettings.accumulator.Total) * magnitude;
		accumulated += thisAcc;
		var sheet = context.propertySheets.Get(Shader.Find("Shaders/AccumulateFrame"));
		sheet.properties.SetFloat("_Magnitude", thisAcc);
		context.command.BlitFullscreenTriangle(context.source, processingRenderTexture, sheet, 0);
	}

	private void RenderToScreen(PostProcessRenderContext context)
	{
		var sheet = context.propertySheets.Get(Shader.Find("Hidden/RenderAccumulatedFrame"));
		sheet.properties.SetFloat("_TotalAccumulations", accumulated);
		context.command.BlitFullscreenTriangle(processingRenderTexture, context.destination, sheet, 0);
	}

	public void ResetAccumulation()
	{
		RenderTexture rt = RenderTexture.active;
		RenderTexture.active = processingRenderTexture;
		GL.Clear(true, true, Color.clear);
		RenderTexture.active = rt;
		accumulated = 0;
	}

	public override void Release()
	{
		base.Release();
		processingRenderTexture?.Release();
		Debug.Log("Release Accumulation Renderer");
	}

	public override void Render(PostProcessRenderContext context)
	{
		if (!Application.isPlaying)
		{
			return;
		}

		if (AccumulationSettings.accumulator.Accumulation == 0)
		{
			ResetAccumulation();
		}

		if (processingRenderTexture == null)
		{
			processingRenderTexture = new RenderTexture(context.width, context.height, 0, RenderTextureFormat.ARGBHalf);
		}

		/*if (!AccumulationSettings.cameraController.cameraState.Equals(previousState))
		{
			RenderTexture rt = RenderTexture.active;
			RenderTexture.active = processingRenderTexture;
			GL.Clear(true, true, Color.clear);
			RenderTexture.active = rt;
			i = 0;
			accumulated = 0;
			AccumulationSettings.cameraController.ResetCamera();
			previousState = AccumulationSettings.cameraController.cameraState;
		}*/

		if (AccumulationSettings.accumulator.Accumulation < AccumulationSettings.accumulator.Total && AccumulationSettings.accumulator.State != null)
		{
			Accumulate(context, AccumulationSettings.accumulator.State);
		}
		RenderToScreen(context);
	}
}