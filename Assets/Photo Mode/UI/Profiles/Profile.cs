using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PhotoMode.UI
{
	[Serializable]
	public class Profile
	{
		[Serializable]
		public class ProfileSetting
		{
			public string name;
			public string value;

			public ProfileSetting(string name, string value)
			{
				this.name = name;
				this.value = value;
			}
		}

		[SerializeField] private string name;
		[SerializeField] private List<ProfileSetting> data;
		[SerializeField] private string category;

		public string Name { get => name; set => name = value; }
		public string Category { get => category; set => category = value; }

		public static Profile FromFile(string file)
		{
			string json = File.ReadAllText(file);
			Profile profile = JsonUtility.FromJson<Profile>(json);
			return profile;
		}

		public void SetData(PhotoModeSettings settings, string category = "")
		{
			this.category = category;
			data = settings.GetType().GetProperties()
				.Select(p => new { property = p, attr = p.GetCustomAttribute<CategoryAttribute>() })
				.Where(fa => fa.attr != null && fa.attr.saveable && (fa.attr.category == category || category == ""))
				.Select(fa => new ProfileSetting(fa.property.Name, JsonUtility.ToJson(fa.property.GetValue(settings))))
				.ToList();
		}

		public void ApplyToSettings(PhotoModeSettings settings)
		{
			foreach(var setting in data)
			{
				PropertyInfo property = settings.GetType().GetProperty(setting.name);
				if (property == null) continue;
				JsonUtility.FromJsonOverwrite(setting.value, property.GetValue(settings));
			}
		}
	}
}
