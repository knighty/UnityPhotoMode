using UnityEngine;

namespace Drawing
{
	public enum LineCap
	{
		Square,
		Rounded,
		None
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

	public struct LineRendererOptions
	{
		public LineCap LineCap;
		public FillPaint FillPaint;
		public StrokePaint StrokePaint;
		public PathMode PathMode;

		public LineRendererOptions(FillPaint fillPaint, StrokePaint strokePaint)
		{
			FillPaint = fillPaint;
			StrokePaint = strokePaint;
			LineCap = LineCap.Rounded;
			PathMode = PathMode.Open;
		}
	}

	public class LineRenderer2D
	{
		protected void RenderEndpoint(LineRendererHelper helper, Vector2 p0, Vector2 t0, bool end, LineRendererOptions options)
		{
			Vector2 offset = new Vector2(t0.y, -t0.x) * (options.StrokePaint.Weight + 2) * 0.5f;

			float r = 1;
			float d = Mathf.Ceil(options.StrokePaint.Weight / 2.0f + 2.5f * r * 0.5f);

			t0 *= options.StrokePaint.Weight * (end ? -1 : 1);
			/*for (int i = 0; i < 32; i++)
			{
				float r1 = i / 32.0f * Mathf.PI;
				float r2 = (i + 1) / 32.0f * Mathf.PI;
				Vector2 p1 = p0 + new Vector2(t0.x * Mathf.Cos(r1) + t0.y * Mathf.Sin(r1), -t0.x * Mathf.Sin(r1) + t0.y * Mathf.Cos(r1));
				Vector2 p2 = p0 + new Vector2(t0.x * Mathf.Cos(r2) + t0.y * Mathf.Sin(r2), -t0.x * Mathf.Sin(r2) + t0.y * Mathf.Cos(r2));
				//helper.AddTriangle(p0, p1, p2, Color.white);
			}*/

			int index = helper.AddVert(p0 + offset - t0, options.StrokePaint.Color, new Vector4(-options.StrokePaint.Weight, -d, options.StrokePaint.Weight / 2.0f, 0), 1);
			helper.AddVert(p0 - offset - t0, options.StrokePaint.Color, new Vector4(-options.StrokePaint.Weight, d, options.StrokePaint.Weight / 2.0f, 0), 1);

			helper.AddVert(p0 + offset, options.StrokePaint.Color, new Vector4(0, -d, options.StrokePaint.Weight / 2.0f, 0), 1);
			helper.AddVert(p0 - offset, options.StrokePaint.Color, new Vector4(0, d, options.StrokePaint.Weight / 2.0f, 0), 1);

			helper.AddTriangle(index + 0, index + 2, index + 1);
			helper.AddTriangle(index + 1, index + 2, index + 3);
		}

		public void Render(LineRendererHelper helper, LineRendererOptions options, Vector2[] points)
		{
			if (points.Length > 1 && options.LineCap == LineCap.Rounded && options.PathMode == PathMode.Open)
			{
				Vector2 t0 = points[1] - points[0];
				t0.Normalize();
				RenderEndpoint(helper, points[0], t0, false, options);
			}

			int last = points.Length - 1 + (options.PathMode == PathMode.Closed ? 1 : 0);

			bool isFirst(int i)
			{
				return i == 0;
			}

			bool isLast(int i)
			{
				return i == last;
			}

			Vector2 position(int i)
			{
				return points[i % points.Length];
			}

			Vector2 getPreviousPoint(int i)
			{
				Vector2 p0 = points[i % points.Length];
				bool firstPoint = i == 0;
				return options.PathMode == PathMode.Closed ? points[firstPoint ? (points.Length - 1) : (i - 1)] : (firstPoint ? (p0 + (p0 - points[i + 1])) : points[i - 1]);
			}

			Vector2 getNextPoint(int i)
			{
				Vector2 p0 = points[i % points.Length];
				bool lastPoint = i == last;
				return options.PathMode == PathMode.Closed ? points[(i + 1) % points.Length] : (lastPoint ? (p0 + (p0 - points[i - 1])) : points[i + 1]);
			}

			Vector2 getHalfway(Vector2 p0, Vector2 p, Vector2 n)
			{
				Vector2 d0 = p - p0;
				d0.Normalize();
				Vector2 d1 = n - p0;
				d1.Normalize();
				Vector2 halfway = (d0 + d1) / 2;
				halfway.Normalize();
				return halfway;
			}

			float getSinAngle(Vector2 p0, Vector2 p, Vector2 n)
			{
				Vector2 d0 = p - p0;
				d0.Normalize();
				Vector2 d1 = n - p0;
				d1.Normalize();
				Vector2 halfway = (d0 + d1) / 2;
				float sinAngle = 1;

				halfway.Normalize();
				return (halfway.x * d0.y - halfway.y * d0.x) * -1;
			}

			Vector2 getOffset(Vector2 point, Vector2 previous, Vector2 next)
			{
				Vector2 d0 = previous - point;
				d0.Normalize();
				Vector2 d1 = next - point;
				d1.Normalize();
				Vector2 halfway = (d0 + d1) / 2;
				float sinAngle = 1;

				halfway.Normalize();
				sinAngle = (halfway.x * d0.y - halfway.y * d0.x) * -1;
				if (sinAngle == 0)
				{
					halfway.x = -d0.y;
					halfway.y = d0.x;
					sinAngle = 1;
				}

				float directionSign = (halfway.x * d0.y - halfway.y * d0.x) > 0 ? -1 : 1;

				return halfway * (options.StrokePaint.Weight / 2 + 1) / sinAngle;// * directionSign;
			}

			void handleSegment(int i)
			{
				Vector2 p1 = position(i);
				Vector2 p2 = position(i + 1);
				Vector2 d1 = p2 - p1;
				d1.Normalize();
				float l = (p2 - p1).magnitude;

				float r = 1;
				float d = Mathf.Ceil(options.StrokePaint.Weight / 2.0f + 2.5f * r * 0.5f);

				Vector2 offset1 = getOffset(p1, getPreviousPoint(i), getNextPoint(i));
				int index = helper.AddVert(p1 + offset1, options.StrokePaint.Color, new Vector4(Vector2.Dot(offset1, d1), -d, options.StrokePaint.Weight / 2.0f, l), 1);
				helper.AddVert(p1 - offset1, options.StrokePaint.Color, new Vector4(-Vector2.Dot(offset1, d1), d, options.StrokePaint.Weight / 2.0f, l), 1);

				Vector2 offset2 = getOffset(p2, getPreviousPoint(i+1), getNextPoint(i+1));
				helper.AddVert(p2 + offset2, options.StrokePaint.Color, new Vector4(l + Vector2.Dot(offset2, d1), -d, options.StrokePaint.Weight / 2.0f, l), 1);
				helper.AddVert(p2 - offset2, options.StrokePaint.Color, new Vector4(l - Vector2.Dot(offset2, d1), d, options.StrokePaint.Weight / 2.0f, l), 1);

				helper.AddTriangle(index + 0, index + 1, index + 2);
				helper.AddTriangle(index + 1, index + 3, index + 2);
			}

			for (int i = 0; i < last; i++)
			{
				handleSegment(i);
			}

			if (points.Length > 1 && options.PathMode == PathMode.Open && options.LineCap == LineCap.Rounded)
			{
				Vector2 t0 = points[points.Length - 1] - points[points.Length - 2];
				t0.Normalize();
				RenderEndpoint(helper, points[points.Length - 1], t0, true, options);
			}
		}
	}
}