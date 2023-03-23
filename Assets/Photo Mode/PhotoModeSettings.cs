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

	[Serializable]
	[CreateAssetMenu(fileName = "Photo Mode Settings", menuName = "Photo Mode/Settings", order = 1)]
	public class PhotoModeSettings : ScriptableObject
	{
		const string CATEGORY_CAMERA = "Camera";
		const string CATEGORY_COLOR = "Color";
		const string CATEGORY_LIGHTING = "Lighting";

		[SerializeField]
		private PhotoModeSetting<float> width = new PhotoModeSetting<float>(1920, false);

		[SerializeField]
		private PhotoModeSetting<float> height = new PhotoModeSetting<float>(1080, false);

		[SerializeField]
		private PhotoModeSetting<float> aperture = new PhotoModeSetting<float>(35);

		[SerializeField]
		private PhotoModeSetting<float> fov = new PhotoModeSetting<float>(60);

		[SerializeField]
		private PhotoModeSetting<float> focusDistance = new PhotoModeSetting<float>(1000);

		[SerializeField]
		private PhotoModeSetting<ApertureShape> apertureShape = new PhotoModeSetting<ApertureShape>(null);

		[SerializeField]
		private PhotoModeSetting<Vector2> lensShift = new PhotoModeSetting<Vector2>(Vector2.zero);

		[SerializeField]
		private PhotoModeSetting<Tonemapper> tonemapper = new PhotoModeSetting<Tonemapper>(UnityEngine.Rendering.PostProcessing.Tonemapper.ACES);

		[SerializeField]
		private PhotoModeSetting<float> exposure = new PhotoModeSetting<float>(0);

		[SerializeField]
		private PhotoModeSetting<float> colorTemperature = new PhotoModeSetting<float>(0);

		[SerializeField]
		private PhotoModeSetting<float> contrast = new PhotoModeSetting<float>(0);

		[SerializeField]
		private PhotoModeSetting<float> saturation = new PhotoModeSetting<float>(0);

		[SerializeField]
		private PhotoModeSetting<float> bloomThreshold = new PhotoModeSetting<float>(0);

		[SerializeField]
		private PhotoModeSetting<float> bloomIntensity = new PhotoModeSetting<float>(0);

		[SerializeField]
		private PhotoModeSetting<float> chromaticAberration = new PhotoModeSetting<float>(0);

		[SerializeField]
		private PhotoModeSetting<float> vignette = new PhotoModeSetting<float>(0);

		[SerializeField]
		private PhotoModeSetting<PhotoModeQuality> quality = new PhotoModeSetting<PhotoModeQuality>(PhotoModeQuality.High);

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

		private void Changed(PhotoModeSetting setting)
		{
			OnChange?.Invoke(setting);
		}

		[Category(CATEGORY_CAMERA), Min(1), Max(3840), Round(0), Overridable(true)]
		public PhotoModeSetting<float> Width { get => width; }

		[Category(CATEGORY_CAMERA), Min(1), Max(2160), Round(0), Overridable(true)]
		public PhotoModeSetting<float> Height { get => height; }

		[Category(CATEGORY_CAMERA), Min(0), Max(200), Round(0)]
		public PhotoModeSetting<float> Aperture { get => aperture; set => aperture.Value = value; }

		[Min(1), Max(160), Round(0), Category(CATEGORY_CAMERA)]
		public PhotoModeSetting<float> Fov { get => fov; }

		[Min(0.2f), Max(100), Round(2), Category(CATEGORY_CAMERA)]
		public PhotoModeSetting<float> FocusDistance { get => focusDistance; }

		[Category(CATEGORY_CAMERA)]
		public PhotoModeSetting<ApertureShape> ApertureShape { get => apertureShape; }

		[Category(CATEGORY_CAMERA)]
		public PhotoModeSetting<Vector2> LensShift { get => lensShift; }

		[Overridable(true), Category(CATEGORY_COLOR)]
		public PhotoModeSetting<Tonemapper> Tonemapper { get => tonemapper; }

		[Min(-4), Max(4), Round(1), Overridable(true), Category(CATEGORY_COLOR)]
		public PhotoModeSetting<float> Exposure { get => exposure; }

		[Min(-100), Max(100), Round(0), Overridable(true), Category(CATEGORY_COLOR)]
		public PhotoModeSetting<float> ColorTemperature { get => colorTemperature; }

		[Min(-100), Max(100), Round(0), Overridable(true), Category(CATEGORY_COLOR)]
		public PhotoModeSetting<float> Contrast { get => contrast; }

		[Min(-100), Max(100), Round(0), Overridable(true), Category(CATEGORY_COLOR)]
		public PhotoModeSetting<float> Saturation { get => saturation; }

		[Min(0), Max(2), Round(2), Overridable(true), Category(CATEGORY_LIGHTING)]
		public PhotoModeSetting<float> BloomThreshold { get => bloomThreshold; }

		[Min(0), Max(2), Round(2), Overridable(true), Category(CATEGORY_LIGHTING)]
		public PhotoModeSetting<float> BloomIntensity { get => bloomIntensity; }

		[Min(0), Max(1), Round(2), Overridable(true), Category(CATEGORY_LIGHTING)]
		public PhotoModeSetting<float> ChromaticAberration { get => chromaticAberration; }

		[Min(0), Max(1), Round(2), Overridable(true), Category(CATEGORY_LIGHTING)]
		public PhotoModeSetting<float> Vignette { get => vignette; }

		[Category(CATEGORY_CAMERA)]
		public PhotoModeSetting<PhotoModeQuality> Quality { get => quality; }

		public int Accumulations => Quality.Value.Accumulations();

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

		public virtual void Apply()
		{

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
	}
}