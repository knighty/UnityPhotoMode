using PhotoMode.UI.Overlays;
using System.Collections.Generic;
using UnityEngine;

namespace PhotoMode.UI
{
	public class PhotoModeUI : MonoBehaviour
	{
		[SerializeField] private CanvasGroup panelGroup;
		[SerializeField] private RectTransform window;
		[SerializeField] private RectTransform overlayOptions;
		[SerializeField] private Overlay overlay;
		[SerializeField] private PhotoModeSettingsEditor settingEditor;
		[SerializeField] private MetricsPanel metrics;
		[SerializeField] private FocusOverlay focusOverlay;

		[SerializeField] private Camera camera;
		[SerializeField] private PhotoModeSettings settings;

		bool visible = true;
		float alpha = 1;
		Coroutine coroutine;

		public bool Visible
		{
			get => visible;
			set
			{
				visible = value;
				if (coroutine != null)
					StopCoroutine(coroutine);
				TransitionChild[] transitionChildren = GetComponentsInChildren<TransitionChild>();
				coroutine = StartCoroutine(Utils.TweenTime(alpha, visible ? 1 : 0, 0.4f, a =>
				{
					alpha = a;
					foreach (TransitionChild transitionChild in transitionChildren)
					{
						transitionChild.UpdateState(alpha);
					}
					/*panelGroup.alpha = alpha;
					panelGroup.interactable = alpha > 0.9f;
					panelGroup.blocksRaycasts = alpha > 0.9f;
					window.pivot = new Vector2(0.3f - 0.3f * alpha, window.pivot.y);
					overlayOptions.pivot = new Vector2(overlayOptions.pivot.x, 1 - 1 * alpha);*/
				}, Utils.EaseOutBack));
			}
		}

		public PhotoModeSettings Settings
		{
			set
			{
				focusOverlay.Settings = value;
				settingEditor.Settings = value;
				overlay.Settings = value;
			}
		}

		public Camera Camera
		{
			set
			{
				focusOverlay.Camera = value;
				overlay.Camera = value;
				metrics.Camera = value.GetComponent<AccumulationCameraController>();
				metrics.Histogram = value.GetComponent<Histogram>();
			}
		}

		private void Start()
		{
			if (settings != null)
			{
				Settings = settings;
			}
			if (camera != null)
			{
				Camera = camera;
			}
		}

		public void ToggleVisible()
		{
			Visible = !Visible;
		}
	}
}