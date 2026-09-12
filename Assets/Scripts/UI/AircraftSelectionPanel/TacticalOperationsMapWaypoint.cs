using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TacticalOperationsMapWaypoint : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerClickHandler
{
    public event Action<TacticalOperationsMapWaypoint> WaypointRightButtonClicked;

    [SerializeField] private RectTransform _rectTransform;

    private Canvas _canvas;
    private RectTransform _parentRectTransform;

    private TacticalOperationsMapPathManager _tacticalOperationsMapPathManager;

    private Vector2 _offset;

    private float _waypointHalfWidth;
    private float _waypointHalfheight;
    private float _waypointParentHalfWidth;
    private float _waypointParentHalfheight;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _offset = CalculateLocalPoint(eventData) - _rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 newPos = CalculateLocalPoint(eventData) - _offset;

        newPos.x = Mathf.Clamp(newPos.x, -_waypointParentHalfWidth + _waypointHalfWidth, _waypointParentHalfWidth - _waypointHalfWidth);
        newPos.y = Mathf.Clamp(newPos.y, -_waypointParentHalfheight + _waypointHalfheight, _waypointParentHalfheight - _waypointHalfheight);

        _rectTransform.anchoredPosition = newPos;

        _tacticalOperationsMapPathManager.UpdatePath();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            WaypointRightButtonClicked?.Invoke(this);
        }
    }

    public void Init(TacticalOperationsMapPathManager tacticalOperationsMapPathManager, Canvas canvas, RectTransform parentRectTransform)
    {
        _tacticalOperationsMapPathManager = tacticalOperationsMapPathManager;
        _canvas = canvas;
        _parentRectTransform = parentRectTransform;
        _waypointHalfWidth = _rectTransform.rect.size.x * 0.5f;
        _waypointHalfheight = _rectTransform.rect.size.y * 0.5f;
        _waypointParentHalfWidth = _parentRectTransform.rect.size.x * 0.5f;
        _waypointParentHalfheight = _parentRectTransform.rect.size.y * 0.5f;
    }

    private Vector2 CalculateLocalPoint(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRectTransform, eventData.position, _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera, out Vector2 localPoint);

        return localPoint;
    }
}
