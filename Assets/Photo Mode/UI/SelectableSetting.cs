using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableSetting : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	[SerializeField] private Image image;

	float transitionState = 0;

	private Coroutine coroutine;
	bool isFocused = false;

	public void Start()
	{
		image.type = Image.Type.Tiled;
		image.color = Color.clear;
		foreach(var selectable in GetComponentsInChildren<Selectable>())
		{
			selectable.gameObject.AddComponent<SelectableSettingChild>();
		}
	}

	public delegate bool CoFunc();
	private static System.Collections.IEnumerator CoroutineRunnerSimple(CoFunc func)
	{
		while (true)
		{
			if (func())
			{
				yield return null;
			}
			else
			{
				yield break;
			}
		}
	}

	public void Tween(float state, float to, float speed, Action<float> handler)
	{
		if (coroutine != null)
			StopCoroutine(coroutine);
		coroutine = StartCoroutine(CoroutineRunnerSimple(() =>
		{
			float delta = Mathf.Abs(state - to);
			if (delta < 0.0001f)
			{
				state = to;
				handler(state);
				return false;
			}

			state += ((to > state) ? 1 : -1) * Mathf.Min(delta, Time.unscaledDeltaTime * speed);
			handler(state);
			return true;
		}));
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		//if (EventSystem.current.currentSelectedGameObject == null)
		if (!isFocused)
			GoToState(0.5f);
	}

	public void GoToState(float newState)
	{
		Tween(transitionState, newState, 8.0f, state =>
		{
			transitionState = state;
			image.color = new Color(1, 1, 1, Mathf.SmoothStep(0, 1, transitionState));
		});
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!isFocused)
			GoToState(0);
	}

	public void Focus()
	{
		GoToState(1);
		isFocused = true;
	}

	public void Blur()
	{
		GoToState(0);
		isFocused = false;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		Selectable selectable = GetComponentInChildren<Selectable>();
		if (selectable != null)
			selectable.Select();
	}
}

class SelectableSettingChild : MonoBehaviour, ISelectHandler, IDeselectHandler
{
	public void OnDeselect(BaseEventData eventData)
	{
		GetComponentInParent<SelectableSetting>().Blur();
	}

	public void OnSelect(BaseEventData eventData)
	{
		GetComponentInParent<SelectableSetting>().Focus();
	}
}