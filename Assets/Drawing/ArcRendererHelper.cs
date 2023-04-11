using UnityEngine;
using UnityEngine.UI;

namespace Drawing
{
	public struct ArcRendererHelper
	{
		internal FillPaint fillPaint;
		internal VertexHelper vertexHelper;
		int index;

		public ArcRendererHelper(FillPaint fillPaint, VertexHelper vh)
		{
			index = vh.currentVertCount;
			this.fillPaint = fillPaint;
			this.vertexHelper = vh;
		}

		public void AddVert(Vector2 position)
		{
			vertexHelper.AddVert(new UIVertex() { position = position, color = fillPaint.Color });
		}

		public void AddTriangle(int index0, int index1, int index2)
		{
			vertexHelper.AddTriangle(index + index0, index + index1, index + index2);
		}

		public void Begin()
		{
			index = vertexHelper.currentVertCount;
		}
	}
}