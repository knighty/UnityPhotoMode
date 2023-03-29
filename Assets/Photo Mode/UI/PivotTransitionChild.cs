using UnityEngine;

namespace PhotoMode.UI
{
	[RequireComponent(typeof(RectTransform))]
    public class PivotTransitionChild : MonoBehaviour, TransitionChild
    {
		[SerializeField] private Vector2 pivotStart;
		[SerializeField] private Vector2 pivotEnd;
		[SerializeField] private bool pivotX = false;
		[SerializeField] private bool pivotY = false;

		public string State => "visible";
		RectTransform rectTransform;

		private void Awake()
		{
			rectTransform = GetComponent<RectTransform>();
		}

		public void UpdateState(float value)
		{
			Vector2 pivot = Vector2.LerpUnclamped(pivotStart, pivotEnd, value);
			rectTransform.pivot = new Vector2(pivotX ? pivot.x : rectTransform.pivot.x, pivotY ? pivot.y : rectTransform.pivot.y);
		}
    }
}
