using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhotoMode.UI
{
	[CreateAssetMenu(menuName = "Photo Mode/Options")]
	public class ScriptableOptions : ScriptableObject, Options
	{
		[Serializable]
		public class OptionData<T> : Option<T>
		{
			[SerializeField] private T value;

			public Action<T> OnChange { get; set; }
			public T Value { get => value; set => this.value = value; }

			public OptionData(T v)
			{
				value = v;
			}
		}

		[SerializeField] OptionData<string> screenshotFolder = new OptionData<string>("");
		[SerializeField] OptionData<float> cameraSpeed = new OptionData<float>(1);
		[SerializeField] OptionData<bool> saveJPG = new OptionData<bool>(true);
		[SerializeField] OptionData<bool> savePNG = new OptionData<bool>(false);
		[SerializeField] OptionData<bool> saveClipboard = new OptionData<bool>(false);

		public Option<string> ScreenshotFolder { get => screenshotFolder; }
		public Option<float> CameraSpeed { get => cameraSpeed; }
		public Option<bool> SaveJPG { get => saveJPG ; }
		public Option<bool> SavePNG { get => savePNG ; }
		public Option<bool> SaveClipboard { get => saveClipboard ; }
	}
}
