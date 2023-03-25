using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoMode.UI
{
	[AddComponentMenu("Photo Mode/Settings/Lens Shift Setting")]
	public class LensShiftSetting : BaseSettingEditor<Vector2>
	{
		[SerializeField] Slider horizontalSlider;
		[SerializeField] Slider verticalSlider;

		private void Awake()
		{
			horizontalSlider.onValueChanged.AddListener(value => SettingEditor.SetSettingValue(new Vector2(value, Setting.Value.y), Setting.Value));
			verticalSlider.onValueChanged.AddListener(value => SettingEditor.SetSettingValue(new Vector2(Setting.Value.x, value), Setting.Value));
		}

		private void Start()
		{
			if (PropertyInfo.GetCustomAttribute<MinAttribute>() is MinAttribute min && min != null)
			{
				horizontalSlider.minValue = min.min;
				verticalSlider.minValue = min.min;
			}

			if (PropertyInfo.GetCustomAttribute<MaxAttribute>() is MaxAttribute max && max != null)
			{
				horizontalSlider.maxValue = max.max;
				verticalSlider.maxValue = max.max;
			}

			horizontalSlider.value = Setting.Value.x;
			verticalSlider.value = Setting.Value.y;

			Setting.OnChange += OnSettingChanged;
		}

		private void OnSettingChanged(PhotoModeSetting setting)
		{
			horizontalSlider.SetValueWithoutNotify(Setting.Value.x);
			verticalSlider.SetValueWithoutNotify(Setting.Value.y);
		}

		protected void OnDestroy()
		{
			Setting.OnChange -= OnSettingChanged;
		}
	}
}