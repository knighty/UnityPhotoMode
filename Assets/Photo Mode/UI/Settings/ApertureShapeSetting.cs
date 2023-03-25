using UnityEngine;
using UnityEngine.UI;

namespace PhotoMode.UI
{
	[AddComponentMenu("Photo Mode/Settings/Aperture Shape Setting")]
	public class ApertureShapeSetting : BaseSettingEditor<ApertureShape>
	{
		[SerializeField] RawImage previewImage;
		[SerializeField] ApertureShapeDropdown dropdown;

		Texture2D texture;

		private void OnSettingChanged(PhotoModeSetting s)
		{
			UpdateTexture(Setting.Value);
		}

		private void UpdateTexture(ApertureShape shape)
		{
			shape.RenderPreview(texture, 4);
			previewImage.texture = texture;
		}

		private void Start()
		{
			dropdown.SetSelected(Setting.Value);
			texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
			dropdown.OnChange += (shape) => {
				UpdateTexture(shape);
				Setting.Value = shape;
			};
			UpdateTexture(Setting.Value);
		}

		protected void OnDestroy()
		{
			//base.OnDestroy();
			//setting.OnChange -= OnSettingChanged;
		}
	}
}