using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Runtime.InteropServices;

namespace PhotoMode.UI
{
	[AddComponentMenu("Photo Mode/Settings/Drop Down Setting")]
	public class DropDownSetting : BaseSettingEditor<object>
	{
		[SerializeField] Dropdown dropdown;

		PropertyInfo propertyInfo;
		Type type;
		int currentValue;
		string[] enumNames = null;

		private void Start()
		{
			type = PropertyInfo.PropertyType.GenericTypeArguments[0];
			List<OptionSelector.Option> options = new List<OptionSelector.Option>();
			int i = 0;
			enumNames = Enum.GetNames(PropertyInfo.PropertyType.GenericTypeArguments[0]);
			dropdown.ClearOptions();
			dropdown.AddOptions(enumNames.Select(v => new Dropdown.OptionData() { text = v }).ToList());

			Type t = typeof(PhotoModeSetting<>).MakeGenericType(type);
			PropertyInfo p = t.GetProperty(nameof(PhotoModeSetting<object>.Value));
			propertyInfo = p;
			dropdown.onValueChanged.AddListener(value => SetValue(value, (int)p.GetValue(SettingEditor.Setting)));
			dropdown.SetValueWithoutNotify((int)p.GetValue(SettingEditor.Setting));

			if (SettingEditor.Setting != null)
				SettingEditor.Setting.OnChange += OnSettingChanged;
		}

		private void OnSettingChanged(PhotoModeSetting setting)
		{
			dropdown.SetValueWithoutNotify((int)propertyInfo.GetValue(SettingEditor.Setting));
		}

		protected int EnumNameToValue(string name)
		{
			for (int i = 0; i < enumNames.Length; i++)
			{
				if (enumNames[i] == name)
					return i;
			}
			return 0;
		}

		protected void SetValue(int value, int old)
		{
			MethodInfo method = typeof(SettingEditor).GetMethod(nameof(SettingEditor.SetSettingValue));
			MethodInfo generic = method.MakeGenericMethod(type);
			generic.Invoke(SettingEditor, new object[] { value, old, false });
		}

		private void OnDestroy()
		{
			if (SettingEditor.Setting != null)
				SettingEditor.Setting.OnChange -= OnSettingChanged;
		}
	}
}
