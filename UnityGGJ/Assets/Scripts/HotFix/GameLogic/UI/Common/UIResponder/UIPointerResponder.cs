using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class UIPointerResponder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Action OnMouseEnter { get; set; }
    public Action OnMouseExit { get; set; }
    public Action OnMouseLeftDown { get; set; }
    public Action OnMouseLeftUp { get; set; }
    public Action OnMouseRightDown { get; set; }
    public Action OnMouseRightUp { get; set; }
    public Action OnMouseMiddleDown { get; set; }
    public Action OnMouseMiddleUp { get; set; }
    public Action<Vector2, Vector2> OnMouseDrag { get; set; }
    public Action<Vector2> OnMouseDragBegin { get; set; }
    public Action<Vector2> OnMouseDragRelease { get; set; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseExit?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                OnMouseLeftDown?.Invoke();
                OnMouseDragBegin?.Invoke(eventData.position);
                break;
            case PointerEventData.InputButton.Right:
                OnMouseRightDown?.Invoke();
                break;
            case PointerEventData.InputButton.Middle:
                OnMouseMiddleDown?.Invoke();
                break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                OnMouseDragRelease?.Invoke(eventData.position);
                OnMouseLeftUp?.Invoke();
                break;
            case PointerEventData.InputButton.Right:
                OnMouseRightUp?.Invoke();
                break;
            case PointerEventData.InputButton.Middle:
                OnMouseMiddleUp?.Invoke();
                break;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnMouseDrag?.Invoke(eventData.position, eventData.delta);
    }
}
