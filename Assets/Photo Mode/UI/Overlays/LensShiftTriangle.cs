using Drawing;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoMode.UI.Overlays
{
	[AddComponentMenu("Photo Mode/Overlays/Lens Shift Triangle")]
	[RequireComponent(typeof(CanvasRenderer))]
	public class LensShiftTriangle : MonoBehaviour// : Graphic
	{
		public enum Direction
		{
			Horizontal,
			Vertical
		}

		[SerializeField] Direction direction;

		float Value
		{
			set
			{
				RectTransform rectTransform = GetComponent<RectTransform>();
				float offset = 0.5f + value / 0.15f * 0.5f;
				rectTransform.anchorMin = new Vector2(offset, 0);
				rectTransform.anchorMax = new Vector2(offset, 0);
			}
		}

		public PhotoModeSetting<Vector2> LensShiftSetting
		{
			set
			{
				value.OnChange += value =>
				{
					PhotoModeSetting<Vector2> lensShift = value as PhotoModeSetting<Vector2>;
					Value = direction == Direction.Horizontal ? lensShift.Value.x : -lensShift.Value.y;
				};
			}
		}

		/*protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			Rect rect = GetPixelAdjustedRect();
			DrawingCanvas Canvas = new DrawingCanvas(vh);
			Vector2 c = rect.center;
			Vector2 P(float x, float y) => new Vector2(x, y);

			Canvas.FillPaint = new FillPaint() { Color = color };
			Canvas.DrawTriangle(P(rect.xMin, rect.yMin), P(rect.xMax, rect.yMin), P(rect.center.x, rect.yMax));
		}

		protected override void OnValidate()
		{
			SetVerticesDirty();
		}*/
	}
}