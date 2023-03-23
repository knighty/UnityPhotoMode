using UnityEngine;
using UnityEngine.EventSystems;

namespace PhotoMode.UI.Overlays
{
	[AddComponentMenu("Photo Mode/Overlays/Overlay")]
	public class Overlay : MonoBehaviour, IPointerClickHandler
	{
		[SerializeField] PhotoModeSettings settings;
		[SerializeField] new Camera camera;
		[SerializeField] ViewFinder viewFinder;
		[SerializeField] CropOverlay crop;

		public void OnPointerClick(PointerEventData eventData)
		{
			Vector3 rayDirection = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 1)) - Camera.main.transform.position;
			rayDirection.Normalize();

			if (Physics.Raycast(new Ray(Camera.main.transform.position, rayDirection), out RaycastHit hitInfo))
			{
				float distance = Vector3.Dot(Camera.main.transform.forward, rayDirection * hitInfo.distance);
				settings.FocusDistance.Value = distance;
			}
		}

		void Start()
		{
			if (viewFinder != null)
			{
				viewFinder.Settings = settings;
				viewFinder.Camera = camera;
			}
			if (crop != null)
			{
				crop.Settings = settings;
			}
		}
	}
}