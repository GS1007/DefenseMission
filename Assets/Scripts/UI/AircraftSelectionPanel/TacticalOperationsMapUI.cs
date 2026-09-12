using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TacticalOperationsMapUI : MonoBehaviour, IPointerClickHandler
{
    public event Action<Vector2> OnDoubleClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && eventData.clickCount == 2)
        {
            OnDoubleClick?.Invoke(eventData.position);
        }
    }
}