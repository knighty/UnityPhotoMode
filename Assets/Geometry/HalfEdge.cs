using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Geometry
{
	public struct HalfMeshTriangle
	{
		public Vector2 a, b, c;
	}

	public class HalfEdgeMeshTriangleEnumerator : IEnumerator<HalfMeshTriangle>, IEnumerable<HalfMeshTriangle>
	{
		private HalfEdgeMesh mesh;
		int faceIndex = -1;
		HashSet<HalfEdgeFace>.Enumerator enumerator;

		public HalfEdgeMeshTriangleEnumerator(HalfEdgeMesh mesh)
		{
			this.mesh = mesh;
			enumerator = mesh.faces.GetEnumerator();
		}

		public HalfMeshTriangle Current
		{
			get
			{
				return new HalfMeshTriangle()
				{
					a = enumerator.Current.halfEdge.v.position,
					b = enumerator.Current.halfEdge.next.v.position,
					c = enumerator.Current.halfEdge.next.next.v.position,
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
			enumerator.MoveNext();
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
			//pool.Push(item);
		}

		public T Request()
		{
			return create();
			T item = pool.Pop();
			reset(item);
			return item;
		}
	}

	public class HalfEdgeMesh
	{
		public HalfMeshPool<HalfEdgeFace> facePool = new HalfMeshPool<HalfEdgeFace>(0, () => new HalfEdgeFace(), face =>
		{
			face.halfEdge = null;
		});
		public HalfMeshPool<HalfEdge> edgePool = new HalfMeshPool<HalfEdge>(0, () => new HalfEdge(), edge =>
		{
			edge.next = null;
			edge.v = null;
			edge.twin = null;
			edge.face = null;
		});

		public Dictionary<Vector2, HalfEdgeVertex> vertices = new Dictionary<Vector2, HalfEdgeVertex>();
		public HashSet<HalfEdgeFace> faces = new HashSet<HalfEdgeFace>();
		public HashSet<HalfEdge> edges = new HashSet<HalfEdge>();

		static List<HalfEdge> removedHalfEdges = new List<HalfEdge>();
		static HashSet<HalfEdgeFace> checkedForInside = new HashSet<HalfEdgeFace>();
		Stack<HalfEdge> edgesToInvestigate = new Stack<HalfEdge>();

		Dictionary<Vector2Int, HalfEdgeVertex> verticesGrid = new Dictionary<Vector2Int, HalfEdgeVertex>();
		float vertexGridResolution = 10;

		public IEnumerable<HalfMeshTriangle> EnumerateTriangles()
		{
			return new HalfEdgeMeshTriangleEnumerator(this);
		}

		private HalfEdgeVertex AddVertex(Vector2 position)
		{
			HalfEdgeVertex vertex = new HalfEdgeVertex(position);
			vertices.Add(position, vertex);
			Vector2Int vertexGridPosition = new Vector2Int((int)(position.x * vertexGridResolution), (int)(position.y * vertexGridResolution));
			verticesGrid[vertexGridPosition] = vertex;
			return vertex;
		}

		public HalfEdgeVertex AddVertex(HalfEdgeVertex vertex)
		{
			vertices.Add(vertex.position, vertex);
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
			HalfEdgeFace face = GetFaceForPoint(position);
			if (face == null)
				return;

			// Create the new vertex at split position
			HalfEdgeVertex vertex;
			try
			{
				vertex = AddVertex(position);
			}
			catch (Exception e)
			{
				Debug.Log("Avoided adding duplicate vertex");
				return;
			}

			// Remove the face and collect the half edges it had
			removedHalfEdges.Clear();
			RemoveFace(face, removedHalfEdges);

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

				b = removedHalfEdge;
				b.face = newFace;
				/*b.v = removedHalfEdge.next.v;
				b.face = newFace;
				b.twin = removedHalfEdge;*/

				c.v = removedHalfEdge.next.v;
				c.face = newFace;

				a.next = b;
				b.next = c;
				c.next = a;

				// Failsafe for exterior triangles
				if (b.twin.next != null)
					edgesToInvestigate.Push(b.twin);

				// The removed half edge is now twinned with our new b edge
				//removedHalfEdge.twin = b;

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

		public HalfEdgeVertex GetVertex(Vector2 position)
		{
			if (vertices.TryGetValue(position, out HalfEdgeVertex vertex))
			{
				return vertex;
			}
			return null;
		}

		public HalfEdgeFace GetFaceForPoint(Vector2 position)
		{
			HalfEdgeFace face = faces.First();
			Vector2Int vertexGridPosition = new Vector2Int((int)(position.x * vertexGridResolution), (int)(position.y * vertexGridResolution));
			if (verticesGrid.TryGetValue(vertexGridPosition, out HalfEdgeVertex startVertex))
			{
				face = startVertex.halfEdge.face;
			}

			int emergency = 0;
			while (emergency < 1000)
			{
				emergency++;

				HalfEdge edge = face.halfEdge;
				bool changedFace = false;
				do
				{
					if (!edge.PointInside(position))// && !checkedForInside.Contains(edge.twin.face))
					{
						face = edge.twin.face;
						changedFace = true;
						break;
					}
					edge = edge.next;
				} while (edge != face.halfEdge);
				if (!changedFace)
					return face;
			}

			Debug.Log("Failed to find triangle to split");
			return null;
		}

		public void FlipEdge(HalfEdge edge)
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

			// Also need to update the vertices to the right half edge
			twinNext.v.halfEdge = twinNext;
			next.v.halfEdge = next;
		}

		public bool RemoveFace(HalfEdgeFace face, List<HalfEdge> halfEdges = null)
		{
			if (!faces.Contains(face))
				return false;

			HalfEdge halfEdge = face.halfEdge;
			do
			{
				HalfEdge next = halfEdge.next;
				if (halfEdges != null)
					halfEdges.Add(halfEdge);
				halfEdge.face = null;
				//edgePool.Release(halfEdge);
				halfEdge = next;
			} while (halfEdge != face.halfEdge);
			faces.Remove(face);
			facePool.Release(face);

			return true;
		}

		public bool RemoveEdge(HalfEdge edge)
		{
			// Check edge edge is in the structure. This may stop being true for edges suddenly due to other deletions
			if (!edges.Contains(edge))
				return false;

			// See if the vertex points to this edge.
			if (edge.v.halfEdge == edge)
			{
				// If it does then we need to move it to another one
				edge.v.halfEdge = edge?.twin?.next ?? null;
			}

			// Check if we removed a face
			if (edge.face != null)
			{
				int numEdges = 0;
				HalfEdge faceEdge = edge;
				do
				{
					numEdges++;
					faceEdge = faceEdge.next;
				} while (faceEdge != edge);

				// If it's less than 4 we can't possibly still have a face (we're deleting the 3rd)
				if (numEdges < 4)
				{
					RemoveFace(edge.face);
				}
				else
				{
					// Otherwise, make sure the face doesn't point to our deleted edge
					edge.face.halfEdge = edge.next;
				}
			}

			edge.twin = null;
			edge.next = null;
			edge.v = null;
			edge.face = null; 

			edges.Remove(edge);

			return true;
		}

		public bool RemoveVertex(HalfEdgeVertex vertex)
		{
			if (!vertices.ContainsKey(vertex.position))
				return false;

			vertices.Remove(vertex.position);

			// Remove attached edges
			HalfEdge last = vertex.halfEdge;
			HalfEdge edge = last;
			do
			{
				HalfEdge twin = edge.twin;
				HalfEdge next = twin.next;

				if (RemoveEdge(edge))
				{
					Debug.Log("Removed edge");
				}
				if (RemoveEdge(twin))
				{
					Debug.Log("removed twin");
				}

				edge = next;
			} while (edge != last);

			return true;
		}

		internal void Reset()
		{
			vertices = new Dictionary<Vector2, HalfEdgeVertex>();
			faces = new HashSet<HalfEdgeFace>();
			edges = new HashSet<HalfEdge>();
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
			return GeometryUtils.PointOnRightOfPlane(v.position, next.v.position, position);
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

		public override int GetHashCode()
		{
			return position.GetHashCode();
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
