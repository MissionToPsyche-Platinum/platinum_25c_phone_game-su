using System;
using UnityEditor.Hardware;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;

public class JoystickMover : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private RectTransform joystickBase;
    [SerializeField] private OnScreenStick joystickKnob;

    private Vector2 defaultPosition;
    
    private void Awake()
    {
        defaultPosition = joystickBase.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // convert screen position to position in canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, eventData.position, eventData.pressEventCamera, out var localPoint); // uses pressEventCamera as recommended [web:7]

        joystickBase.anchoredPosition = localPoint;
        joystickBase.gameObject.SetActive(true);
        
        //send the pointer down event to be used for the joystick
        ExecuteEvents.Execute(joystickKnob.gameObject, eventData, ExecuteEvents.pointerDownHandler);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //send the drag event to be used for the joystick
        ExecuteEvents.Execute(joystickKnob.gameObject, eventData, ExecuteEvents.dragHandler);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ExecuteEvents.Execute(joystickKnob.gameObject, eventData, ExecuteEvents.pointerUpHandler);
    }
}