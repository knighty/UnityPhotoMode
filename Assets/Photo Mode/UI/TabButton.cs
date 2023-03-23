using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabButton : MonoBehaviour, IPointerClickHandler
{
	private bool selected = false;

	public Action<TabButton> OnClick;

	public bool Selected
	{
		get => selected;
		set
		{
			selected = value;
		}
	}

	public string Text
	{
		get => GetComponent<Text>().text;
		set => GetComponent<Text>().text = value;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		OnClick?.Invoke(this);
	}
}
