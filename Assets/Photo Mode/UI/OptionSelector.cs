using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionSelector : MonoBehaviour, IPointerClickHandler, ILayoutElement
{
	[Serializable]
	public class Option
	{
		public int Id;
		public string Name;
	}

	[SerializeField] Sprite leftSprite;
	[SerializeField] Sprite middleSprite;
	[SerializeField] Sprite rightSprite;

	[SerializeField] Text label;

	[SerializeField] RectTransform handle;
	[SerializeField] RectTransform background;

	[SerializeField] List<Option> options;

	[SerializeField] float transitionDuration = 0.3f;

	public UnityEvent<Option> OnSelected;

	public float minWidth => 0;
	public float preferredWidth => 0;
	public float flexibleWidth => 1;
	public float minHeight => 30;
	public float preferredHeight => 30;
	public float flexibleHeight => 0;
	public int layoutPriority => 2;

	int selected = 0;
	float selectedPos = 0;
	Coroutine selectedCoroutine = null;

	public List<Option> Options
	{
		get => options;
		set
		{
			options = value;
			UpdateOptions();
		}
	}

	public Option Selected
	{
		get => options[selected];
		set
		{
			for (int i = 0; i < options.Count; i++)
			{
				if (options[i].Id == value.Id)
				{
					SetSelected(i);
					return;
				}
			}
		}
	}

	public int SelectedID
	{
		get => selected;
	}

	private void Awake()
	{
		UpdateOptions();
	}

	private void ClearProperties()
	{
		if (background == null)
			return;
		int childs = background.transform.childCount;
		for (int i = childs - 1; i >= 0; i--)
		{
			Destroy(background.transform.GetChild(i).gameObject);
		}
	}

	private void UpdateOptions()
	{
		if (background == null)
			return;
		ClearProperties();
		SetSelected(0, false, false);

		int i = 0;
		foreach (Option option in options)
		{
			GameObject obj = new GameObject(option.Name);
			Image image = obj.AddComponent<Image>();
			image.sprite = middleSprite;
			if (i == 0)
				image.sprite = leftSprite;
			if (i == options.Count - 1)
				image.sprite = rightSprite;
			image.type = Image.Type.Sliced;
			i++;
			obj.transform.SetParent(background);
		}
	}

	public void SetSelected(int id, bool alertListeners = true, bool animate = true)
	{
		if (id < 0 || id > options.Count - 1) return;

		selected = id;
		label.text = Selected.Name;

		float targetPos = SelectedID * 1.0f / options.Count;
		if (selectedCoroutine != null)
			StopCoroutine(selectedCoroutine);
		if (animate)
		{
			selectedCoroutine = StartCoroutine(Utils.TweenTime(selectedPos, targetPos, transitionDuration, s =>
			{
				selectedPos = s;
				LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
			}, Utils.EaseOutBack));
		}
		else
		{
			selectedPos = targetPos;
			LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
		}

		if (alertListeners)
			OnSelected?.Invoke(options[selected]);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		Rect rect = background.rect;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, null, out Vector2 clickPosition);
		int clickId = (int)((clickPosition.x * options.Count) / rect.width);
		SetSelected(clickId);
	}

	public void CalculateLayoutInputHorizontal()
	{
		float a = 1.0f / options.Count;
		handle.anchorMin = new Vector2(selectedPos, 0);
		handle.anchorMax = new Vector2(selectedPos + a, 1);
	}

	public void CalculateLayoutInputVertical() { }
}
