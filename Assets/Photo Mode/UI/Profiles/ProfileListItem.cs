using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PhotoMode.UI
{
	public class ProfileListItem : MonoBehaviour, IPointerClickHandler
	{
		[SerializeField] Text label;
		[SerializeField] Button deleteButton;

		private Profile profile;

		public event Action<Profile> OnClick;
		public event Action<Profile> OnDoubleClick;
		public event Action<Profile> OnDelete;

		public Profile Profile
		{
			get => profile;
			set
			{
				profile = value;
				label.text = profile.Name;
			}
		}

		private void Awake()
		{
			deleteButton.onClick.AddListener(() => OnDelete?.Invoke(profile));
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			OnClick?.Invoke(profile);
			if (eventData.clickCount == 2)
			{
				OnDoubleClick?.Invoke(profile);
			}
		}
	}
}
