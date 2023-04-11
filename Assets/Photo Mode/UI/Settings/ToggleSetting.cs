using System;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoMode.UI
{
	[AddComponentMenu("Photo Mode/Settings/Toggle Setting")]
	[RequireComponent(typeof(SettingEditor))]
	public class ToggleSetting : BaseSettingEditor<bool>
    {
        [SerializeField] private Toggle toggle;

        void Start()
        {
            toggle.onValueChanged.AddListener(value => SettingEditor.SetSettingValue<bool>(value, Setting.Value, true));
            Setting.OnChange += OnSettingChanged;
			toggle.SetIsOnWithoutNotify(Setting.Value);
		}

		protected void OnDestroy()
		{
			if (Setting != null)
				Setting.OnChange -= OnSettingChanged;
		}

		private void OnSettingChanged(PhotoModeSetting obj)
		{
			toggle.SetIsOnWithoutNotify(Setting.Value);
		}
	}
}
