using PhotoMode;
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace PhotoMode
{
	public interface PhotoModeSetting
	{
		public void Reset();
		public bool IsOverriding { get; set; }
		public Action<PhotoModeSetting> OnChange { get; set; }
	}

	[Serializable]
	public class PhotoModeSetting<T> : PhotoModeSetting, ISerializationCallbackReceiver
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

		public PhotoModeSetting() { }

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

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			OnChange?.Invoke(this);
		}

		public static implicit operator T(PhotoModeSetting<T> prop)
		{
			return prop.currentValue;
		}

		/*protected abstract T ValueInternal { get; set; }
		protected abstract bool OverridingInternal { get; set; }*/
	}

	[Serializable]
	public class PhotoModeSettingFloat : PhotoModeSetting<float> { }
	/*{
		[SerializeField] private float currentValue = 0;
		[SerializeField] private bool overriding = false;

		public PhotoModeSettingFloat(float value, bool overriding = true) : base(value, overriding) { }

		protected override float ValueInternal { get => currentValue; set => currentValue = value; }
		protected override bool OverridingInternal { get => overriding; set => overriding = value; }
	}*/

	[Serializable]
	public class PhotoModeSettingApertureShape : PhotoModeSetting<ApertureShape> { }
	/*{
		[SerializeField] private ApertureShape currentValue = null;
		[SerializeField] private bool overriding = false;

		public PhotoModeSettingApertureShape(ApertureShape value, bool overriding = true) : base(value, overriding) { }

		protected override ApertureShape ValueInternal { get => currentValue; set => currentValue = value; }
		protected override bool OverridingInternal { get => overriding; set => overriding = value; }
	}*/

	[Serializable]
	public class PhotoModeSettingVector2 : PhotoModeSetting<Vector2> { }
	/*{
		[SerializeField] private Vector2 currentValue = Vector2.zero;
		[SerializeField] private bool overriding = false;

		public PhotoModeSettingVector2(Vector2 value, bool overriding = true) : base(value, overriding) { }

		protected override Vector2 ValueInternal { get => currentValue; set => currentValue = value; }
		protected override bool OverridingInternal { get => overriding; set => overriding = value; }
	}*/

	public enum PhotoModeTonemapper
	{
		None,
		Neutral,
		ACES
	}

	[Serializable]
	public class PhotoModeSettingTonemapper : PhotoModeSetting<PhotoModeTonemapper> { }
	/*{
		[SerializeField] private PhotoModeTonemapper currentValue = PhotoModeTonemapper.ACES;
		[SerializeField] private bool overriding = false;

		public PhotoModeSettingTonemapper(PhotoModeTonemapper value, bool overriding = true) : base(value, overriding) { }

		protected override PhotoModeTonemapper ValueInternal { get => currentValue; set => currentValue = value; }
		protected override bool OverridingInternal { get => overriding; set => overriding = value; }
	}*/

	[Serializable]
	public class PhotoModeSettingPhotoModeQuality : PhotoModeSetting<PhotoModeQuality> { }
	/*{
		[SerializeField] private PhotoModeQuality currentValue = PhotoModeQuality.High;
		[SerializeField] private bool overriding = false;

		public PhotoModeSettingPhotoModeQuality(PhotoModeQuality value, bool overriding = true) : base(value, overriding) { }

		protected override PhotoModeQuality ValueInternal { get => currentValue; set => currentValue = value; }
		protected override bool OverridingInternal { get => overriding; set => overriding = value; }
	}*/
}