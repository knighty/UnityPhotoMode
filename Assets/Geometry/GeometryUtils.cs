using UnityEngine;

namespace Geometry
{
	internal class GeometryUtils
	{
		public static bool PointOnRightOfPlane(Vector2 a, Vector2 b, Vector2 point)
		{
			Vector2 plane = b - a;
			Vector2 normal = new Vector2(-plane.y, plane.x);
			return Vector2.Dot((point - a), normal) <= 0;
		}

		public static bool LinesIntersecting(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
		{
			float epsilon = 0.00001f;// float.MinValue;

			bool isIntersecting = false;

			float denominator = (b2.y - b1.y) * (a2.x - a1.x) - (b2.x - b1.x) * (a2.y - a1.y);

			//Make sure the denominator is != 0 (or the lines are parallel)
			if (denominator > 0f + epsilon || denominator < 0f - epsilon)
			{
				float u_a = ((b2.x - b1.x) * (a1.y - b1.y) - (b2.y - b1.y) * (a1.x - b1.x)) / denominator;
				float u_b = ((a2.x - a1.x) * (a1.y - b1.y) - (a2.y - a1.y) * (a1.x - b1.x)) / denominator;

				float zero = 0f + epsilon;
				float one = 1f - epsilon;

				if (u_a > zero && u_a < one && u_b > zero && u_b < one)
				{
					isIntersecting = true;
				}
			}

			return isIntersecting;
		}
	}
}
