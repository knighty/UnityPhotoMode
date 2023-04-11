using System;

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
		public Option<FlyCamera.FlyControlMode> ControlMode { get; }
	}
}
