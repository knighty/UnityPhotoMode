using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class BetterInputField : MonoBehaviour, ILayoutElement
{
	[SerializeField] Vector2 padding;

	InputField inputField;

	InputField InputField { get => inputField ??= GetComponent<InputField>(); }

	public float minWidth => -1;

	public float preferredWidth => InputField.preferredWidth + padding.x * 2;

	public float flexibleWidth => -1;

	public float minHeight => -1;

	public float preferredHeight => InputField.preferredHeight + padding.y * 2;

	public float flexibleHeight => -1;

	public int layoutPriority => 2;

	public void CalculateLayoutInputHorizontal()
	{
		RectTransform rectTransform = InputField.textComponent.transform as RectTransform;
		rectTransform.anchorMin = Vector2.zero;
		rectTransform.anchorMax = Vector2.one;
		rectTransform.offsetMin = new Vector2(padding.x, padding.y);
		rectTransform.offsetMax = new Vector2(-padding.x, -padding.y);
	}

	public void CalculateLayoutInputVertical()
	{
	}

#if UNITY_EDITOR
	private void OnValidate()
	{
		LayoutRebuilder.MarkLayoutForRebuild(this.GetComponent<RectTransform>());
	}
#endif
}
