using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContentSizedButton : UIBehaviour, ILayoutElement
{
	[SerializeField] MonoBehaviour content;
	[SerializeField] Vector2 padding = new Vector2(0, 8);

	ILayoutElement contentLayout;

	public float minWidth => (contentLayout?.minWidth ?? 0) + padding.x * 2;
	public float preferredWidth => (contentLayout?.preferredWidth ?? 0) + padding.x * 2;
	public float flexibleWidth => -1;
	public float minHeight => (contentLayout?.minHeight ?? 0) + padding.y * 2;
	public float preferredHeight => (contentLayout?.preferredHeight ?? 0) + padding.y * 2;
	public float flexibleHeight => -1;
	public int layoutPriority => 2;

	public void CalculateLayoutInputHorizontal() { }
	public void CalculateLayoutInputVertical() { }

	protected override void Start()
	{
		if (content is ILayoutElement layout)
		{
			contentLayout = layout;
		}
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
		if (content is ILayoutElement layout)
		{
			contentLayout = layout;
		}
		SetDirty();
	}
#endif
}
