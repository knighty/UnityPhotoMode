using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class BetterInputField : MonoBehaviour, ILayoutElement
{
	[SerializeField] Vector2 padding;

	InputField inputField;

	InputField InputField { get { return inputField ??= GetComponent<InputField>(); } }

	public float minWidth => -1;

	public float preferredWidth => InputField.preferredWidth + padding.x * 2;

	public float flexibleWidth => -1;

	public float minHeight => -1;

	public float preferredHeight => InputField.preferredHeight + padding.y * 2;

	public float flexibleHeight => -1;

	public int layoutPriority => 2;

	public void CalculateLayoutInputHorizontal()
	{
	}

	public void CalculateLayoutInputVertical()
	{
	}
}
