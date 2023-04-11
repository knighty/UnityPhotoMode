using PhotoMode;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoMode.UI
{
	public class SettingEditor : MonoBehaviour
	{
		[SerializeField] string labelText;
		[SerializeField] Text label;
		[SerializeField] Toggle overrideToggle;
		Command lastSetCommand = null;

		PhotoModeSetting setting;
		PropertyInfo propertyInfo;

		public string Label { get => label.text; set => label.text = value; }

		public virtual PhotoModeSetting Setting
		{
			get => setting;
			set
			{
				setting = value;
				setting.OnChange += OnSettingChanged;
				overrideToggle.SetIsOnWithoutNotify(setting.IsOverriding);
			}
		}

		public virtual PropertyInfo PropertyInfo
		{
			get => propertyInfo;
			set
			{
				propertyInfo = value;
				OverridableAttribute attr = value.GetCustomAttribute<OverridableAttribute>();
				bool showToggle = false;
				if (attr != null)
				{
					showToggle = attr.overridable;
				}
				overrideToggle.gameObject.SetActive(showToggle);
			}
		}

		public PhotoModeSetting<T> GetSetting<T>()
		{
			return setting as PhotoModeSetting<T>;
		}

		public void SetSettingValue<T>(T newValue, T oldValue, bool forceNewCommand = false)
		{
			SetPhotoModeSettingValue<T> command = new SetPhotoModeSettingValue<T>(GetSetting<T>(), newValue, oldValue);
			CommandList.Instance.Add(command);
		}

		private void OnSettingChanged(PhotoModeSetting setting)
		{
			overrideToggle.SetIsOnWithoutNotify(setting.IsOverriding);
		}

		private void Start()
		{
			overrideToggle.onValueChanged.AddListener(value => setting.IsOverriding = value);
		}

		protected virtual void OnDestroy()
		{
			if (setting != null)
				setting.OnChange -= OnSettingChanged;
		}
	}
}