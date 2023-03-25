using Drawing;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoMode.UI.Overlays
{
	[AddComponentMenu("Photo Mode/Overlays/Crop")]
	public class CropOverlay : Graphic
	{
		private PhotoModeSettings settings;
		public PhotoModeSettings Settings
		{
			set
			{
				value.Width.OnChange += UpdateCrop;
				value.Height.OnChange += UpdateCrop;
				settings = value;
			}
		}

		private void UpdateCrop(PhotoModeSetting obj)
		{
			SetVerticesDirty();
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