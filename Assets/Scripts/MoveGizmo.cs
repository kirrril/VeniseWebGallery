using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveGizmo : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private float startPointX;
    private float startPointY;
    [SerializeField] private RectTransform handleRect;
    public float speedX;
    public float speedY;
    private float maxDragDistance = 100f;
    private float joystickRange = 50f;
    private Vector2 handleRestPosition;

    void Awake()
    {
        RectTransform frameRect = GetComponent<RectTransform>();

        if (handleRect == null)
        {
            handleRect = frameRect;
        }

        handleRestPosition = handleRect.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPointX = eventData.position.x;
        startPointY = eventData.position.y;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float currentPointX = eventData.position.x;
        float currentPointY = eventData.position.y;
        float dragDistanceX = currentPointX - startPointX;
        float dragDistanceY = currentPointY - startPointY;

        speedX = Math.Clamp(dragDistanceX / maxDragDistance, -1f, 1f);
        speedY = Math.Clamp(dragDistanceY / maxDragDistance, -1f, 1f);

        MoveTheJoystick();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        speedX = 0;
        speedY = 0;
        MoveTheJoystick();
    }

    private void MoveTheJoystick()
    {
        if (handleRect == null) return;

        float x = Mathf.Clamp(speedX * joystickRange, -joystickRange, joystickRange);
        float y = Mathf.Clamp(speedY * joystickRange, -joystickRange, joystickRange);

        handleRect.anchoredPosition = handleRestPosition + new Vector2(x, y);
    }
}
