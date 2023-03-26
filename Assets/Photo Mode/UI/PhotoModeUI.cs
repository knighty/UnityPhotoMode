using PhotoMode.UI.Overlays;
using UnityEngine;

namespace PhotoMode.UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class PhotoModeUI : MonoBehaviour
	{
		[SerializeField] private RectTransform window;
		[SerializeField] private RectTransform overlayOptions;

		CanvasGroup canvasGroup;
		CanvasGroup CanvasGroup => canvasGroup ??= GetComponent<CanvasGroup>();
		float alpha = 1;

		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.F1))
			{
				StartCoroutine(Utils.TweenTime(alpha, alpha == 0 ? 1 : 0, 0.4f, a =>
				{
					alpha = a;
					CanvasGroup.alpha = alpha;
					window.pivot = new Vector2(0.3f - 0.3f * alpha, window.pivot.y);
					overlayOptions.pivot = new Vector2(overlayOptions.pivot.x, 1 - 1 * alpha);
				}, Utils.EaseOutBack));
			}
		}
	}
}