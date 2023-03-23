using System;
using UnityEngine;

class EnvironmentAttribute : Attribute
{
	string name;

	public EnvironmentAttribute(string name)
	{
		this.name = name;
	}

	public string Name { get => name; }
}

public enum Environments
{
	[Environment("None")] None,
	[Environment("Clear")] Clear,
	[Environment("Twilight - Clear")] Twilight_Clear,
	[Environment("Plains - Clear")] Heath_clear,
	[Environment("Misty")] Misty,
	[Environment("Black Forest - Mist")] DeepForest_Mist,
	[Environment("Darklands - Dark")] Darklands_dark,
	[Environment("No Fog")] nofogts,
	[Environment("Light Rain")] LightRain,
	[Environment("Rain")] Rain,
	[Environment("Thunderstorm")] ThunderStorm,
	[Environment("Swamp - Rain")] SwampRain,
	[Environment("Eikthyr")] Eikthyr,
	[Environment("Bonemass")] Bonemass,
	[Environment("Moder")] Moder,
	[Environment("Golin King")] GoblinKing,
	[Environment("GDKing")] GDKing,
	[Environment("Snow")] Snow,
	[Environment("Snowstorm")] SnowStorm,
	[Environment("Twilight - Snow")] Twilight_Snow,
	[Environment("Twilight - Snow Storm")] Twilight_SnowStorm,
	[Environment("Ashrain")] Ashrain,
	[Environment("Crypt")] Crypt,
	[Environment("Sunken Crypt")] SunkenCrypt,
}

public static class EnvironmentsExtensions
{
	public static string ID(this Environments e)
	{
		string name = Enum.GetName(typeof(Environments), e);
		name = name.Replace("_", " ");
		return name;
	}
	public static string Name(this Environments e)
	{
		EnvironmentAttribute attribute = e.GetAttributeOfType<EnvironmentAttribute>();
		if (attribute != null)
			return attribute.Name;
		return "";
	}
}

public static class EnumHelper
{
	/// <summary>
	/// Gets an attribute on an enum field value
	/// </summary>
	/// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
	/// <param name="enumVal">The enum value</param>
	/// <returns>The attribute of type T that exists on the enum value</returns>
	/// <example><![CDATA[string desc = myEnumVariable.GetAttributeOfType<DescriptionAttribute>().Description;]]></example>
	public static T GetAttributeOfType<T>(this Enum enumVal) where T : System.Attribute
	{
		var type = enumVal.GetType();
		var memInfo = type.GetMember(enumVal.ToString());
		var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
		return (attributes.Length > 0) ? (T)attributes[0] : null;
	}
}

[RequireComponent(typeof(PhotoMode.PhotoMode))]
public class ValheimPhotoMode : MonoBehaviour
{
	[SerializeField] ValheimPhotoModeSettings valheimSettings;
	[SerializeField] Transform light;

	public void Update()
	{
		light.rotation = Quaternion.Euler(180 * valheimSettings.TimeOfDay.Value, 45, 0);
	}
}
