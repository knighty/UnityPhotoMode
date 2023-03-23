using PhotoMode;
using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Valheim Photo Mode Settings", menuName = "Photo Mode/Valheim Settings", order = 1)]
public class ValheimPhotoModeSettings : PhotoModeSettings
{
	//[SerializeField] Transform light;

	const string CATEGORY_WORLD = "World";

	[SerializeField]
	private PhotoModeSetting<float> timeOfDay;

	[SerializeField]
	private PhotoModeSetting<Environments> environment;

	[PhotoMode.Min(0), Max(1), Round(2), Overridable(true), Category(CATEGORY_WORLD)]
	public PhotoModeSetting<float> TimeOfDay { get => timeOfDay; }

	[Overridable(true), Category(CATEGORY_WORLD)]
	public PhotoModeSetting<Environments> Environment { get => environment; }

	public override void Apply()
	{
		base.Apply();

		//light.rotation = Quaternion.Euler(0, 0, timeOfDay.Value * Mathf.PI * 2);
	}
}
