using System;
using System.Reflection;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoMode.UI
{
	[RequireComponent(typeof(SettingEditor))]
	public class SliderSetting : BaseSettingEditor<float>
	{
		[SerializeField]
		Slider slider;

		[SerializeField]
		InputField input;

		[SerializeField]
		int min = 0;

		[SerializeField]
		int max = 100;

		int decimalPlaces = 0;
		bool ignoreSettingCallback = false;
		float currentValue = 0;

		public int Max { set => slider.maxValue = value; }
		public int Min { set => slider.minValue = value; }
		public int DecimalPlaces
		{
			set
			{
				decimalPlaces = value;
				slider.wholeNumbers = decimalPlaces == 0;
			}
		}

		void OnSettingChanged(PhotoModeSetting s)
		{
			if (ignoreSettingCallback)
				return;
			currentValue = Setting.Value;
			slider.SetValueWithoutNotify(Setting.Value);
			input.SetTextWithoutNotify(ProcessNum(Setting.Value));
		}

		string ProcessNum(float num)
		{
			return string.Format($"{{0:F{decimalPlaces}}}", num);
		}

		private void Start()
		{
			slider.minValue = min;
			slider.maxValue = max;

			MinAttribute minAttribute = PropertyInfo.GetCustomAttribute<MinAttribute>();
			if (minAttribute != null)
				Min = (int)minAttribute.min;

			MaxAttribute maxAttribute = PropertyInfo.GetCustomAttribute<MaxAttribute>();
			if (maxAttribute != null)
				Max = (int)maxAttribute.max;

			RoundAttribute roundAttribute = PropertyInfo.GetCustomAttribute<RoundAttribute>();
			if (roundAttribute != null)
				DecimalPlaces = roundAttribute.places;

			slider.onValueChanged.AddListener(num =>
			{
				ignoreSettingCallback = true;
				input.SetTextWithoutNotify(ProcessNum(num));
				UpdateSetting(num);
				currentValue = num;
				ignoreSettingCallback = false;
			});

			input.onValueChanged.AddListener(text =>
			{
				ignoreSettingCallback = true;
				float num = text == "" ? 0 : float.Parse(text);
				slider.SetValueWithoutNotify(text == "" ? 0 : num);
				UpdateSetting(num);
				currentValue = num;
				ignoreSettingCallback = false;
			});
			input.onEndEdit.AddListener(text =>
			{
				ignoreSettingCallback = true;
				float num = text == "" ? 0 : float.Parse(text.ToString());
				slider.SetValueWithoutNotify(num);
				if (text == "")
					input.SetTextWithoutNotify("0");
				UpdateSetting(num);

				currentValue = slider.value;
				ignoreSettingCallback = false;
			});

			currentValue = Setting.Value;
			Setting.OnChange += OnSettingChanged;
			OnSettingChanged(Setting);
		}

		void UpdateSetting(float value)
		{
			if (Setting != null)
				SettingEditor.SetSettingValue<float>(value, Setting.Value, true);
			/*if (Setting != null)
				Setting.Value = value;*/
		}

		protected void OnDestroy()
		{
			if (Setting != null)
				Setting.OnChange -= OnSettingChanged;
		}
	}
}