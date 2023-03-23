using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Drawing
{
	public class FillPaint
	{
		public Color Color = Color.white;
	}

	public class StrokePaint
	{
		public float Weight = 0;
		public Color Color = Color.white;
	}

	public class DrawingCanvas 
	{
		internal VertexHelper vertexHelper;

		public ArcRenderer ArcRenderer = new ArcRenderer();
		public LineRenderer2D LineRenderer = new LineRenderer2D();

		public FillPaint FillPaint { get; set; } = new FillPaint();
		public StrokePaint StrokePaint { get; set; } = new StrokePaint();

		public DrawingCanvas(VertexHelper vertexHelper)
		{
			this.vertexHelper = vertexHelper;
		}

		public void DrawCircle(float x, float y, float resolution, float outerRadius, float innerRadius = 0)
		{
			DrawArc(x, y, resolution, outerRadius, innerRadius);
		}

		public void DrawArc(float x, float y, float resolution, float outerRadius, float innerRadius = 0, float startAngle = 0, float endAngle = Mathf.PI * 2)
		{
			ArcRendererHelper helper = new ArcRendererHelper(FillPaint, vertexHelper);
			ArcRenderer.DrawArc(helper, x, y, resolution, outerRadius, innerRadius, startAngle, endAngle);
		}

		public void DrawLine(Vector2[] points, PathMode pathMode = PathMode.Open)
		{
			LineRendererHelper helper = new LineRendererHelper(vertexHelper);
			LineRendererOptions options = new LineRendererOptions()
			{
				FillPaint = FillPaint,
				StrokePaint = StrokePaint,
				PathMode = pathMode
			};
			LineRenderer.Render(helper, points, options);
		}

		public void DrawQuad(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
		{
			vertexHelper.AddUIVertexQuad(new UIVertex[4]
			{
				new UIVertex() { position = p0, color = FillPaint.Color },
				new UIVertex() { position = p2, color = FillPaint.Color },
				new UIVertex() { position = p3, color = FillPaint.Color },
				new UIVertex() { position = p1, color = FillPaint.Color },
			});
		}

		public void DrawTriangle(Vector2 p0, Vector2 p1, Vector2 p2)
		{
			int index = vertexHelper.currentVertCount;
			vertexHelper.AddVert(p0, FillPaint.Color, Vector4.zero);
			vertexHelper.AddVert(p1, FillPaint.Color, Vector4.zero);
			vertexHelper.AddVert(p2, FillPaint.Color, Vector4.zero);
			vertexHelper.AddTriangle(index, index + 1, index + 2);
		}
	}
}