using UnityEngine;

namespace Drawing
{
	public class ArcRenderer
	{
		public void DrawCircle(ArcRendererHelper helper, float x, float y, float resolution, float outerRadius, float innerRadius = 0)
		{
			DrawArc(helper, x, y, resolution, outerRadius, innerRadius);
		}

		public void DrawArc(ArcRendererHelper helper, float x, float y, float resolution, float outerRadius, float innerRadius = 0, float startAngle = 0, float endAngle = Mathf.PI * 2)
		{
			helper.Begin();
			bool circle = innerRadius == 0;
			if (circle)
			{
				helper.AddVert(new Vector2(x, y));
			}
			for (int i = 0; i < resolution + 1; i++)
			{
				float theta = startAngle + (endAngle - startAngle) * (float)i / resolution;
				helper.AddVert(new Vector2(x + Mathf.Cos(theta) * outerRadius, y + Mathf.Sin(theta) * outerRadius));
				if (circle)
				{
					if (i < resolution)
					{
						helper.AddTriangle(1, i + 1, i + 2);
					}
				}
				else
				{
					helper.AddVert(new Vector2(x + Mathf.Cos(theta) * innerRadius, y + Mathf.Sin(theta) * innerRadius));
					if (i < resolution)
					{
						helper.AddTriangle(i * 2, i * 2 + 2, i * 2 + 1);
						helper.AddTriangle(i * 2 + 2, i * 2 + 3, i * 2 + 1);
					}
				}
			}
		}
	}
}