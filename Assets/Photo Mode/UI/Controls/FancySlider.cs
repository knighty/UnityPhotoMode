using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
class FancySlider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, ILayoutElement
{
	[SerializeField] Image handle;
	[SerializeField] RectTransform backgroundRect;

	Slider slider;
	float state = 0;
	bool hovering = false;
	bool selected = false;
	Coroutine co = null;

	public float minWidth => -1;
	public float preferredWidth => -1;
	public float flexibleWidth => -1;
	public float minHeight => -1;
	public float preferredHeight => -1;
	public float flexibleHeight => -1;
	public int layoutPriority => -1;

	Slider Slider { get { return slider ??= GetComponent<Slider>(); } }

	protected void GoToState(float newState)
	{
		if (co != null)
			StopCoroutine(co);
		co = StartCoroutine(Utils.Tween(state, newState, 10.0f, s =>
		{
			state = Mathf.SmoothStep(0, 1, s);
			UpdateState();
		}, 0));
	}

	private void Start()
	{
		UpdateState();
	}

	private void UpdateState()
	{
		float offset = (Slider.transform as RectTransform).rect.height / 4.0f;
		float invState = 1 - state;

		Slider.fillRect.offsetMin = new Vector2(Slider.fillRect.offsetMin.x, invState * offset);
		Slider.fillRect.offsetMax = new Vector2(Slider.fillRect.offsetMax.x, -invState * offset);

		backgroundRect.anchorMin = new Vector2(0, 0.25f);
		backgroundRect.anchorMax = new Vector2(1, 1 - 0.25f);

		Slider.handleRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, handle.preferredWidth * state);
		Slider.handleRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, handle.preferredHeight * state);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		GoToState(1);
		hovering = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		hovering = false;
		if (!selected)
		{
			GoToState(0);
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		selected = true;
		GoToState(1);
	}

	public void OnDeselect(BaseEventData eventData)
	{
		selected = false;
		if (!hovering)
		{
			GoToState(0);
		}
	}

	public void CalculateLayoutInputHorizontal()
	{
		UpdateState();
	}

	public void CalculateLayoutInputVertical()
	{
		UpdateState();
	}
}
