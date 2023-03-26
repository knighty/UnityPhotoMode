using PhotoMode;
using UnityEngine;
using UnityEngine.UI;

public class HistogramGraphic : Graphic
{
	[SerializeField] Material histogramMaterial;
	[SerializeField] Histogram histogram;
	[SerializeField] HistogramType type;

	enum HistogramType
	{
		Color,
		Luminance
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		Rect rect = GetPixelAdjustedRect();
		vh.Clear();
		vh.AddUIVertexQuad(new UIVertex[]
		{
			new UIVertex(){ position = new Vector2(rect.xMin, rect.yMax), color = Color.white, uv0 = new Vector4(0, 0, 0, 0)},
			new UIVertex(){ position = new Vector2(rect.xMin, rect.yMin), color = Color.white, uv0 = new Vector4(0, 1, 0, 0)},
			new UIVertex(){ position = new Vector2(rect.xMax, rect.yMin), color = Color.white, uv0 = new Vector4(1, 1, 0, 0)},
			new UIVertex(){ position = new Vector2(rect.xMax, rect.yMax), color = Color.white, uv0 = new Vector4(1, 0, 0, 0)},
		});

		material = histogramMaterial;
	}

	void Update()
	{
		if (histogram == null)
			return;
		histogramMaterial.SetBuffer("histogramBuffer", histogram.HistogramBuffer);
		histogramMaterial.SetBuffer("histogramMaxBuffer", histogram.HistogramMaxBuffer);
		//histogramMaterial.SetFloat("_Normalization", (Screen.width * Screen.height) / 256.0f);
		//float normalize = (float)histogram.MaxValue;
		//histogramMaterial.SetFloat("_Normalization", normalize);
		if (type == HistogramType.Color)
		{
			histogramMaterial.DisableKeyword("HISTOGRAM_LUMINANCE");
			histogramMaterial.EnableKeyword("HISTOGRAM_COLOR");
		}
		if (type == HistogramType.Luminance)
		{
			histogramMaterial.DisableKeyword("HISTOGRAM_COLOR");
			histogramMaterial.EnableKeyword("HISTOGRAM_LUMINANCE");
		}
	}

#if UNITY_EDITOR
	protected override void OnValidate()
	{
		SetVerticesDirty();
	}
#endif
}
