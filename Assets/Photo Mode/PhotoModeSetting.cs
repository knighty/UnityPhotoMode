using System;
using UnityEngine;

namespace PhotoMode
{
	public interface PhotoModeSetting
	{
		public void Reset();
		public bool IsOverriding { get; set; }
		public Action<PhotoModeSetting> OnChange { get; set; }
	}

	[Serializable]
	public class PhotoModeSetting<T> : PhotoModeSetting
	{
		[SerializeField]
		private bool overriding;

		[SerializeField]
		private T currentValue;

		public T Value
		{
			get => currentValue;
			set
			{
				overriding = true;
				currentValue = value;
				OnChange?.Invoke(this);
			}
		}

		public bool IsOverriding
		{
			get => overriding;
			set
			{
				overriding = value;
				OnChange?.Invoke(this);
			}
		}

		public Action<PhotoModeSetting> OnChange { get; set; }

		public PhotoModeSetting(T value, bool overriding = true)
		{
			currentValue = value;
			this.overriding = overriding;
		}

		public void Reset()
		{
			overriding = false;
			OnChange?.Invoke(this);
		}

		public static implicit operator T(PhotoModeSetting<T> prop)
		{
			return prop.currentValue;
		}
	}
}