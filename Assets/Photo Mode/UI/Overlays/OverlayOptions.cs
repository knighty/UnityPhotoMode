using UnityEngine;
using UnityEngine.UI;

namespace PhotoMode.UI.Overlays
{
	[AddComponentMenu("Photo Mode/Overlays/Options")]
	public class OverlayOptions : MonoBehaviour
	{
		[SerializeField] RuleOfThirds compositionOverlay;
		[SerializeField] ViewFinder viewFinder;

		[SerializeField] Dropdown guidelineDropdown;
		[SerializeField] Toggle viewfinderToggle;

		private void Awake()
		{
			viewfinderToggle.onValueChanged.AddListener(value => viewFinder.gameObject.SetActive(value));
			guidelineDropdown.onValueChanged.AddListener(value => compositionOverlay.gameObject.SetActive(value == 1));
		}
	}
}