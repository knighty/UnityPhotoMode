using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoMode.UI
{
	public class ProfilesWindow : MonoBehaviour
	{
		[SerializeField] private ProfileRepository profileRepository;
		[SerializeField] private ProfileListItem profilePrefab;
		[SerializeField] private InputField profileNameInput;
		[SerializeField] private Button saveButton;
		[SerializeField] private Button loadButton;

		[SerializeField] private PhotoModeSettings settings;

		private string category = "Camera";
		private Profile selectedProfile;
		private Window window;

		public PhotoModeSettings Settings { set => settings = value; }
		public string Category
		{
			get => category; 
			set
			{
				category = value;
				UpdateProfiles(null);
			}
		}

		public event Action<Profile> OnLoadProfile;

		private void Awake()
		{
			window = GetComponent<Window>();
			saveButton.onClick.AddListener(Save);
			loadButton.onClick.AddListener(LoadSelected);
			profileNameInput.onValueChanged.AddListener(InputChanged);
		}

		void Start()
		{
			profileRepository.OnProfilesChanged += UpdateProfiles;
			UpdateProfiles(profileRepository.Profiles);
		}

		private void ClearProfiles()
		{
			int childs = window.Content.transform.childCount;
			for (int i = childs - 1; i >= 0; i--)
			{
				Destroy(window.Content.transform.GetChild(i).gameObject);
			}
		}

		private void UpdateProfiles(List<Profile> profiles)
		{
			ClearProfiles();
			foreach (var profile in profileRepository.GetProfilesByCategory(category))
			{
				ProfileListItem p = Instantiate(profilePrefab, window.Content);
				p.Profile = profile;
				p.OnClick += SelectProfile;
				p.OnDoubleClick += profile =>
				{
					SelectProfile(profile);
					LoadSelected();
				};
				p.OnDelete += profile => profileRepository.DeleteProfile(profile);
			}
		}

		private void InputChanged(string name)
		{
			Profile p = profileRepository.GetProfileByName(name, category);
			selectedProfile = p;
			loadButton.interactable = p != null;
		}

		private void LoadSelected()
		{
			if (selectedProfile == null)
				return;

			OnLoadProfile?.Invoke(selectedProfile);
			selectedProfile.ApplyToSettings(settings);
		}

		private void Save()
		{
			if (selectedProfile != null)
			{
				profileRepository.SaveProfile(selectedProfile, settings, category);
			}
			else if (profileNameInput.text != "")
			{
				profileRepository.CreateNewProfile(profileNameInput.text, settings, category);
			}
		}

		private void SelectProfile(Profile profile)
		{
			selectedProfile = profile;
			profileNameInput.text = profile.Name;
		}
	}
}
