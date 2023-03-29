using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PhotoMode.UI
{
	public class Window : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
	{
		[SerializeField] Button closeButton;
		[SerializeField] RectTransform content;
		[SerializeField] Text title;
		[SerializeField] RectTransform draggableRect;

		public RectTransform Content { get => content; }
		public string Title { set => title.text = value; }

		public event Action OnCloseClicked;
		public event Action OnClose;

		private Vector2 dragOffset;
		private bool dragging;

		public void OnDrag(PointerEventData eventData)
		{
			if (!dragging) return;

			Vector2 desiredPos = eventData.position - dragOffset;
			desiredPos.x = Mathf.Clamp(desiredPos.x, 0, Screen.width - (transform as RectTransform).rect.width);
			desiredPos.y = Mathf.Clamp(desiredPos.y, 0, Screen.height - (transform as RectTransform).rect.height);
			(transform as RectTransform).anchoredPosition = desiredPos;
		}

		void Start()
		{
			if (closeButton != null)
			{
				closeButton.onClick.AddListener(() => {
					OnCloseClicked?.Invoke();
					Close();
				});
			}
		}

		public void Close()
		{
			OnClose?.Invoke();
			this.gameObject.SetActive(false);
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(draggableRect, eventData.position, null, out Vector2 localMousePos);
			if (draggableRect.rect.Contains(localMousePos))
			{
				dragging = true;
				dragOffset = eventData.position - (transform as RectTransform).anchoredPosition;
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			dragging = false;
			//throw new NotImplementedException();
		}
	}
}
