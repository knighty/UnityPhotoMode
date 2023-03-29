using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoMode.UI
{
	public class ImageTexture : Graphic
	{
		[SerializeField] float scale = 1;
		[SerializeField] Texture2D texture;

		public override Texture mainTexture => texture;

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			if (mainTexture == null)
				return;
			Rect rect = GetPixelAdjustedRect();
			float w = (rect.width / mainTexture.width) * scale;
			float h = (rect.height / mainTexture.height) * scale;
			vh.AddUIVertexQuad(new UIVertex[] {
				new UIVertex(){ position = new Vector2(rect.xMin, rect.yMax), color = Color.white, uv0 = new Vector4(0, 0, 0, 0)},
				new UIVertex(){ position = new Vector2(rect.xMin, rect.yMin), color = Color.white, uv0 = new Vector4(0, h, 0, 0)},
				new UIVertex(){ position = new Vector2(rect.xMax, rect.yMin), color = Color.white, uv0 = new Vector4(w, h, 0, 0)},
				new UIVertex(){ position = new Vector2(rect.xMax, rect.yMax), color = Color.white, uv0 = new Vector4(w, 0, 0, 0)},
			});
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();
			SetVerticesDirty();
		}
#endif
	}
}