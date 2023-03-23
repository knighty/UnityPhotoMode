using UnityEngine;

namespace PhotoMode.UI.Overlays
{
	[AddComponentMenu("Photo Mode/Overlays/View Finder")]
	public class ViewFinder : MonoBehaviour
	{
		[SerializeField] LensShiftRuler horizontalLensShiftRuler;
		[SerializeField] LensShiftRuler verticalLensShiftRuler;
		[SerializeField] RollOverlay roll;

		public PhotoModeSettings Settings
		{
			set
			{
				horizontalLensShiftRuler.LensShiftSetting = value.LensShift;
				verticalLensShiftRuler.LensShiftSetting = value.LensShift;
			}
		}

		public Camera Camera
		{
			set
			{
				roll.Camera = value;
			}
		}
	}
}