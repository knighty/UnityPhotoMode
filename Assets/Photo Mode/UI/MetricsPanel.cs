using UnityEngine;
using UnityEngine.UI;

namespace PhotoMode.UI
{
    public class MetricsPanel : MonoBehaviour
    {
        [SerializeField] Button toggleButton;
        [SerializeField] RectTransform panel;
		[SerializeField] RectTransform caret;

		float slideState = 1;

		private void Start()
		{
			toggleButton.onClick.AddListener(() =>
			{
				StartCoroutine(Utils.TweenTime(slideState, slideState == 0 ? 1 : 0, 0.4f, value =>
				{
					slideState = value;
					panel.pivot = new Vector2(value, panel.pivot.y);
					panel.anchoredPosition = new Vector2(-10 * value, panel.anchoredPosition.y);
					caret.rotation = Quaternion.Euler(0, 0, 90 - 180 * slideState);
				}, Utils.EaseOutBack));
			});
		}
	}
}