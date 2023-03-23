using Drawing;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoMode.UI.Overlays
{
	[RequireComponent(typeof(CanvasRenderer))]
	public class LensShiftRuler : Graphic
	{
		[SerializeField, Range(1, 10)] float lineWeight = 2;
		[SerializeField, Range(5, 20)] int spacing = 10;
		[SerializeField] LensShiftTriangle triangle;

		public PhotoModeSetting<Vector2> LensShiftSetting
		{
			set
			{
				triangle.LensShiftSetting = value;
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			Rect rect = GetPixelAdjustedRect();
			DrawingCanvas Canvas = new DrawingCanvas(vh);
			Vector2 P(float x, float y) => new Vector2(x, y);

			Canvas.FillPaint = new FillPaint() { Color = color };

			/*Canvas.DrawQuad(C(-size / 2, offset), C(size / 2, offset), C(-size / 2, offset - lineWeight), C(size / 2, offset - lineWeight));
			Canvas.DrawQuad(C(-size / 2 - lineWeight, offset + markerSize), C(-size / 2, offset + markerSize), C(-size / 2 - lineWeight, offset - lineWeight), C(-size / 2, offset - lineWeight));
			Canvas.DrawQuad(C(size / 2, offset + markerSize), C(size / 2 + lineWeight, offset + markerSize), C(size / 2, offset - lineWeight), C(size / 2 + lineWeight, offset - lineWeight));
			Canvas.DrawTriangle(C(-triangleSize, offset - lineWeight - triangleSize), C(triangleSize, offset - lineWeight - triangleSize), C(0, offset - lineWeight));*/

			Canvas.DrawQuad(P(rect.xMin, rect.yMin + lineWeight), P(rect.xMax, rect.yMin + lineWeight), P(rect.xMin, rect.yMin), P(rect.xMax, rect.yMin));
			Canvas.DrawQuad(P(rect.xMin, rect.yMax), P(rect.xMin + lineWeight, rect.yMax), P(rect.xMin, rect.yMin + lineWeight), P(rect.xMin + lineWeight, rect.yMin + lineWeight));
			Canvas.DrawQuad(P(rect.xMax - lineWeight, rect.yMax), P(rect.xMax, rect.yMax), P(rect.xMax - lineWeight, rect.yMin + lineWeight), P(rect.xMax, rect.yMin + lineWeight));

			float s = rect.width / spacing;
			for (float p = s; p < rect.width; p += s)
			{
				Canvas.DrawQuad(P(rect.xMin + p, rect.center.y), P(rect.xMin + lineWeight + p, rect.center.y), P(rect.xMin + p, rect.yMin + lineWeight), P(rect.xMin + lineWeight + p, rect.yMin + lineWeight));
			}
		}

		protected override void OnValidate()
		{
			SetVerticesDirty();
		}
	}
}