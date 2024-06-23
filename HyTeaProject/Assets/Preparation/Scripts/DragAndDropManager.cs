using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class DragAndDropManager : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private bool _placed;
    private Transform _optionTransform;
    private CanvasGroup _optionCanvasGroup;
    private Vector3 _defaultPosition;

    private void Awake()
    {
        _optionTransform = GetComponent<Transform>();
        _defaultPosition = GetComponent<Transform>().position;
        _optionCanvasGroup = GetComponent<CanvasGroup>();
        _placed = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("test");
        _optionTransform.position = eventData.position;
        SetAsPlaced(false);
        EventManager.GrabOptionEvent.Invoke();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag");
        _optionCanvasGroup.alpha = .6f;
        _optionCanvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag");
        _optionCanvasGroup.alpha = 1f;
        _optionCanvasGroup.blocksRaycasts = true;
        EventManager.DropOptionEvent.Invoke();
        if(!_placed) ResetPosition();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log(_optionTransform.position);
        _optionTransform.position = eventData.position;
    }

    public void SetAsPlaced(bool placed)
    {
        _placed = placed;
    }

    public void ResetPosition()
    {
        _optionTransform.position = _defaultPosition;
    }

    private void DisableRaycastTarget()
    {
        _optionCanvasGroup.blocksRaycasts = false;
    }
    
    private void EnableRaycastTarget()
    {
        _optionCanvasGroup.blocksRaycasts = true;
    }
    
    private void OnEnable()
    {
        EventManager.GrabOptionEvent.AddListener(DisableRaycastTarget);
        EventManager.DropOptionEvent.AddListener(EnableRaycastTarget);
    }

    private void OnDisable()
    {
        EventManager.GrabOptionEvent.RemoveListener(DisableRaycastTarget);
        EventManager.DropOptionEvent.RemoveListener(EnableRaycastTarget);
    }
}
