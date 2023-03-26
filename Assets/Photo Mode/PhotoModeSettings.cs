using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace PhotoMode
{
	public enum PhotoModeQuality
	{
		Low,
		Medium,
		High,
		VeryHigh,
		Ultra
	}

	static class PhotoModeQualityExtensions
	{
		public static int Accumulations(this PhotoModeQuality e)
		{
			switch (e)
			{
				case PhotoModeQuality.Low:
					return 1;
				case PhotoModeQuality.Medium:
					return 128;
				case PhotoModeQuality.High:
					return 256;
				case PhotoModeQuality.VeryHigh:
					return 1024;
				case PhotoModeQuality.Ultra:
					return 2048;
			}
			return 0;
		}
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class MinAttribute : Attribute
	{
		public readonly float min;

		public MinAttribute(float min)
		{
			this.min = min;
		}
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class MaxAttribute : Attribute
	{
		public readonly float max;

		public MaxAttribute(float max)
		{
			this.max = max;
		}
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class RoundAttribute : Attribute
	{
		public readonly int places;

		public RoundAttribute(int places)
		{
			this.places = places;
		}
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class OverridableAttribute : Attribute
	{
		public readonly bool overridable;

		public OverridableAttribute(bool overridable)
		{
			this.overridable = overridable;
		}
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class CategoryAttribute : Attribute
	{
		public readonly string category;

		public CategoryAttribute(string category)
		{
			this.category = category;
		}
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class PhotoModeSettingAttribute : Attribute
	{
		public readonly string name;
		public readonly string description;

		public PhotoModeSettingAttribute(string name, string description = "")
		{
			this.name = name;
			this.description = description;
		}
	}

	[Serializable]
	public class TestClass
	{
		[SerializeField] public int ugh = 5;
	}

	[CreateAssetMenu(fileName = "Photo Mode Settings", menuName = "Photo Mode/Settings", order = 1)]
	public class PhotoModeSettings : ScriptableObject, ISerializationCallbackReceiver
	{
		const string CATEGORY_IMAGE = "Image";
		const string CATEGORY_CAMERA = "Camera";
		const string CATEGORY_COLOR = "Color";
		const string CATEGORY_LIGHTING = "Lighting";

		[SerializeField]
		private PhotoModeSettingFloat width = new PhotoModeSettingFloat(1920, false);

		[SerializeField]
		private PhotoModeSettingFloat height = new PhotoModeSettingFloat(1080, false);

		[SerializeField]
		private PhotoModeSettingFloat aperture = new PhotoModeSettingFloat(35);

		[SerializeField]
		private PhotoModeSettingFloat fov = new PhotoModeSettingFloat(60);

		[SerializeField]
		private PhotoModeSettingFloat focusDistance = new PhotoModeSettingFloat(1000);

		[SerializeField]
		private PhotoModeSettingApertureShape apertureShape = new PhotoModeSettingApertureShape(null);

		[SerializeField]
		private PhotoModeSettingVector2 lensShift = new PhotoModeSettingVector2(Vector2.zero);

		[SerializeField]
		private PhotoModeSettingVector2 lensTilt = new PhotoModeSettingVector2(Vector2.zero);

		[SerializeField]
		private PhotoModeSettingTonemapper tonemapper = new PhotoModeSettingTonemapper(UnityEngine.Rendering.PostProcessing.Tonemapper.ACES);

		[SerializeField]
		private PhotoModeSettingFloat exposure = new PhotoModeSettingFloat(0);

		[SerializeField]
		private PhotoModeSettingFloat colorTemperature = new PhotoModeSettingFloat(0);

		[SerializeField]
		private PhotoModeSettingFloat contrast = new PhotoModeSettingFloat(0);

		[SerializeField]
		private PhotoModeSettingFloat saturation = new PhotoModeSettingFloat(0);

		[SerializeField]
		private PhotoModeSettingFloat clarity = new PhotoModeSettingFloat(0);

		[SerializeField]
		private PhotoModeSettingFloat vibrance = new PhotoModeSettingFloat(0);

		[SerializeField]
		private PhotoModeSettingFloat bloomThreshold = new PhotoModeSettingFloat(0);

		[SerializeField]
		private PhotoModeSettingFloat bloomIntensity = new PhotoModeSettingFloat(0);

		[SerializeField]
		private PhotoModeSettingFloat chromaticAberration = new PhotoModeSettingFloat(0);

		[SerializeField]
		private PhotoModeSettingFloat vignette = new PhotoModeSettingFloat(0);

		[SerializeField]
		private PhotoModeSettingPhotoModeQuality quality = new PhotoModeSettingPhotoModeQuality(PhotoModeQuality.High);

		Action<PhotoModeSetting> onChange;

		public Action<PhotoModeSetting> OnChange
		{
			get => onChange;
			set
			{
				if (onChange == null)
					Settings.ForEach(setting => setting.OnChange += Changed);
				onChange = value;
				if (onChange == null)
					Settings.ForEach(setting => setting.OnChange -= Changed);
			}
		}

		public List<PhotoModeSetting> Settings
		{
			get
			{
				List<PhotoModeSetting> list = new List<PhotoModeSetting>();
				Type type = this.GetType();
				while (typeof(PhotoModeSettings).IsAssignableFrom(type))
				{
					FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
					foreach (FieldInfo field in fields)
					{
						if (typeof(PhotoModeSetting).IsAssignableFrom(field.FieldType))
						{
							list.Add((PhotoModeSetting)field.GetValue(this));
						}
					}
					type = type.BaseType;
				}
				return list;
			}
		}

		[PhotoModeSetting("Width")]
		[Category(CATEGORY_IMAGE), Min(1), Max(3840), Round(0), Overridable(true)]
		public PhotoModeSetting<float> Width { get => width; }

		[PhotoModeSetting("Height")]
		[Category(CATEGORY_IMAGE), Min(1), Max(2160), Round(0), Overridable(true)]
		public PhotoModeSetting<float> Height { get => height; }

		[PhotoModeSetting("Aperture Size")]
		[Category(CATEGORY_CAMERA), Min(0), Max(200), Round(0)]
		public PhotoModeSetting<float> Aperture { get => aperture; set => aperture.Value = value; }

		[PhotoModeSetting("Field Of View")]
		[Min(1), Max(160), Round(0), Category(CATEGORY_CAMERA)]
		public PhotoModeSetting<float> Fov { get => fov; }

		[PhotoModeSetting("Focus Distance")]
		[Min(0.2f), Max(100), Round(2), Category(CATEGORY_CAMERA)]
		public PhotoModeSetting<float> FocusDistance { get => focusDistance; }

		[PhotoModeSetting("Aperture Shape")]
		[Category(CATEGORY_CAMERA)]
		public PhotoModeSetting<ApertureShape> ApertureShape { get => apertureShape; }

		[PhotoModeSetting("Lens Shift")]
		[Category(CATEGORY_CAMERA), Min(-0.15f), Max(0.15f)]
		public PhotoModeSetting<Vector2> LensShift { get => lensShift; }

		[PhotoModeSetting("Lens Tilt")]
		[Category(CATEGORY_CAMERA), Min(-45), Max(45)]
		public PhotoModeSetting<Vector2> LensTilt { get => lensTilt; }

		[PhotoModeSetting("Tonemapper")]
		[Overridable(true), Category(CATEGORY_COLOR)]
		public PhotoModeSetting<Tonemapper> Tonemapper { get => tonemapper; }

		[PhotoModeSetting("Exposure")]
		[Min(-4), Max(4), Round(1), Overridable(true), Category(CATEGORY_COLOR)]
		public PhotoModeSetting<float> Exposure { get => exposure; }

		[PhotoModeSetting("Color Temperature")]
		[Min(-100), Max(100), Round(0), Overridable(true), Category(CATEGORY_COLOR)]
		public PhotoModeSetting<float> ColorTemperature { get => colorTemperature; }

		[PhotoModeSetting("Contrast")]
		[Min(-100), Max(100), Round(0), Overridable(true), Category(CATEGORY_COLOR)]
		public PhotoModeSetting<float> Contrast { get => contrast; }

		[PhotoModeSetting("Saturation")]
		[Min(-100), Max(100), Round(0), Overridable(true), Category(CATEGORY_COLOR)]
		public PhotoModeSetting<float> Saturation { get => saturation; }

		[PhotoModeSetting("Clarity")]
		[Min(-1), Max(3), Round(2), Overridable(true), Category(CATEGORY_COLOR)]
		public PhotoModeSetting<float> Clarity { get => clarity; }

		[PhotoModeSetting("Vibrance")]
		[Min(-1), Max(3), Round(2), Overridable(true), Category(CATEGORY_COLOR)]
		public PhotoModeSetting<float> Vibrance { get => vibrance; }

		[PhotoModeSetting("Bloom - Threshold")]
		[Min(0), Max(2), Round(2), Overridable(true), Category(CATEGORY_LIGHTING)]
		public PhotoModeSetting<float> BloomThreshold { get => bloomThreshold; }

		[PhotoModeSetting("Bloom - Intensity")]
		[Min(0), Max(2), Round(2), Overridable(true), Category(CATEGORY_LIGHTING)]
		public PhotoModeSetting<float> BloomIntensity { get => bloomIntensity; }

		[PhotoModeSetting("Chromatic Aberration")]
		[Min(0), Max(1), Round(2), Overridable(true), Category(CATEGORY_LIGHTING)]
		public PhotoModeSetting<float> ChromaticAberration { get => chromaticAberration; }

		[PhotoModeSetting("Vignette")]
		[Min(0), Max(1), Round(2), Overridable(true), Category(CATEGORY_LIGHTING)]
		public PhotoModeSetting<float> Vignette { get => vignette; }

		[PhotoModeSetting("Quality")]
		[Category(CATEGORY_IMAGE)]
		public PhotoModeSetting<PhotoModeQuality> Quality { get => quality; }

		public int Accumulations => Quality.Value.Accumulations();

		private void Changed(PhotoModeSetting setting)
		{
			OnChange?.Invoke(setting);
		}

		public void ApplyToCamera(Camera camera)
		{
			camera.lensShift = lensShift;
		}

		public void ApplyToVolume(PostProcessVolume volume)
		{
			if (volume == null)
				return;

			if (volume.profile.TryGetSettings(out ColorGrading colorGrading))
			{
				colorGrading.tonemapper.Override(tonemapper);
				colorGrading.tonemapper.overrideState = tonemapper.IsOverriding;

				colorGrading.postExposure.Override(exposure);
				colorGrading.postExposure.overrideState = exposure.IsOverriding;

				colorGrading.saturation.Override(saturation);
				colorGrading.saturation.overrideState = saturation.IsOverriding;

				colorGrading.contrast.Override(contrast);
				colorGrading.contrast.overrideState = contrast.IsOverriding;

				colorGrading.temperature.Override(colorTemperature);
				colorGrading.temperature.overrideState = colorTemperature.IsOverriding;
			}

			if (volume.profile.TryGetSettings(out Bloom bloom))
			{
				bloom.threshold.Override(bloomThreshold);
				bloom.threshold.overrideState = bloomThreshold.IsOverriding;

				bloom.intensity.Override(bloomIntensity);
				bloom.intensity.overrideState = bloomIntensity.IsOverriding;

				bloom.dirtIntensity.Override(0);
			}

			if (volume.profile.TryGetSettings(out ChromaticAberration ca))
			{
				ca.intensity.Override(chromaticAberration);
				ca.intensity.overrideState = chromaticAberration.IsOverriding;
			}

			if (volume.profile.TryGetSettings(out Vignette v))
			{
				v.intensity.Override(vignette);
				v.intensity.overrideState = vignette.IsOverriding;
			}
		}

		public PostProcessVolume InitVolume()
		{
			GameObject volumeObject = new GameObject();

			PostProcessVolume volume = volumeObject.AddComponent<PostProcessVolume>();
			volume.priority = 1000;
			volume.isGlobal = true;

			PostProcessProfile profile = ScriptableObject.CreateInstance<PostProcessProfile>();

			Bloom bloom = ScriptableObject.CreateInstance<Bloom>();
			bloom.enabled.value = true;
			profile.AddSettings(bloom);

			ColorGrading colorGrading = ScriptableObject.CreateInstance<ColorGrading>();
			colorGrading.enabled.value = true;
			profile.AddSettings(colorGrading);

			ChromaticAberration chromaticAberration = ScriptableObject.CreateInstance<ChromaticAberration>();
			chromaticAberration.enabled.value = true;
			profile.AddSettings(chromaticAberration);

			AccumulationSettings accumulationSettings = ScriptableObject.CreateInstance<AccumulationSettings>();
			profile.AddSettings(accumulationSettings);

			Vignette vignette = ScriptableObject.CreateInstance<Vignette>();
			vignette.enabled.value = true;
			profile.AddSettings(vignette);

			volume.profile = profile;

			return volume;
		}

		[SerializeField] List<string> jsonSerialize;
		public void OnBeforeSerialize()
		{
			jsonSerialize.Clear();
			List<string> strings = new List<string>();
			FieldInfo[] fields = typeof(PhotoModeSettings).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (typeof(PhotoModeSetting).IsAssignableFrom(fieldInfo.FieldType))
				{
					strings.Add(fieldInfo.Name);
					strings.Add(JsonUtility.ToJson(fieldInfo.GetValue(this)));
				}
			}
			jsonSerialize = strings;
		}

		public void OnAfterDeserialize()
		{
			if (Application.isEditor)
				return;

			for (int i = 0; i < jsonSerialize.Count; i++)
			{
				string name = jsonSerialize[i];
				string json = jsonSerialize[++i];
				FieldInfo field = typeof(PhotoModeSettings).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
				JsonUtility.FromJsonOverwrite(json, field.GetValue(this));
			}
			jsonSerialize.Clear();
		}
	}
}