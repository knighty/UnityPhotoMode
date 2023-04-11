using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Geometry
{
	public class DelaunayTriangulation
	{
		public struct Constraint
		{
			public int a;
			public int b;
		}

		public class Result
		{
			HalfEdgeMesh mesh;
			private TimeSpan runtime;

			public Result(HalfEdgeMesh mesh, TimeSpan runtime)
			{
				this.mesh = mesh;
				this.runtime = runtime;
			}

			public HalfEdgeMesh Mesh { get => mesh; }
			public TimeSpan Runtime { get => runtime; }
		}

		public Result Triangulate(List<Vector2> vertices, bool normalize = true)
		{
			System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
			stopWatch.Start();

			HalfEdgeMesh mesh = new HalfEdgeMesh();

			// Create super triangle
			HalfEdgeFace face = mesh.facePool.Request();

			HalfEdgeVertex a = new HalfEdgeVertex(new Vector2(0, 100));
			HalfEdgeVertex b = new HalfEdgeVertex(new Vector2(100, -100));
			HalfEdgeVertex c = new HalfEdgeVertex(new Vector2(-100, -100));

			HalfEdge ha = mesh.edgePool.Request();
			ha.face = face;
			ha.v = a;
			HalfEdge hb = mesh.edgePool.Request();
			hb.face = face;
			hb.v = b;
			HalfEdge hc = mesh.edgePool.Request();
			hc.face = face;
			hc.v = c;

			ha.next = hb;
			hb.next = hc;
			hc.next = ha;

			HalfEdge hai = mesh.edgePool.Request();
			hai.face = face;
			hai.v = a;
			HalfEdge hbi = mesh.edgePool.Request();
			hbi.face = face;
			hbi.v = b;
			HalfEdge hci = mesh.edgePool.Request();
			hci.face = face;
			hci.v = c;

			hai.next = hci;
			hbi.next = hai;
			hci.next = hbi;

			ha.twin = hai;
			hai.twin = ha;
			hb.twin = hbi;
			hbi.twin = hb;
			hc.twin = hci;
			hci.twin = hc;

			face.halfEdge = ha;

			mesh.faces.Add(face);
			mesh.AddVertex(a);
			mesh.AddVertex(b);
			mesh.AddVertex(c);
			mesh.edges.Add(ha);
			mesh.edges.Add(hb);
			mesh.edges.Add(hc);

			// Normalize points
			Vector2 min = Vector2.zero;
			Vector2 max = Vector2.zero;
			if (normalize)
			{
				vertices = new List<Vector2>(vertices);
				NormalizeVertices(ref vertices, out min, out max);
			}

			// Split on each point
			foreach (var point in vertices)
			{
				mesh.Split(point);
			}

			// Delete super triangle faces
			List<HalfEdgeFace> facesToDelete = new List<HalfEdgeFace>();
			foreach (var triangle in mesh.faces)
			{
				HalfEdge edge = triangle.halfEdge;
				do
				{
					if (edge.v == a || edge.v == b || edge.v == c)
					{
						facesToDelete.Add(triangle);
						break;
					}
					edge = edge.next;
				} while (edge != triangle.halfEdge); 
			}	
			List<HalfEdge> deletedHalfEdges = new List<HalfEdge>();
			foreach (var triangle in facesToDelete)
			{
				mesh.RemoveFace(triangle, deletedHalfEdges);
			}
			/*Debug.Log("Remove A");
			mesh.RemoveVertex(a);
			Debug.Log("Remove B");
			mesh.RemoveVertex(b);
			Debug.Log("Remove C");
			mesh.RemoveVertex(c);*/

			stopWatch.Stop();

			// Denormalize the points
			if (normalize)
				DenormalizeVertices(mesh, min, max);

			return new Result(mesh, stopWatch.Elapsed);
		}

		public List<HalfEdge> validConstraints = new List<HalfEdge>();
		public List<HalfEdge> debugEdges = new List<HalfEdge>();
		public Result ConstrainedTriangulation(List<Vector2> vertices, List<Constraint> constraints, int maxFlips = 0, bool interior = true, bool exterior = false)
		{
			System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
			stopWatch.Start();

			// Normalize
			List<Vector2> normalizedVertices = new List<Vector2>(vertices);
			NormalizeVertices(ref normalizedVertices, out Vector2 min, out Vector2 max);

			// Triangulate using normal delaunay
			Result triangulation = Triangulate(normalizedVertices, false);

			Vector2 invDimensions = new Vector2(1 / (max.x - min.x), 1 / (max.y - min.y));
			Vector2 normalize(Vector2 point)
			{
				return new Vector2(
					(point.x - min.x) * invDimensions.x,
					(point.y - min.y) * invDimensions.y
				);
			}

			// Constrain all the edges provided and get a list of the valid ones
			HashSet<HalfEdge> constraintEdges = ConstrainEdges(triangulation.Mesh, vertices, constraints, normalize, maxFlips);
			//Debug.Log($"{constraintEdges.Count} constraint edges");

			// Remove exterior faces
			if (!exterior)
			{
				RemoveExteriorFaces(triangulation.Mesh, constraintEdges);
			}

			// Denormalize
			DenormalizeVertices(triangulation.Mesh, min, max);

			return new Result(triangulation.Mesh, stopWatch.Elapsed);
		}

		private HashSet<HalfEdge> ConstrainEdges(HalfEdgeMesh mesh, List<Vector2> vertices, List<Constraint> constraints, Func<Vector2, Vector2> normalize, int maxFlips)
		{
			HashSet<HalfEdge> edges = new HashSet<HalfEdge>();
			foreach (var constraint in constraints)
			{
				// Get the vertex points for this constraint
				HalfEdgeVertex a = mesh.GetVertex(normalize(vertices[constraint.a]));
				HalfEdgeVertex b = mesh.GetVertex(normalize(vertices[constraint.b]));

				// If a/b don't exist then the mesh is invalid
				if (a == null || b == null)
				{
					throw new Exception("Couldn't find the constrained vertices");
				}

				// Find the edge that points here
				HalfEdge edge = GetConstraintEdge(a, b);

				// If we find an edge then we can bail now
				if (edge != null)
				{
					//Debug.Log("constraint valid before");
					validConstraints.Add(edge);
					edges.Add(edge);
					continue;
				}

				// Constrain this edge
				ConstrainEdge(mesh, a, b, maxFlips);

				// Add it to the list of valid constraints if found
				edge = GetConstraintEdge(a, b);
				if (edge == null)
				{
					// If the constraint didn't end up in the list, it's probably because of some coplanar vertices
					// To fix this we'll just path find from a to b
					/*HalfEdge e = a.halfEdge;
					do
					{
						e = e.twin.next;
					} while (e != a.);*/
					Debug.Log("Somehow didn't find the constraint");
				}
				else
				{
					//Debug.Log("constraint valid");
					validConstraints.Add(edge);
					edges.Add(edge);
				}
			}

			return edges;
		}

		private void ConstrainEdge(HalfEdgeMesh triangulation, HalfEdgeVertex a, HalfEdgeVertex b, int maxFlips)
		{
			// Get all the edges that initially intersect with the constraint
			Queue<HalfEdge> intersectingEdges = GetIntersectingEdges(a, b);
			//Debug.Log($"{intersectingEdges.Count} intersecting edges");

			/*foreach(var e in intersectingEdges)
				debugEdges.Add(e);*/

			int safety = 0;
			while (intersectingEdges.Count > 0 && safety < 1000 && safety < maxFlips)
			{
				safety++;

				HalfEdge edge = intersectingEdges.Dequeue();

				// See if we can flip this edge
				if (IsConvex(edge.v.position, edge.next.v.position, edge.next.next.v.position, edge.twin.next.next.v.position))
				{
					triangulation.FlipEdge(edge);

					//Check if this flipped edge intersects, if so add to the queue
					//bool edgeIsOnContraint = a == edge.v || b == edge.next.v || a == edge.next.v || b == edge.v;
					//if (!edgeIsOnContraint && LinesIntersecting(a.position, b.position, edge.v.position, edge.next.v.position))
					if (EdgeIntersectsConstraint(a, b, edge, true))
					{
						intersectingEdges.Enqueue(edge);
					}
				}
				else
				{
					intersectingEdges.Enqueue(edge);
				}
			}
			//Debug.Log($"Fixed constraint from {a.position} to {b.position} with {safety} flips");
			if (safety > 995)
			{
				Debug.Log("Had to bail on solving constraint");
			}
		}

		private void RemoveExteriorFaces(HalfEdgeMesh triangulation, HashSet<HalfEdge> constraintEdges)
		{
			HashSet<HalfEdgeFace> checkedFaces = new HashSet<HalfEdgeFace>();
			Queue<HalfEdgeFace> facesToCheck = new Queue<HalfEdgeFace>();

			foreach (var constrainedEdge in constraintEdges)
			{
				if (constrainedEdge.twin.face != null && !checkedFaces.Contains(constrainedEdge.twin.face))
				{
					facesToCheck.Enqueue(constrainedEdge.twin.face);
					checkedFaces.Add(constrainedEdge.twin.face);
				}
			}

			List<HalfEdge> halfEdges = new List<HalfEdge>();
			HashSet<HalfEdgeFace> trianglesToRemove = new HashSet<HalfEdgeFace>();

			int removedFaces = 0;
			while (facesToCheck.Count > 0)
			{
				HalfEdgeFace face = facesToCheck.Dequeue();

				// Add surrounding faces that don't cross the constraint line and aren't already in the checked list
				HalfEdge edge = face.halfEdge;
				do
				{
					if (edge.twin.face != null && !constraintEdges.Contains(edge.twin))
					{
						if (!checkedFaces.Contains(edge.twin.face))
						{
							facesToCheck.Enqueue(edge.twin.face);
							checkedFaces.Add(edge.twin.face);
						}
					}
					edge = edge.next;
				} while (edge != face.halfEdge);

				trianglesToRemove.Add(face);
				removedFaces++;
			}

			foreach (var triangle in trianglesToRemove)
			{
				halfEdges.Clear();
				triangulation.RemoveFace(triangle, halfEdges);
			}

			//Debug.Log($"Removed {removedFaces} faces");
		}

		private HalfEdge GetConstraintEdge(HalfEdgeVertex a, HalfEdgeVertex b)
		{
			// We just rotate around the edges connected to the vertex and return one that matches if it does
			HalfEdge edge = a.halfEdge;
			do
			{
				if (edge.next.v == b)
				{
					return edge;
				}
				edge = edge.twin.next;
			} while (edge != a.halfEdge);

			return null;
		}

		public Queue<HalfEdge> GetIntersectingEdges(HalfEdgeVertex a, HalfEdgeVertex b)
		{
			Queue<HalfEdge> intersectingEdges = new Queue<HalfEdge>();

			// Find the first face
			HalfEdge first = FindFirstIntersectingEdge(a, b);
			if (first == null)
			{
				Debug.Log("Didn't find first intersecting edge");
			}

			intersectingEdges.Enqueue(first);

			Queue<HalfEdge> facesToCheck = new Queue<HalfEdge>();
			facesToCheck.Enqueue(first.twin);

			// Iterate over each intersecting face to find the next one and collect the half edges
			int safety = 0;
			while (facesToCheck.Count > 0 && safety++ < 1000)
			{
				HalfEdge face = facesToCheck.Dequeue();
				HalfEdge intersectingEdge = GetIntersectingEdge2(a, b, face);
				if (intersectingEdge != null)
				{
					intersectingEdges.Enqueue(intersectingEdge);
					facesToCheck.Enqueue(intersectingEdge.twin);
				}
			}
			return intersectingEdges;
		}

		private bool IsConvex(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
		{
			bool ccw(Vector2 a, Vector2 b, Vector2 c)
			{
				return ((b.x - a.x) * (c.y - a.y) - (c.x - a.x) * (b.y - a.y)) >= 0;
			}

			bool abc = ccw(a, b, c);
			bool abd = ccw(a, b, d);
			bool bcd = ccw(b, c, d);
			bool cad = ccw(c, a, d);

			if (abc && abd && bcd & !cad)
			{
				return true;
			}
			else if (abc && abd && !bcd & cad)
			{
				return true;
			}
			else if (abc && !abd && bcd & cad)
			{
				return true;
			}
			//The opposite sign, which makes everything inverted
			else if (!abc && !abd && !bcd & cad)
			{
				return true;
			}
			else if (!abc && !abd && bcd & !cad)
			{
				return true;
			}
			else if (!abc && abd && !bcd & !cad)
			{
				return true;
			}

			return false;
		}

		private HalfEdge GetIntersectingEdge1(HalfEdgeVertex a, HalfEdgeVertex b, HalfEdge faceEdge)
		{
			// We're starting at an already intersecting edge, so iterate over the remaining ones to find the next intersection
			HalfEdge edge = faceEdge.next;
			do
			{
				//If a or b are on the edge then it's not intersecting
				bool isOnAOrB = a == edge.v || a == edge.next.v || b == edge.v || b == edge.next.v;

				if (!isOnAOrB && GeometryUtils.LinesIntersecting(a.position, b.position, edge.v.position, edge.next.v.position))
					return edge;
				edge = edge.next;
			} while (edge != faceEdge);

			return null;
		}

		private HalfEdge GetIntersectingEdge2(HalfEdgeVertex a, HalfEdgeVertex b, HalfEdge faceEdge)
		{
			// We're starting at an already intersecting edge, so iterate over the remaining ones to find the next intersection
			HalfEdge edge = faceEdge.next;
			do
			{
				//debugEdges.Add(edge);

				if (EdgeIntersectsConstraint(a, b, edge))
				{
					return edge;
				}

				edge = edge.next;
			} while (edge != faceEdge);

			return null;
		}

		private bool EdgeIntersectsConstraint(HalfEdgeVertex a, HalfEdgeVertex b, HalfEdge edge, bool eitherSide = false)
		{
			bool isOnAOrB = a == edge.v || a == edge.next.v || b == edge.v || b == edge.next.v;
			if (isOnAOrB)
				return false;

			bool aSide = GeometryUtils.PointOnRightOfPlane(a.position, b.position, edge.v.position);
			bool bSide = GeometryUtils.PointOnRightOfPlane(a.position, b.position, edge.next.v.position);
			if (eitherSide)
			{
				return aSide != bSide;
			}
			return !aSide && bSide;
		}

		private HalfEdge FindFirstIntersectingEdge(HalfEdgeVertex a, HalfEdgeVertex b)
		{
			HalfEdge edge = a.halfEdge.twin;
			do
			{
				HalfEdge next = edge.next.twin;
				if (edge.PointInside(b.position) && edge.next.PointInside(b.position))
				{
					return edge.next.next;
				}
				edge = next;
			} while (edge != a.halfEdge.twin);

			return null;
		}

		private void NormalizeVertices(ref List<Vector2> vertices, out Vector2 min, out Vector2 max)
		{
			max = Vector2.zero;
			min = Vector2.zero;
			for (int i = 0; i < vertices.Count; i++)
			{
				min.x = Mathf.Min(vertices[i].x, min.x);
				min.y = Mathf.Min(vertices[i].y, min.y);
				max.x = Mathf.Max(vertices[i].x, max.x);
				max.y = Mathf.Max(vertices[i].y, max.y);
			}
			Vector2 dimensions = max - min;
			if (dimensions.y > dimensions.x)
			{
				min.x *= dimensions.y / dimensions.x;
				max.x *= dimensions.y / dimensions.x;
			}
			else
			{
				min.y *= dimensions.x / dimensions.y;
				max.y *= dimensions.x / dimensions.y;
			}

			Vector2 invDimensions = new Vector2(1 / (max.x - min.x), 1 / (max.y - min.y));

			for (int i = 0; i < vertices.Count; i++)
			{
				vertices[i] = new Vector2(
					(vertices[i].x - min.x) * invDimensions.x,
					(vertices[i].y - min.y) * invDimensions.y
				);
			}
		}

		private void DenormalizeVertices(HalfEdgeMesh mesh, Vector2 min, Vector2 max)
		{
			Dictionary<Vector2, HalfEdgeVertex> verts = new Dictionary<Vector2, HalfEdgeVertex>(mesh.vertices);
			mesh.vertices.Clear();
			Vector2 delta = max - min;
			foreach (var kv in verts)
			{
				HalfEdgeVertex vertex = kv.Value;
				vertex.position = vertex.position * delta + min;
				mesh.vertices.Add(vertex.position, vertex);
			}
		}
	}
}
