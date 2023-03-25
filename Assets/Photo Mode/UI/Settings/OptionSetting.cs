using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;

namespace PhotoMode.UI
{
	[AddComponentMenu("Photo Mode/Settings/Option Setting")]
	[RequireComponent(typeof(SettingEditor))]
	public class OptionSetting : BaseSettingEditor<object>
	{
		[SerializeField] OptionSelector optionSelector;

		PropertyInfo propertyInfo;
		Type type;
		int currentValue;

		private void Start()
		{
			type = PropertyInfo.PropertyType.GenericTypeArguments[0];
			List<OptionSelector.Option> options = new List<OptionSelector.Option>();
			int i = 0;
			optionSelector.Options = Enum.GetNames(PropertyInfo.PropertyType.GenericTypeArguments[0]).Select(v => new OptionSelector.Option() { Id = i++, Name = v }).ToList();

			Type t = typeof(PhotoModeSetting<>).MakeGenericType(type);
			PropertyInfo p = t.GetProperty(nameof(PhotoModeSetting<object>.Value));
			propertyInfo = p;
			optionSelector.OnSelected.AddListener(option => SetValue(option.Id, (int)p.GetValue(SettingEditor.Setting)));
			optionSelector.SetSelected((int)p.GetValue(SettingEditor.Setting), false, false);

			if (SettingEditor.Setting != null)
				SettingEditor.Setting.OnChange += OnSettingChanged;
		}

		protected void SetValue(int value, int old)
		{
			MethodInfo method = typeof(SettingEditor).GetMethod(nameof(SettingEditor.SetSettingValue));
			MethodInfo generic = method.MakeGenericMethod(type);
			generic.Invoke(SettingEditor, new object[] { value, old, false });
		}

		private void OnSettingChanged(PhotoModeSetting setting)
		{
			optionSelector.SetSelected((int)propertyInfo.GetValue(SettingEditor.Setting), false);
		}

		private void OnDestroy()
		{
			if (SettingEditor.Setting != null)
				SettingEditor.Setting.OnChange -= OnSettingChanged;
		}
	}
}