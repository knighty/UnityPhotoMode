using UnityEngine;
using UnityEngine.EventSystems;
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

		[SerializeField] ShaderFactory shaderFactory;

		[SerializeField] Clarity clarity;

		public PhotoModeSettings Settings { get => settings; set => settings = value; }

		public void Awake()
		{
			photoModeRenderer = new PhotoModeRenderer(shaderFactory);

			volume = settings.InitVolume();
			volume.transform.SetParent(transform);

			PostProcessingAccumulationCameraAccumulator postProcessingAccumulator = new PostProcessingAccumulationCameraAccumulator(Camera, settings, volume.profile.GetSetting<AccumulationSettings>());
			AccumulationCameraController.SetAccmulator(postProcessingAccumulator);
		}

		public void Start()
		{
			GetComponent<AccumulationCameraController>().Settings = settings;
		}

		Coroutine activeRender = null;

		public void Update()
		{
			if (Settings == null || Camera == null)
			{
				return;
			}

			Settings.ApplyToVolume(volume);
			Settings.ApplyToCamera(Camera);
			clarity.ClarityValue = Settings.Clarity;
			clarity.Vibrance = Settings.Vibrance;

			/*if (Input.GetKeyDown(KeyCode.I))
			{
				photoModeRenderer.Render(Camera, Settings, output);
			}*/

			if (EventSystem.current.currentSelectedGameObject == null)
			{
				if (Input.GetKeyDown(KeyCode.O))
				{
					if (activeRender == null)
					{
						activeRender = StartCoroutine(photoModeRenderer.RenderRealtime(Camera, Settings, output, (result) =>
						{
							Debug.Log($"--- Render completed ---");
							Debug.Log($"Time Taken: {result.Duration}s");
							Debug.Log($"Frames Rendered: {result.Frames}s");
							activeRender = null;
						}));
					}
				}

				if (Input.GetKeyDown(KeyCode.T))
				{
					Time.timeScale = Time.timeScale == 0 ? 1 : 0;
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