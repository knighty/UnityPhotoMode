using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PhotoMode.UI
{
	public interface Option<T>
	{
		Action<T> OnChange { get; set; }
		T Value { get; set; }
	}

	public interface Options
	{
		public Option<string> ScreenshotFolder { get; }
		public Option<float> CameraSpeed { get; }
		public Option<bool> SaveJPG { get; }
		public Option<bool> SavePNG { get; }
		public Option<bool> SaveClipboard { get; }
	}

	public class OptionsWindow : MonoBehaviour
	{
		[SerializeField] InputField screenshotFolderInputField;
		[SerializeField] UnityEngine.UI.Button screenshotFolderButton;
		[SerializeField] Slider cameraSpeedSlider;
		[SerializeField] Toggle jpgToggle;
		[SerializeField] Toggle pngToggle;
		[SerializeField] Toggle clipboardToggle;
		[SerializeField] ScriptableOptions scriptableOptions;

		Options options;
		EventSubscriptions subscriptions = new EventSubscriptions();

		public Options Options
		{
			get => options; set
			{
				options = value;

				subscriptions.Unsubscribe();
				InitInputField(screenshotFolderInputField, options.ScreenshotFolder);
				InitSlider(cameraSpeedSlider, options.CameraSpeed);
				InitToggle(jpgToggle, options.SaveJPG);
				InitToggle(pngToggle, options.SavePNG);
				InitToggle(clipboardToggle, options.SaveClipboard);
			}
		}

		private void Start()
		{
			if (scriptableOptions != null)
			{
				Options = scriptableOptions;
			}

			screenshotFolderButton.onClick.AddListener(() =>
			{
				//System.Windows.Forms.OpenFileDialog d = new System.Windows.Forms.OpenFileDialog();
				//d.ShowDialog();
			});
		}

		private void InitInputField(InputField inputField, Option<string> option)
		{
			subscriptions.Subscribe(option.OnChange, value => inputField.SetTextWithoutNotify(value));
			subscriptions.Subscribe(inputField.onEndEdit, value => option.Value = value);
			inputField.SetTextWithoutNotify(option.Value);
		}

		private void InitSlider(Slider slider, Option<float> option)
		{
			subscriptions.Subscribe(option.OnChange, value => slider.SetValueWithoutNotify(value));
			subscriptions.Subscribe(slider.onValueChanged,value => option.Value = value); 
			slider.SetValueWithoutNotify(option.Value);
		}

		private void InitToggle(Toggle toggle, Option<bool> option)
		{
			subscriptions.Subscribe(option.OnChange, value => toggle.SetIsOnWithoutNotify(value));
			subscriptions.Subscribe(toggle.onValueChanged, value => option.Value = value);
			toggle.SetIsOnWithoutNotify(option.Value);
		}

		private void OnDestroy()
		{
			subscriptions.Unsubscribe();
		}
	}
}
