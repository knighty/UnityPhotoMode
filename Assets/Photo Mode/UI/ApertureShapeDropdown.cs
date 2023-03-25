using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoMode.UI
{
	public class ApertureShapeDropdown : MonoBehaviour
	{
		[SerializeField]
		private ApertureShapeRepository shapeRepository;

		List<ApertureShapeRepository.Entry> entries;

		public Action<ApertureShape> OnChange;

		public void SetSelected(ApertureShape shape)
		{
			Dropdown dropdown = GetComponent<Dropdown>();
			for (int i = 0; i < entries.Count; i++)
			{
				if (entries[i].ApertureShape == shape)
				{
					dropdown.SetValueWithoutNotify(i);
				}
			}
		}

		void Awake()
		{
			Dropdown dropdown = GetComponent<Dropdown>();
			entries = shapeRepository.GetShapes().ToList();
			if (shapeRepository != null)
			{
				dropdown.AddOptions(entries.Select(item => new Dropdown.OptionData(item.Name)).ToList());
			}
			dropdown.onValueChanged.AddListener(i => OnChange?.Invoke(entries[i].ApertureShape));
		}
	}
}