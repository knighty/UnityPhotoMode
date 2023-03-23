using UnityEngine;
using UnityEngine.UI;

namespace Drawing
{
	public class LineRendererHelper
	{
		int index = 0;
		VertexHelper vh;

		public LineRendererHelper(VertexHelper vh)
		{
			this.vh = vh;
			index = vh.currentVertCount;
		}

		public void AddTriangle(Vector3 p0, Vector3 p1, Vector3 p2, Color color)
		{
			vh.AddVert(p0, color, new Vector4(0, 0));
			vh.AddVert(p1, color, new Vector4(0, 1));
			vh.AddVert(p2, color, new Vector4(1, 0));

			vh.AddTriangle(index + 0, index + 1, index + 2);

			index += 3;
		}

		public int AddVert(Vector2 p0, Color color)
		{
			vh.AddVert(p0, color, new Vector4(0, 0));
			index++;
			return index - 1;
		}

		public void AddTriangle(int i0, int i1, int i2)
		{
			vh.AddTriangle(i0, i1, i2);
		}

		public void AddSegment(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Color color)
		{
			vh.AddVert(p0, color, new Vector4(0, 0));
			vh.AddVert(p1, color, new Vector4(0, 1));
			vh.AddVert(p2, color, new Vector4(1, 0));
			vh.AddVert(p3, color, new Vector4(1, 1));

			vh.AddTriangle(index + 0, index + 2, index + 1);
			vh.AddTriangle(index + 2, index + 3, index + 1);

			index += 4;
		}
	}
}