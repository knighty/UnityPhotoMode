using Drawing;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoMode.UI.Overlays
{
	[AddComponentMenu("Photo Mode/Overlays/Rule Of Thirds")]
	public class RuleOfThirds : Graphic, CompositionOverlay
	{
		[SerializeField, Range(1, 10)] float lineWeight = 2;

		public string Name => "Rule Of Thirds";

		public bool Enabled { get => gameObject.activeSelf; set => gameObject.SetActive(value); }

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

			DrawingCanvas Canvas = new DrawingCanvas(vh);

			Canvas.FillPaint = new FillPaint() { Color = color };

			Canvas.DrawQuad(P(x1 - w, y0), P(x1 + w, y0), P(x1 - w, y3), P(x1 + w, y3));
			Canvas.DrawQuad(P(x2 - w, y0), P(x2 + w, y0), P(x2 - w, y3), P(x2 + w, y3));

			Canvas.DrawQuad(P(x0, y1 - w), P(x0, y1 + w), P(x1 - w, y1 - w), P(x1 - w, y1 + w));
			Canvas.DrawQuad(P(x0, y2 - w), P(x0, y2 + w), P(x1 - w, y2 - w), P(x1 - w, y2 + w));

			Canvas.DrawQuad(P(x1 + w, y1 - w), P(x1 + w, y1 + w), P(x2 - w, y1 - w), P(x2 - w, y1 + w));
			Canvas.DrawQuad(P(x1 + w, y2 - w), P(x1 + w, y2 + w), P(x2 - w, y2 - w), P(x2 - w, y2 + w));

			Canvas.DrawQuad(P(x2 + w, y1 - w), P(x2 + w, y1 + w), P(x3, y1 - w), P(x3, y1 + w));
			Canvas.DrawQuad(P(x2 + w, y2 - w), P(x2 + w, y2 + w), P(x3, y2 - w), P(x3, y2 + w));
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			SetVerticesDirty();
		}
#endif
	}
}