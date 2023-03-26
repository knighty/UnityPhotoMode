using Drawing;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoMode.UI.Overlays
{
	[AddComponentMenu("Photo Mode/Overlays/Crop")]
	public class CropOverlay : Graphic
	{
		[SerializeField] private RectTransform compositionOverlay;

		private PhotoModeSettings settings;
		public PhotoModeSettings Settings
		{
			set
			{
				value.Width.OnChange += UpdateCrop;
				value.Height.OnChange += UpdateCrop;
				UpdateCrop(null);
				settings = value;
			}
		}

		private void UpdateCrop(PhotoModeSetting obj)
		{
			SetVerticesDirty();

			if (settings == null || !settings.Width.IsOverriding || !settings.Height.IsOverriding)
			{
				compositionOverlay.anchorMin = new Vector2(0, 0);
				compositionOverlay.anchorMax = new Vector2(1, 1);
				return;
			}

			Rect rect = GetPixelAdjustedRect();
			float aspectRatio = settings.Width.Value / settings.Height.Value;
			float rectAspectRatio = rect.width / rect.height;
			float normalizedW = settings.Width.Value / rect.width;
			float normalizedH = settings.Height.Value / rect.height;
			if (normalizedW > normalizedH)
			{
				float a = (1.0f / aspectRatio) * rectAspectRatio * 0.5f;
				compositionOverlay.anchorMin = new Vector2(0, 0.5f - a);
				compositionOverlay.anchorMax = new Vector2(1, 0.5f + a);
			}
			else
			{
				float a = (aspectRatio) * (1.0f / rectAspectRatio) * 0.5f;
				compositionOverlay.anchorMin = new Vector2(0.5f - a, 0);
				compositionOverlay.anchorMax = new Vector2(0.5f + a, 1);
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			if (settings == null || !settings.Width.IsOverriding || !settings.Height.IsOverriding)
			{
				return;
			}

			Rect rect = GetPixelAdjustedRect();
			float aspectRatio = settings.Width.Value / settings.Height.Value;
			float normalizedW = settings.Width.Value / rect.width;
			float normalizedH = settings.Height.Value / rect.height;

			DrawingCanvas Canvas = new DrawingCanvas(vh);
			Canvas.FillPaint = new FillPaint() { Color = color };

			float hrw = rect.width / 2;
			float hrh = rect.height / 2;

			if (normalizedW > normalizedH)
			{
				float w = rect.width;
				float h = w / aspectRatio;

				Canvas.DrawQuad(new Vector2(-hrw, hrh), new Vector2(hrw, hrh), new Vector2(-hrw, h / 2), new Vector2(hrw, h / 2));
				Canvas.DrawQuad(new Vector2(-hrw, -hrh), new Vector2(hrw, -hrh), new Vector2(-hrw, -h / 2), new Vector2(hrw, -h / 2));
			}
			else
			{
				float h = rect.height;
				float w = h * aspectRatio;

				Canvas.DrawQuad(new Vector2(-hrw, hrh), new Vector2(-w / 2, hrh), new Vector2(-hrw, -hrh), new Vector2(-w / 2, -hrh));
				Canvas.DrawQuad(new Vector2(w / 2, hrh), new Vector2(hrw, hrh), new Vector2(w / 2, -hrh), new Vector2(hrw, -hrh));
			}
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			SetVerticesDirty();
		}
#endif
	}
}