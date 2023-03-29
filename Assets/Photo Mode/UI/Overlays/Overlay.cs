using UnityEngine;
using UnityEngine.EventSystems;

namespace PhotoMode.UI.Overlays
{
	[AddComponentMenu("Photo Mode/Overlays/Overlay")]
	public class Overlay : MonoBehaviour
	{
		[SerializeField] PhotoModeSettings settings;
		[SerializeField] new Camera camera;
		[SerializeField] ViewFinder viewFinder;
		[SerializeField] CropOverlay crop;

		public PhotoModeSettings Settings
		{
			set
			{
				settings = value;
				if (viewFinder != null)
				{
					viewFinder.Settings = settings;
				}
				if (crop != null)
				{
					crop.Settings = settings;
				}
			}
		}
		public Camera Camera
		{
			set
			{
				camera = value;
				if (viewFinder != null)
				{
					viewFinder.Camera = camera;
				}
			}
		}

		void Start()
		{
			if (settings != null && camera != null)
			{
				viewFinder.Settings = settings;
				viewFinder.Camera = camera;
			}
			if (settings != null)
			{
				crop.Settings = settings;
			}
		}

		void Update()
		{
			if (camera != null)
			{
				camera.GetComponent<FlyCamera>().enabled = EventSystem.current.currentSelectedGameObject == null;
			}
		}
	}
}