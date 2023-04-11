using Drawing;
using UnityEngine;
using UnityEngine.UI;

public class LineTest : Graphic
{
	[SerializeField, Range(2, 1000)] int segments = 500;
	[SerializeField, Range(0, 50)] float weight = 4;

	DrawingCanvas drawingCanvas = new DrawingCanvas();
	StrokePaint graphStrokePaint =  new StrokePaint();

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();

		DrawGraph(vh);
	}

	private void DrawGraph(VertexHelper vh)
	{
		drawingCanvas.VertexHelper = vh;

		Vector2[] points = new Vector2[segments];
		for (int i = 0; i < segments; i++)
		{
			float t = i / (float)segments;
			//points[i] = new Vector2(t * 1000.0f, 500.0f + Mathf.Sin(t * Mathf.PI * 6) * 100 + Random.Range(-100, 100));
			points[i] = new Vector2(50 + t * (Screen.width - 100), MathsUtilities.Perlin.Sample(t * 3.0f, 0, 8, 0.5f) * 500.0f + 500.0f);
			//points[i] = Random.insideUnitCircle * 500 + new Vector2(500, 500);
		}

		drawingCanvas.StrokePaint = graphStrokePaint;
		drawingCanvas.DrawLine(points);
	}

	protected override void OnValidate()
	{
		SetVerticesDirty();
		graphStrokePaint.Weight = weight;
		graphStrokePaint.Color = color;
	}
}
