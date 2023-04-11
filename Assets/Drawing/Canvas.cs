using UnityEngine;
using UnityEngine.UI;

namespace Drawing
{
	public struct FillPaint
	{
		public Color Color;

		public FillPaint(Color color)
		{
			Color = color;
		}
	}

	public struct StrokePaint
	{
		public float Weight;
		public Color Color;

		public StrokePaint(float weight, Color color)
		{
			Weight = weight;
			Color = color;
		}
	}

	public class DrawingCanvas 
	{
		private VertexHelper vertexHelper;

		private ArcRenderer ArcRenderer = new ArcRenderer();
		private LineRenderer2D LineRenderer = new LineRenderer2D();

		public FillPaint FillPaint { get; set; } = new FillPaint();
		public StrokePaint StrokePaint { get; set; } = new StrokePaint();
		public VertexHelper VertexHelper { set => vertexHelper = value; }

		public DrawingCanvas(VertexHelper vertexHelper = null)
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
			LineRendererOptions options = new LineRendererOptions(FillPaint, StrokePaint);
			options.PathMode = pathMode;
			LineRenderer.Render(helper, options, points);
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