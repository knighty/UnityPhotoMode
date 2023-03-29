using PhotoMode.UI.Overlays;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoMode.UI
{
	public class CompositionOverlayDropdown : MonoBehaviour
	{
		[SerializeField] private RectTransform compositionOverlaysRect;

		List<CompositionOverlay> overlays;
		Dropdown dropdown;

		private void Awake()
		{
			dropdown = GetComponent<Dropdown>();
		}

		void Start()
		{
			List<Dropdown.OptionData> none = new List<Dropdown.OptionData>() { new Dropdown.OptionData("None") };
			overlays = compositionOverlaysRect.GetComponentsInChildren<CompositionOverlay>().ToList();
			dropdown.AddOptions(
				none.Concat(
					overlays
					.Select(overlay => new Dropdown.OptionData(overlay.Name))
				).ToList()
			);

			dropdown.onValueChanged.AddListener(SelectOverlay);
			SelectOverlay(0);
			dropdown.value = 0;
		}

		void SelectOverlay(int overlayIndex)
		{
			int index = 1;
			foreach (var overlay in overlays) {
				overlay.Enabled = index == overlayIndex;
				index++;
			}
		}
	}
}
