using UnityEngine;

namespace Drawing
{
	public enum LineCap
	{
		Square,
		Rounded
	}

	public enum LineJoinStyle
	{
		Miter,
		Bevel,
		Rounded
	}

	public enum PathMode
	{
		Open, 
		Closed
	}

	public class LineRendererOptions
	{
		public LineCap LineCap = LineCap.Rounded;
		public FillPaint FillPaint;
		public StrokePaint StrokePaint;
		public PathMode PathMode = PathMode.Open;
	}

	public class LineRenderer2D
	{
		protected void RenderEndpoint(LineRendererHelper helper, Vector2 p0, Vector2 t0, bool end, LineRendererOptions options)
		{
			t0 = end ? new Vector2(-t0.y, t0.x) : new Vector2(t0.y, -t0.x);
			t0 *= options.StrokePaint.Weight;
			for (int i = 0; i < 32; i++)
			{
				float r1 = i / 32.0f * Mathf.PI;
				float r2 = (i + 1) / 32.0f * Mathf.PI;
				Vector2 p1 = p0 + new Vector2(t0.x * Mathf.Cos(r1) + t0.y * Mathf.Sin(r1), -t0.x * Mathf.Sin(r1) + t0.y * Mathf.Cos(r1));
				Vector2 p2 = p0 + new Vector2(t0.x * Mathf.Cos(r2) + t0.y * Mathf.Sin(r2), -t0.x * Mathf.Sin(r2) + t0.y * Mathf.Cos(r2));
				helper.AddTriangle(p0, p1, p2, Color.white);
			}
		}

		public void Render(LineRendererHelper helper, Vector2[] points, LineRendererOptions options)
		{
			if (points.Length > 1 && options.LineCap == LineCap.Rounded && options.PathMode == PathMode.Open)
			{
				Vector2 t0 = points[1] - points[0];
				t0.Normalize();
				RenderEndpoint(helper, points[0], t0, false, options);
			}

			int last = points.Length - 1 + (options.PathMode == PathMode.Closed ? 1 : 0);
			for (int i = 0; i <= last; i++)
			{
				bool firstPoint = i == 0;
				bool lastPoint = i == last;

				Vector2 p0 = points[i % points.Length];

				Vector2 n = options.PathMode == PathMode.Closed ? points[(i + 1) % points.Length] : (lastPoint ? (p0 + (p0 - points[i - 1])) : points[i + 1]);
				Vector2 p = options.PathMode == PathMode.Closed ? points[firstPoint ? (points.Length - 1) : (i - 1)] : (firstPoint ? (p0 + (p0 - points[i + 1])) : points[i - 1]);
				Vector2 d0 = p - p0;
				d0.Normalize();
				Vector2 d1 = n - p0;
				d1.Normalize();
				Vector2 halfway = (d0 + d1) / 2;
				float sinAngle = 1;

				halfway.Normalize();
				sinAngle = Mathf.Abs(halfway.x * d0.y - halfway.y * d0.x);
				if (sinAngle == 0)
				{
					halfway.x = -d0.y;
					halfway.y = d0.x;
					sinAngle = 1;
				}

				Vector2 offset = halfway * options.StrokePaint.Weight / sinAngle;
				float directionSign = Vector2.Dot(d0, new Vector2(-d1.y, d1.x)) > 0 ? -1 : 1;

				int index0 = helper.AddVert(p0 + offset * directionSign, options.StrokePaint.Color);
				int index1 = helper.AddVert(p0 - offset * directionSign, options.StrokePaint.Color);
				if (!lastPoint)
				{
					helper.AddTriangle(index0, index0 + 2, index0 + 1);
					helper.AddTriangle(index0 + 1, index0 + 2, index0 + 3);
				}
			}

			if (points.Length > 1 && options.PathMode == PathMode.Open)
			{
				Vector2 t0 = points[points.Length - 1] - points[points.Length - 2];
				t0.Normalize();
				RenderEndpoint(helper, points[points.Length - 1], t0, true, options);
			}
		}
	}
}