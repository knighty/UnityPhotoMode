using System;
using UnityEngine;

namespace PhotoMode.UI
{
	[RequireComponent(typeof(CanvasGroup))]
	internal class PanelGroupTransitionChild : MonoBehaviour, TransitionChild
	{
		[SerializeField] float hideAlpha = 0;
		[SerializeField] float showAlpha = 0;

		private CanvasGroup canvasGroup;

		public string State => throw new NotImplementedException();

		void Awake()
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}

		public void UpdateState(float value)
		{
			canvasGroup.alpha = value;
			canvasGroup.interactable = value > 0.9f;
			canvasGroup.blocksRaycasts = value > 0.9f;
		}
	}
}
