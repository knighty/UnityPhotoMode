using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DrawingOld
{
	public struct HalfMeshTriangle
	{
		public Vector2 a, b, c;
	}

	public class HalfEdgeMeshTriangleEnumerator : IEnumerator<HalfMeshTriangle>, IEnumerable<HalfMeshTriangle>
	{
		private HalfEdgeMesh mesh;
		int faceIndex = -1;

		public HalfEdgeMeshTriangleEnumerator(HalfEdgeMesh mesh)
		{
			this.mesh = mesh;
		}

		public HalfMeshTriangle Current
		{
			get
			{
				return new HalfMeshTriangle()
				{
					a = mesh.faces[faceIndex].halfEdge.v.position,
					b = mesh.faces[faceIndex].halfEdge.next.v.position,
					c = mesh.faces[faceIndex].halfEdge.next.next.v.position,
				};
			}
		}

		object IEnumerator.Current => throw new System.NotImplementedException();

		public void Dispose()
		{
		}

		public IEnumerator<HalfMeshTriangle> GetEnumerator()
		{
			return this;
		}

		public bool MoveNext()
		{
			if (faceIndex < mesh.faces.Count - 1)
			{
				faceIndex++;
				return true;
			}
			return false;
		}

		public void Reset()
		{
			faceIndex = -1;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this;
		}
	}

	public class HalfMeshPool<T>
	{
		Stack<T> pool;
		Action<T> reset;
		Func<T> create;

		public HalfMeshPool(int size, Func<T> create, Action<T> reset)
		{
			this.create = create;
			this.reset = reset;
			pool = new Stack<T>(size);
			for (int i = 0; i < size; i++)
			{
				pool.Push(create());
			}
		}

		public void Release(T item)
		{
			pool.Push(item);
		}

		public T Request()
		{
			T item = pool.Pop();
			reset(item);
			return item;
		}
	}

	public class HalfEdgeMesh
	{
		public HalfMeshPool<HalfEdgeFace> facePool = new HalfMeshPool<HalfEdgeFace>(30000, () => new HalfEdgeFace(), face =>
		{
			face.halfEdge = null;
		});
		public HalfMeshPool<HalfEdge> edgePool = new HalfMeshPool<HalfEdge>(30000, () => new HalfEdge(), edge =>
		{
			edge.next = null;
			edge.v = null;
			edge.twin = null;
			edge.face = null;
		});

		public List<HalfEdgeVertex> vertices = new List<HalfEdgeVertex>();
		public List<HalfEdgeFace> faces = new List<HalfEdgeFace>();
		public List<HalfEdge> edges = new List<HalfEdge>();

		static List<HalfEdge> removedHalfEdges = new List<HalfEdge>();
		static List<HalfEdgeFace> checkedForInside = new List<HalfEdgeFace>();
		Stack<HalfEdge> edgesToInvestigate = new Stack<HalfEdge>();

		public IEnumerable<HalfMeshTriangle> EnumerateTriangles()
		{
			return new HalfEdgeMeshTriangleEnumerator(this);
		}

		private HalfEdgeVertex AddVertex(Vector2 position)
		{
			HalfEdgeVertex vertex = new HalfEdgeVertex(position);
			vertices.Add(vertex);
			return vertex;
		}

		private HalfEdgeFace AddFace()
		{
			HalfEdgeFace newFace = facePool.Request();
			faces.Add(newFace);
			return newFace;
		}

		public void Split(Vector2 position)
		{
			checkedForInside.Clear();

			HalfEdgeFace face = GetFaceForPoint(position);
			if (face == null)
				return;

			// Create the new vertex at split position
			HalfEdgeVertex vertex = AddVertex(position);

			// Remove the face and collect the half edges it had
			removedHalfEdges.Clear();
			RemoveFace(face, ref removedHalfEdges);

			edgesToInvestigate.Clear();

			// Each half edge should make a new triangle
			HalfEdge previousHalfEdge = null;
			HalfEdge firstHalfEdge = null;
			foreach (HalfEdge removedHalfEdge in removedHalfEdges)
			{
				// Create the new face
				HalfEdgeFace newFace = AddFace();

				// Create 3 new half edges using the new vertex and the removed half edge
				HalfEdge a = edgePool.Request();
				HalfEdge b = edgePool.Request();
				HalfEdge c = edgePool.Request();

				a.v = vertex;
				a.face = newFace;
				a.twin = previousHalfEdge;

				b.v = removedHalfEdge.next.v;
				b.face = newFace;
				b.twin = removedHalfEdge;

				c.v = removedHalfEdge.v;
				c.face = newFace;

				a.next = b;
				b.next = c;
				c.next = a;

				// Failsafe for exterior triangles
				if (b.twin.next != null)
					edgesToInvestigate.Push(b.twin);

				// The removed half edge is now twinned with our new b edge
				removedHalfEdge.twin = b;

				// Match up the previous c edge with this face's a edge
				if (previousHalfEdge != null)
					previousHalfEdge.twin = a;

				// Add the new edges to the edge list
				edges.Add(a);
				edges.Add(b);
				edges.Add(c);

				// Use the a edge as the half edge for our new face. This is entirely arbitrary
				newFace.halfEdge = a;

				// Keep a point to the c edge so we can twin it with the next a edge
				previousHalfEdge = c;

				// Store the first half edge we make so we can twin it later
				if (firstHalfEdge == null) firstHalfEdge = a;
			}

			// Match up the first/last ones as twins
			firstHalfEdge.twin = previousHalfEdge;
			previousHalfEdge.twin = firstHalfEdge;

			// Use the first as the half edge for the new vertex
			vertex.halfEdge = firstHalfEdge;

			int safety = 0;
			while (edgesToInvestigate.Count > 0)
			{
				safety += 1;
				if (safety > 1000000)
				{
					Debug.Log("Stuck in infinite loop when restoring delaunay in incremental sloan algorithm");
					break;
				}

				HalfEdge edgeToTest = edgesToInvestigate.Pop();

				Vector2 a = edgeToTest.v.position;
				Vector2 b = edgeToTest.next.v.position;
				Vector2 c = edgeToTest.next.next.v.position;

				bool ShouldFlipEdgeStable(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 vp)
				{
					float x_13 = v1.x - v3.x;
					float x_23 = v2.x - v3.x;
					float x_1p = v1.x - vp.x;
					float x_2p = v2.x - vp.x;

					float y_13 = v1.y - v3.y;
					float y_23 = v2.y - v3.y;
					float y_1p = v1.y - vp.y;
					float y_2p = v2.y - vp.y;

					float cos_a = x_13 * x_23 + y_13 * y_23;
					float cos_b = x_2p * x_1p + y_2p * y_1p;

					if (cos_a >= 0f && cos_b >= 0f)
					{
						return false;
					}
					if (cos_a < 0f && cos_b < 0)
					{
						return true;
					}

					float sin_ab = (x_13 * y_23 - x_23 * y_13) * cos_b + (x_2p * y_1p - x_1p * y_2p) * cos_a;

					if (sin_ab < 0)
					{
						return true;
					}

					return false;
				}

				// BAC because it assumes CCW winding
				// BA is the flipping edge and must be the first two parameters
				if (ShouldFlipEdgeStable(b, a, c, vertex.position))
				{
					if (!edgesToInvestigate.Contains(edgeToTest.next.twin))
						edgesToInvestigate.Push(edgeToTest.next.twin);
					if (!edgesToInvestigate.Contains(edgeToTest.next.next.twin))
						edgesToInvestigate.Push(edgeToTest.next.next.twin);

					FlipEdge(edgeToTest);
				}
			}

			return;
		}

		public HalfEdgeFace GetFaceForPoint(Vector2 position)
		{
			HalfEdgeFace face = faces.First();

			int emergency = 0;
			while (emergency < 0)
			{
				emergency++;

				if (face.PointInside(position))
				{
					return face;
				}

				float closestDist = 0;
				HalfEdge closest = null;
				HalfEdge edge = face.halfEdge;
				bool changedFace = false;
				do
				{
					if (!edge.PointInside(position) && !checkedForInside.Contains(edge.twin.face))
					{
						face = edge.twin.face;
						checkedForInside.Add(edge.twin.face);
						changedFace = true;
						break;
					}
					/*Vector2 center = (edge.v.position + edge.next.v.position + edge.next.next.v.position) / 3;
					float dist = Vector2.SqrMagnitude(center - position);
					if (dist < closestDist || closest == null)
					{
						closestDist = dist;
						closest = edge;
					}*/
					edge = edge.next;
				} while (edge != face.halfEdge);
				if (!changedFace)
					break;
				//face = closest.twin.face;
			}

			foreach (var f in faces)
			{
				if (f.PointInside(position))
				{
					return f;
				}
			}

			Debug.Log("Failed to find triangle to split");
			return null;
		}

		private void FlipEdge(HalfEdge edge)
		{
			// Cache references
			HalfEdgeFace faceA = edge.face;
			HalfEdgeFace faceB = edge.twin.face;

			HalfEdge next = edge.next;
			HalfEdge prev = next.next;

			HalfEdge twin = edge.twin;
			HalfEdge twinNext = twin.next;
			HalfEdge twinPrev = twinNext.next;

			// Update one side
			edge.v = prev.v;
			next.next = edge;
			edge.next = twinPrev;
			twinPrev.next = next;

			// Update twin
			twin.v = twinPrev.v;
			twin.next = prev;
			prev.next = twinNext;
			twinNext.next = twin;

			// We have to update the faces incase any of the faces used a flipped edge as its half edge
			// Two of the half edges also change the face they're in
			faceA.halfEdge = edge;
			twinPrev.face = faceA;
			faceB.halfEdge = edge.twin;
			prev.face = faceB;
		}

		public void RemoveFace(HalfEdgeFace face, ref List<HalfEdge> halfEdges)
		{
			HalfEdge halfEdge = face.halfEdge;
			do
			{
				HalfEdge next = halfEdge.next;
				edges.Remove(halfEdge);
				if (halfEdge.twin != null)
				{
					halfEdge.twin.twin = null;
					halfEdges.Add(halfEdge.twin);
				}
				edgePool.Release(halfEdge);
				halfEdge = next;
			} while (halfEdge != face.halfEdge);
			faces.Remove(face);
			facePool.Release(face);
		}

		internal void Reset()
		{
			vertices = new List<HalfEdgeVertex>();
			faces = new List<HalfEdgeFace>();
			edges = new List<HalfEdge>();
		}
	}

	public class HalfEdge
	{
		public HalfEdgeVertex v;
		public HalfEdgeFace face;
		public HalfEdge next;
		public HalfEdge twin;

		public bool PointInside(Vector2 position)
		{
			Vector2 plane = next.v.position - v.position;
			Vector2 normal = new Vector2(-plane.y, plane.x);
			return Vector2.Dot((position - v.position), normal) <= 0;
		}
	}

	public class HalfEdgeVertex
	{
		public Vector2 position;
		public HalfEdge halfEdge;

		public HalfEdgeVertex(Vector2 position)
		{
			this.position = position;
		}
	}

	public class HalfEdgeFace
	{
		public HalfEdge halfEdge;

		public bool PointInside(Vector2 point)
		{
			// Iterate over all edges in this face and see if the point is outside all edges
			HalfEdge edge = halfEdge;
			do
			{
				if (!edge.PointInside(point)) return false;
				edge = edge.next;
			} while (edge != halfEdge);

			return true;
		}
	}
}