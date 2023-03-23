using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace PhotoMode
{
	public class PhotoMode : MonoBehaviour
	{
		[SerializeField]
		PhotoModeSettings settings;

		PostProcessVolume volume = null;

		PhotoModeRenderer photoModeRenderer;
		FilePhotoModeOutput output = new FilePhotoModeOutput("screenshots/%date% %time%", SaveTextureToFileUtility.SaveTextureFileFormat.JPG, new DateTimeFilePhotoModeOutputPathTokenHandler());

		[SerializeReference]
		public Camera Camera;

		[SerializeReference]
		public AccumulationCameraController AccumulationCameraController;

		public PhotoModeSettings Settings { get => settings; }

		public void Awake()
		{
			photoModeRenderer = new PhotoModeRenderer(new DefaultPhotoModeRendererShaderFactory());

			/*settings.BloomIntensity = 0.5f;
			settings.BloomThreshold = 0.7f;
			settings.Saturation = 12;
			settings.Exposure = 0.3f;
			settings.ChromaticAberration = 0.1f;
			settings.ColorTemperature = -12;
			settings.Contrast = 12;*/

			volume = settings.InitVolume();
			volume.transform.SetParent(transform);

			PostProcessingAccumulationCameraAccumulator postProcessingAccumulator = new PostProcessingAccumulationCameraAccumulator(Camera, settings, volume.profile.GetSetting<AccumulationSettings>());
			AccumulationCameraController.SetAccmulator(postProcessingAccumulator);
		}

		Coroutine activeRender = null;

		public void Update()
		{
			settings.ApplyToVolume(volume);
			settings.ApplyToCamera(Camera);
			settings.Apply();

			if (Input.GetKeyDown(KeyCode.I))
			{
				photoModeRenderer.Render(Camera, settings, output);
			}

			if (Input.GetKeyDown(KeyCode.O))
			{
				if (activeRender == null)
				{
					activeRender = StartCoroutine(photoModeRenderer.RenderRealtime(Camera, settings, output, (result) =>
					{
						Debug.Log($"--- Render completed ---");
						Debug.Log($"Time Taken: {result.Duration}s");
						Debug.Log($"Frames Rendered: {result.Frames}s");
						activeRender = null;
					}));
				}
			}

			if (Input.GetKeyDown(KeyCode.U))
			{
				if (Physics.Raycast(new Ray(transform.position, transform.forward), out RaycastHit hitInfo))
				{
					settings.FocusDistance.Value = hitInfo.distance;
				}
			}
		}

		public void OnDestroy()
		{
			photoModeRenderer?.Release();
			photoModeRenderer = null;
		}
	}
}