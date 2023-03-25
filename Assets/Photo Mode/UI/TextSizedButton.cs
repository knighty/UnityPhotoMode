using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextSizedButton : UIBehaviour, ILayoutElement
{
	[SerializeField] Text text;
	[SerializeField] Vector2 padding = new Vector2(0, 8);

	public float minWidth => text.minWidth + padding.x * 2;

	public float preferredWidth => text.preferredWidth + padding.x * 2;

	public float flexibleWidth => -1;

	public float minHeight => text.minHeight + padding.y * 2;

	public float preferredHeight => text.preferredHeight + padding.y * 2;

	public float flexibleHeight => -1;

	public int layoutPriority => 2;

	public void CalculateLayoutInputHorizontal()
	{
	}

	public void CalculateLayoutInputVertical()
	{
	}

	protected void SetDirty()
	{
		if (IsActive())
		{
			LayoutRebuilder.MarkLayoutForRebuild(base.transform as RectTransform);
		}
	}

#if UNITY_EDITOR
	protected override void OnValidate()
	{
		SetDirty();
	}
#endif
}
