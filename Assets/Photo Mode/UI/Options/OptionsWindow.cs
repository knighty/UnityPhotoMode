using System;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoMode.UI
{
	public class OptionsWindow : MonoBehaviour
	{
		[SerializeField] InputField screenshotFolderInputField;
		[SerializeField] Button screenshotFolderButton;
		[SerializeField] Slider cameraSpeedSlider;
		[SerializeField] Toggle jpgToggle;
		[SerializeField] Toggle pngToggle;
		[SerializeField] Toggle clipboardToggle;
		[SerializeField] Dropdown controlModeDropdown;
		[SerializeField] ScriptableOptions scriptableOptions;

		Options options;
		EventSubscriptions subscriptions = new EventSubscriptions();

		public Options Options
		{
			get => options;
			set
			{
				options = value;

				subscriptions.Unsubscribe();
				InitInputField(screenshotFolderInputField, options.ScreenshotFolder);
				InitSlider(cameraSpeedSlider, options.CameraSpeed);
				InitToggle(jpgToggle, options.SaveJPG);
				InitToggle(pngToggle, options.SavePNG);
				InitToggle(clipboardToggle, options.SaveClipboard);

				subscriptions.Subscribe(options.ControlMode.OnChange, value => controlModeDropdown.SetValueWithoutNotify((int)value));
				subscriptions.Subscribe(controlModeDropdown.onValueChanged, value => options.ControlMode.Value = (FlyCamera.FlyControlMode)value);
				controlModeDropdown.SetValueWithoutNotify((int)options.ControlMode.Value);
			}
		}

		private void Start()
		{
			if (options == null && scriptableOptions != null)
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
