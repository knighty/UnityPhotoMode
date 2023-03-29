using Drawing;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoMode.UI.Overlays
{
	public class Cross : Graphic, CompositionOverlay
	{
		[SerializeField, Range(1, 10)] float lineWeight = 2;

		public string Name => "Cross";

		public bool Enabled { get => gameObject.activeSelf; set => gameObject.SetActive(value); }

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			Rect rect = GetPixelAdjustedRect();

			Vector2 P(float x, float y) => new Vector2(x, y);
			Vector2 c = rect.center;
			float w = lineWeight / 2; // Line Weight

			DrawingCanvas Canvas = new DrawingCanvas(vh);

			Canvas.FillPaint = new FillPaint() { Color = color };
			Canvas.StrokePaint = new StrokePaint() { Color = color, Weight = lineWeight };

			Canvas.DrawLine(new Vector2[]
			{
				new Vector2(rect.xMin, rect.yMin),
				new Vector2(rect.xMax, rect.yMax),
			});

			Canvas.DrawLine(new Vector2[]
			{
				new Vector2(rect.xMin, rect.yMax),
				new Vector2(rect.xMax, rect.yMin),
			});
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			SetVerticesDirty();
		}
#endif
	}
}
