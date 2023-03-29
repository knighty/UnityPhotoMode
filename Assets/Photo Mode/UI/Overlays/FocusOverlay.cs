using UnityEngine;
using static UnityEngine.EventSystems.PointerEventData;
using UnityEngine.EventSystems;

namespace PhotoMode.UI
{
	public class FocusOverlay : MonoBehaviour, IPointerClickHandler
	{
		Camera camera;
		PhotoModeSettings settings;
		[SerializeField] Material sampleDepth;

		RenderTexture sampleDepthTexture;
		RayCastMode rayCastMode = RayCastMode.GPU;

		public Camera Camera { get => camera; set => camera = value; }
		public PhotoModeSettings Settings { get => settings; set => settings = value; }

		enum RayCastMode
		{
			CPU,
			GPU
		}

		void Awake()
		{
			sampleDepthTexture = new RenderTexture(1, 1, 0, RenderTextureFormat.RFloat);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != InputButton.Left)
				return;

			float distance = 0;
			switch (rayCastMode)
			{
				case RayCastMode.CPU:
					Vector3 rayDirection = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 1)) - Camera.main.transform.position;
					rayDirection.Normalize();

					if (Physics.Raycast(new Ray(Camera.main.transform.position, rayDirection), out RaycastHit hitInfo))
					{
						distance = hitInfo.distance;
						distance = Vector3.Dot(Camera.main.transform.forward, rayDirection * distance);
					}
					break;

				case RayCastMode.GPU:
					//Debug.Log(eventData.position);
					Vector4 mousePos = new Vector4(eventData.position.x / (float)Screen.width, eventData.position.y / (float)Screen.height, 0, 0);
					//Debug.Log($"Mouse Position: {mousePos}");
					sampleDepth.SetVector("_MousePosition", mousePos);
					Graphics.Blit(null, sampleDepthTexture, sampleDepth);
					Texture2D tex = new Texture2D(1, 1, TextureFormat.RFloat, false, true);
					tex.ReadPixels(new Rect(0, 0, 1, 1), 0, 0);
					tex.Apply();
					float depth = tex.GetPixel(0, 0).r;
					//Debug.Log($"Depth: {depth}");

					distance = depth;
					break;
			}

			if (distance != 0)
			{
				settings.FocusDistance.Value = distance;
			}
		}
	}
}
