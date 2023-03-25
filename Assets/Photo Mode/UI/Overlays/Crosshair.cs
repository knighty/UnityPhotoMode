using Drawing;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoMode.UI.Overlays
{
	[AddComponentMenu("Photo Mode/Overlays/Crosshair")]
	public class Crosshair : Graphic
	{
		[SerializeField, Range(1, 10)] float lineWeight = 2;
		[SerializeField, Range(0, 100)] float crosshairSize = 5;
		[SerializeField, Range(0, 100)] float circleSize = 20;
		[SerializeField, Range(0, 10)] float circleWeight = 2;

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			Rect rect = GetPixelAdjustedRect();

			float x0 = rect.xMin;
			float x1 = rect.xMax / 3;
			float x2 = rect.xMax / 3 * 2;
			float x3 = rect.xMax;
			float y0 = rect.yMin;
			float y1 = rect.yMax / 3;
			float y2 = rect.yMax / 3 * 2;
			float y3 = rect.yMax;

			Vector2 P(float x, float y) => new Vector2(x, y);
			Vector2 c = rect.center;
			float w = lineWeight / 2; // Line Weight
			float cw = crosshairSize; // Crosshair Width

			DrawingCanvas Canvas = new DrawingCanvas(vh);

			Canvas.FillPaint = new FillPaint() { Color = color };

			if (circleSize > 0)
			{
				Canvas.DrawCircle(0, 0, 64, circleSize, Mathf.Max(circleSize - circleWeight, 0));
			}
			Canvas.DrawQuad(P(c.x - w, c.y + cw), P(c.x + w, c.y + cw), P(c.x - w, c.y - cw), P(c.x + w, c.y - cw));
			Canvas.DrawQuad(P(c.x - cw, c.y + w), P(c.x - w, c.y + w), P(c.x - cw, c.y - w), P(c.x - w, c.y - w));
			Canvas.DrawQuad(P(c.x + w, c.y + w), P(c.x + cw, c.y + w), P(c.x + w, c.y - w), P(c.x + cw, c.y - w));
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			SetVerticesDirty();
		}
#endif
	}
}