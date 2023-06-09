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
	private PhotoModeSetting<float> timeOfDay = new PhotoModeSetting<float>(0.3f, true);

	[SerializeField]
	private PhotoModeSetting<Environments> environment = new PhotoModeSetting<Environments>(Environments.Clear, true);

	[SerializeField]
	private PhotoModeSetting<bool> showCharacter = new PhotoModeSetting<bool>(true, true);

	[PhotoMode.Min(0), Max(1), Round(2), Overridable(true), Category(CATEGORY_WORLD)]
	public PhotoModeSetting<float> TimeOfDay { get => timeOfDay; }

	[Overridable(true), Category(CATEGORY_WORLD)]
	public PhotoModeSetting<Environments> Environment { get => environment; }

	[Overridable(true), Category(CATEGORY_WORLD)]
	public PhotoModeSetting<bool> ShowCharacter { get => showCharacter; }
}

/*[Serializable]
public class PhotoModeSettingEnvironments : PhotoModeSetting<Environments>
{
	[SerializeField] private Environments currentValue = Environments.Clear;
	[SerializeField] private bool overriding = false;

	public PhotoModeSettingEnvironments(Environments value, bool overriding = true) : base(value, overriding) { }

	protected override Environments ValueInternal { get => currentValue; set => currentValue = value; }
	protected override bool OverridingInternal { get => overriding; set => overriding = value; }
}
*/