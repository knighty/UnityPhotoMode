using System.Collections.Generic;
using UnityEngine;

namespace SVG
{
	public class SVGPath
	{
		public List<SVGPathSegment> Segments = new List<SVGPathSegment>();
	}

	public class SVGPathSegment
	{
		public bool Relative;
	}

	public class SVGBezierSegment : SVGPathSegment
	{
		public Vector2 Handle1;
		public Vector2 Handle2;
		public Vector2 End;
		public bool UsePreviousHandle = false;

		public SVGBezierSegment(Vector2 handle1, Vector2 handle2, Vector2 end)
		{
			Handle1 = handle1;
			Handle2 = handle2;
			End = end;
		}

		public SVGBezierSegment(float h1x, float h1y, float h2x, float h2y, float x, float y)
		{
			Handle1 = new Vector2(h1x, h1y);
			Handle2 = new Vector2(h2x, h2y);
			End = new Vector2(x, y);
		}

		public SVGBezierSegment(float h2x, float h2y, float x, float y)
		{
			Handle1 = Vector2.zero;
			Handle2 = new Vector2(h2x, h2y);
			End = new Vector2(x, y);
			UsePreviousHandle = true;
		}
	}

	public class SVGMoveToSegment : SVGPathSegment
	{
		public Vector2 Position;

		public SVGMoveToSegment(Vector2 position)
		{
			Position = position;
		}

		public SVGMoveToSegment(float x, float y)
		{
			Position = new Vector2(x, y);
		}
	}

	public class SVGLineSegment : SVGPathSegment
	{
		public Vector2 End;

		public SVGLineSegment(Vector2 end)
		{
			End = end;
		}

		public SVGLineSegment(float x, float y)
		{
			End = new Vector2(x, y);
		}
	}

	public class SVGHorizontalLineSegment : SVGPathSegment
	{
		public float X;

		public SVGHorizontalLineSegment(float end)
		{
			X = end;
		}
	}

	public class SVGVerticalLineSegment : SVGPathSegment
	{
		public float Y;

		public SVGVerticalLineSegment(float end)
		{
			Y = end;
		}
	}

	public class SVGEndPathSegment : SVGPathSegment
	{
	}
}
