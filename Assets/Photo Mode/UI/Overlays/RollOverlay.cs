using Drawing;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoMode.UI.Overlays
{
	[AddComponentMenu("Photo Mode/Overlays/Roll")]
	[RequireComponent(typeof(CanvasRenderer))]
	public class RollOverlay : Graphic
	{
		[SerializeField, Range(1, 10)] float lineWeight = 2;

		float roll = 0;
		float lastRoll = 0;
		new Camera camera;

		public Camera Camera
		{
			get => camera;
			set => camera = value;
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();

			if (Mathf.Abs(roll) < 0.001f)
				return;

			Rect rect = GetPixelAdjustedRect();
			float size = rect.width / 2;

			DrawingCanvas Canvas = new DrawingCanvas(vh);

			Canvas.FillPaint = new FillPaint() { Color = color };
			Canvas.DrawArc(0, 0, 32, size, size - lineWeight, 0, -roll);
			Canvas.DrawArc(0, 0, 32, size, size - lineWeight, 0 + Mathf.PI, -roll + Mathf.PI);
		}

		protected override void OnValidate()
		{
			SetVerticesDirty();
		}

		private void Update()
		{
			if (Camera == null)
				return;
			Vector3 eulerAngles = Camera.transform.eulerAngles;
			roll = eulerAngles.z * Mathf.Deg2Rad;
			if (roll > Mathf.PI) roll = roll - Mathf.PI * 2;
			if (roll != lastRoll)
			{
				SetVerticesDirty();
				lastRoll = roll;
			}
		}
	}
}