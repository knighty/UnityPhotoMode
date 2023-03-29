using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace PhotoMode.UI
{
	[CreateAssetMenu(fileName = "Profile Repository", menuName = "Photo Mode/Profiles/Repository")]
	public class ProfileRepository : ScriptableObject
	{
		[SerializeField] string directory = "";

		List<Profile> profiles = new List<Profile>();

		public event Action<List<Profile>> OnProfilesChanged;

		public void OnEnable()
		{
			if (directory != "")
			{
				LoadProfilesFromFolder(directory);
			}
		}

		public List<Profile> Profiles { get { return profiles; } }

		public Profile GetProfileByName(string name, string category)
		{
			return profiles.Find(profile => profile.Name == name && profile.Category == category);
		}

		public IEnumerable<Profile> GetProfilesByCategory(string category)
		{
			return profiles.Where(profile => profile.Category == category);
		}

		public void LoadProfilesFromFolder(string folder)
		{
			if (!Directory.Exists(folder))
				return;
			string[] files = Directory.GetFiles(folder, "*.profile", SearchOption.AllDirectories);
			profiles = files
				.Select(Profile.FromFile)
				.ToList();
			OnProfilesChanged?.Invoke(profiles);
		}

		public bool SaveProfile(Profile profile, PhotoModeSettings settings, string category = "")
		{
			string path = $"{directory}{category} - {profile.Name}.profile";
			profile.SetData(settings, category);
			string json = JsonUtility.ToJson(profile);
			var dir = Path.GetDirectoryName(path);
			Directory.CreateDirectory(dir);
			File.WriteAllText(path, json);
			OnProfilesChanged?.Invoke(profiles);
			return true;
		}

		public Profile CreateNewProfile(string name, PhotoModeSettings settings, string category = "")
		{
			Profile profile = new Profile();
			profile.Name = name;
			profiles.Add(profile);
			SaveProfile(profile, settings, category);
			return profile;
		}

		public void DeleteProfile(Profile profile)
		{
			profiles.Remove(profile);
			OnProfilesChanged?.Invoke(profiles);
		}
	}
}
