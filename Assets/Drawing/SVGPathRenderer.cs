using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SVG
{
	[Serializable]
	public class SVGPathRendererOptions
	{
		public Vector2 Translate = Vector2.zero;
		public Vector2 Scale = Vector2.one;
		public bool Reverse = false;
		public bool FilterDuplicates = true;
		[Range(1, 200)] public float CurveResolution = 3.0f;
		[Range(0, 3)] public float CurveMinAngle = 0;
	}

	public class SVGPathRenderer
	{
		public List<List<Vector2>> GetPoints(SVGPath path, SVGPathRendererOptions options)
		{
			Vector2 pos = Vector2.zero;
			Vector2 posUntransformed = Vector2.zero;
			Vector2 previousCurveHandle = Vector2.zero;
			float curveResolution = options.CurveResolution;
			List<Vector2> vertices = new List<Vector2>();

			List<List<Vector2>> paths = new List<List<Vector2>>();

			Vector2 Transform(Vector2 point)
			{
				return (point + options.Translate) * options.Scale;
			}

			void AddVertex(Vector2 vertex, bool transformed = false)
			{
				Vector2 v = vertex;
				if (!transformed)
				{
					vertex = Transform(vertex);
				}

				if (vertex != pos)
				{
					posUntransformed = v;
					pos = vertex;
					vertices.Add(new Vector2(vertex.x, -vertex.y));
				}
			}

			bool CanCalcAngle()
			{
				return vertices.Count > 1;
			}

			float GetDeltaAngle(Vector2 vertex)
			{
				//a - previous
				//b - current
				//c - new
				Vector2 a = vertices[vertices.Count - 2];
				Vector2 b = vertices[vertices.Count - 1];
				Vector2 c = Transform(vertex);
				c.y = -c.y;

				Vector2 ab = b - a;
				ab.Normalize();
				Vector2 bc = c - b;
				bc.Normalize();

				float angle = Mathf.Acos(Vector2.Dot(ab, bc));
				//Debug.Log(angle);
				return angle;
			}

			foreach (var pathSegment in path.Segments)
			{
				switch (pathSegment)
				{
					case SVGMoveToSegment segment:
						{
							AddVertex(segment.Position + (segment.Relative ? posUntransformed : Vector2.zero));
						}
						break;
					case SVGLineSegment segment:
						{
							AddVertex(segment.End + (segment.Relative ? posUntransformed : Vector2.zero));
						}
						break;
					case SVGHorizontalLineSegment segment:
						{
							AddVertex(new Vector2(segment.X + (segment.Relative ? posUntransformed.x : 0), posUntransformed.y));
						}
						break;
					case SVGVerticalLineSegment segment:
						{
							AddVertex(new Vector2(posUntransformed.x, segment.Y + (segment.Relative ? posUntransformed.y : 0)));
						}
						break;
					case SVGBezierSegment segment:
						{
							Vector2 a = posUntransformed;
							Vector2 b = segment.Handle1 + (segment.Relative ? a : Vector2.zero);
							if (segment.UsePreviousHandle)
							{
								b = a + (a - previousCurveHandle);
							}
							Vector2 c = segment.Handle2 + (segment.Relative ? a : Vector2.zero);
							Vector2 d = segment.End + (segment.Relative ? a : Vector2.zero);

							int numCurvePoints = 1 + (int)((Transform(d) - Transform(a)).magnitude / curveResolution);
							for (int ii = 0; ii <= numCurvePoints; ii++)
							{
								float t = ii / (float)numCurvePoints;
								Vector2 aa = Vector2.Lerp(a, b, t);
								Vector2 bb = Vector2.Lerp(b, c, t);
								Vector2 cc = Vector2.Lerp(c, d, t);
								Vector2 aaa = Vector2.Lerp(aa, bb, t);
								Vector2 bbb = Vector2.Lerp(bb, cc, t);
								Vector2 p = Vector2.Lerp(aaa, bbb, t);

								if (!CanCalcAngle() || GetDeltaAngle(p) > options.CurveMinAngle || ii == numCurvePoints || ii == 0)
								{
									AddVertex(p);
								}
							}
							previousCurveHandle = c;
						}
						break;
					case SVGEndPathSegment segment:
						{
							if (options.Reverse)
								vertices.Reverse();

							if (vertices.Count > 0 && options.FilterDuplicates)
								vertices = FilterVertices(vertices);

							paths.Add(vertices);
							vertices = new List<Vector2>();
						}
						break;
				}
			}

			return paths;
		}

		protected List<Vector2> FilterVertices(List<Vector2> vertices)
		{
			List<Vector2> filteredVertices = new List<Vector2>();
			filteredVertices.Capacity = vertices.Count;
			Vector2 previous = vertices.Last();
			for (int j = 0; j < vertices.Count; j++)
			{
				Vector2 vertex = vertices[j];
				if (vertex == previous) continue;
				if (Vector2.SqrMagnitude(vertex - previous) < 0.001f) continue;
				filteredVertices.Add(vertex);
				previous = vertex;
			}
			return filteredVertices;
		}
	}
}
