using Codice.CM.WorkspaceServer.Tree;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

namespace Drawing
{
	public class PolygonRenderer
	{
		struct Triangle
		{
			public int a;
			public int b;
			public int c;
		}

		public void RenderOutline(Vector2[] vertices, VertexHelper vh, float width, int layer = 0)
		{
			int last = vertices.Length;

			Vector2 vertexAt(int i)
			{
				return vertices[i % vertices.Length];
			}

			Vector2 getPreviousPoint(int i)
			{
				int index = i - 1;
				return vertices[index < 0 ? (index + vertices.Length) : index];
			}

			Vector2 getNextPoint(int i)
			{
				int index = i + 1;
				return vertices[index % vertices.Length];
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

				return halfway / sinAngle;
			}


			void handleSegment(int i)
			{
				//Vector2 p0 = vertexAt(i > 0 ? i - 1 : vertices.Length - 1);
				Vector2 p1 = vertexAt(i);
				Vector2 p2 = getNextPoint(i);
				Vector2 d1 = p2 - p1;
				float l = (p2 - p1).magnitude;
				d1 /= l;

				float r = width;
				float d = Mathf.Ceil(2.5f + r * 0.5f);

				Vector2 offset1 = getOffset(p1, getPreviousPoint(i), getNextPoint(i)) * d;
				int startIndex = vh.currentVertCount;
				vh.AddVert(p1 + offset1, Color.white, new Vector4(Vector2.Dot(offset1, d1), -d, 0, l), new Vector4(layer, 0, 0, 0), Vector3.zero, Vector4.zero);
				vh.AddVert(p1 - offset1, Color.white, new Vector4(-Vector2.Dot(offset1, d1), d, 0, l), new Vector4(layer, 0, 0, 0), Vector3.zero, Vector4.zero);

				Vector2 offset2 = getOffset(p2, getPreviousPoint(i + 1), getNextPoint(i + 1)) * d;
				vh.AddVert(p2 + offset2, Color.white, new Vector4(l + Vector2.Dot(offset2, d1), -d, 0, l), new Vector4(layer, 0, 0, 0), Vector3.zero, Vector4.zero);
				vh.AddVert(p2 - offset2, Color.white, new Vector4(l - Vector2.Dot(offset2, d1), d, 0, l), new Vector4(layer, 0, 0, 0), Vector3.zero, Vector4.zero);

				vh.AddTriangle(startIndex + 0, startIndex + 1, startIndex + 2);
				vh.AddTriangle(startIndex + 1, startIndex + 3, startIndex + 2);
			}

			for (int i = 0; i < last; i++)
			{
				handleSegment(i);
			}
		}

		public void RenderAntialiasing(Vector2[] vertices, VertexHelper vh, int layer = 0)
		{
			float width = 1;
			int last = vertices.Length;

			Vector2 vertexAt(int i)
			{
				return vertices[i % vertices.Length];
			}

			Vector2 getPreviousPoint(int i)
			{
				int index = i - 1;
				return vertices[index < 0 ? (index + vertices.Length) : index];
			}

			Vector2 getNextPoint(int i)
			{
				int index = i + 1;
				return vertices[index % vertices.Length];
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

				return halfway / sinAngle;
			}


			void handleSegment(int i)
			{
				//Vector2 p0 = vertexAt(i > 0 ? i - 1 : vertices.Length - 1);
				Vector2 p1 = vertexAt(i);
				Vector2 p2 = getNextPoint(i);
				Vector2 d1 = p2 - p1;
				float l = (p2 - p1).magnitude;
				d1 /= l;

				float r = width;
				float d = Mathf.Ceil(2.5f + r * 0.5f);

				Vector2 offset1 = getOffset(p1, getPreviousPoint(i), getNextPoint(i)) * d;
				int startIndex = vh.currentVertCount;
				vh.AddVert(p1 + offset1, Color.white, new Vector4(Vector2.Dot(offset1, d1), -d, 0, l), new Vector4(layer, 0, 0, 0), Vector3.zero, Vector4.zero);
				vh.AddVert(p1, Color.white, new Vector4(0, 0, 0, l), new Vector4(layer, 0, 0, 0), Vector3.zero, Vector4.zero);

				Vector2 offset2 = getOffset(p2, getPreviousPoint(i + 1), getNextPoint(i + 1)) * d;
				vh.AddVert(p2 + offset2, Color.white, new Vector4(l + Vector2.Dot(offset2, d1), -d, 0, l), new Vector4(layer, 0, 0, 0), Vector3.zero, Vector4.zero);
				vh.AddVert(p2, Color.white, new Vector4(l, 0, 0, l), new Vector4(layer, 0, 0, 0), Vector3.zero, Vector4.zero);

				vh.AddTriangle(startIndex + 0, startIndex + 1, startIndex + 2);
				vh.AddTriangle(startIndex + 1, startIndex + 3, startIndex + 2);
			}

			for (int i = 0; i < last; i++)
			{
				handleSegment(i);
			}
		}

		public void RenderFill(FillPaint fill, Vector2[] vertices, VertexHelper vh, int layer = 0)
		{
			List<int> openList = new List<int>(vertices.Length);
			for (int i = 0; i < vertices.Length; i++)
			{
				if (vertices[i] != vertices[(i + 1) % vertices.Length])
					openList.Add(i);
			}

			int nextIndex(int index, int length)
			{
				return (index + 1) % length;
			}

			int prevIndex(int index, int length)
			{
				int i = index - 1;
				return i < 0 ? (i + length) : i;
			}

			Triangle[] triangles = new Triangle[vertices.Length - 2];
			int triangleIndex = 0;

			int emergencyOut = 0;
			bool firstLoop = true;
			while (openList.Count > 2 && emergencyOut < 1000)// && (emergencyOut < bail || bail == 0))
			{
				emergencyOut++;
				bool suitableTriangle = false;
				for (int i = 0; i < openList.Count; i++)
				{
					int prev;
					int p = 0;
					do
					{
						prev = prevIndex(i + p--, openList.Count);
					} while (vertices[openList[i]] == vertices[openList[prev]] && p > -openList.Count);

					int next;
					int n = 0;
					do
					{
						next = nextIndex(i + n++, openList.Count);
					} while (vertices[openList[i]] == vertices[openList[next]] && n < openList.Count);

					// Check if vertex is convex
					Vector2 a = vertices[openList[prev]];
					Vector2 b = vertices[openList[i]];
					Vector2 c = vertices[openList[next]];

					Vector2 ba = a - b;
					Vector2 bc = c - b;

					float magAB = (b - a).magnitude;
					float magBC = (c - b).magnitude;
					float magCA = (a - c).magnitude;

					float min = Mathf.Min(magAB, magBC, magCA);
					float maxDelta = Mathf.Max(Mathf.Abs(magAB / min), Mathf.Abs(magBC / min), Mathf.Abs(magCA / min));
					if (maxDelta > 3 && firstLoop)
					{
						continue;
					}

					bool innerAngle = ba.x * bc.y < bc.x * ba.y;

					if (openList.Count == 3 && !innerAngle)
					{
						openList.Clear();
						break;
					}

					if (!innerAngle) continue;

					// Check if any points inside triangle
					bool hasPointInside = false;
					for (int j = 0; j < openList.Count; j++)
					{
						if (j == i || j == next || j == prev) continue;

						bool getSideOfPlane(Vector2 point, Vector2 p1, Vector2 p2)
						{
							Vector2 plane = p2 - p1;
							Vector2 normal = new Vector2(-plane.y, plane.x);
							return Vector2.Dot((point - p1), normal) < 0;
						}

						Vector2 point = vertices[openList[j]];

						//if (point == vertices[openList[i]] || point == vertices[openList[next]] || point == vertices[openList[prev]]) continue;

						if (getSideOfPlane(point, a, b)) continue;
						if (getSideOfPlane(point, b, c)) continue;
						if (getSideOfPlane(point, c, a)) continue;

						hasPointInside = true;
						break;
					}
					if (hasPointInside) continue;

					suitableTriangle = true;
					triangles[triangleIndex++] = new Triangle() { a = openList[prev], b = openList[i], c = openList[next] };
					openList.RemoveAt(i);
					break;
				}
				firstLoop = suitableTriangle;
			}

			if (emergencyOut >= 1000)
				Debug.Log("Emergency out");

			int firstIndex = vh.currentVertCount;
			for (int v = 0; v < vertices.Length; v++)
			{
				vh.AddVert(vertices[v], Color.white, new Vector4(0,0,1,1), new Vector4(layer, 0, 0, 0), Vector3.zero, Vector4.zero);
			}

			for (int t = 0; t < triangles.Length; t++)
			{
				//Debug.Log($"{triangles[t].a}, {triangles[t].c}, {triangles[t].b}");
				vh.AddTriangle(triangles[t].a + firstIndex, triangles[t].c + firstIndex, triangles[t].b + firstIndex);
			}

			RenderAntialiasing(vertices, vh, layer);
		}
	}
}
