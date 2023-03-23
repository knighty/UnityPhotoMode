using System.Reflection;
using UnityEngine;

namespace PhotoMode.UI
{
	[RequireComponent(typeof(SettingEditor))]
	public class BaseSettingEditor<T> : MonoBehaviour
	{
		private SettingEditor settingEditor;
		protected SettingEditor SettingEditor { get => settingEditor ??= GetComponent<SettingEditor>(); }

		protected PhotoModeSetting<T> Setting { get => SettingEditor.GetSetting<T>(); }
		protected PropertyInfo PropertyInfo { get => SettingEditor.PropertyInfo; }
	}
}
