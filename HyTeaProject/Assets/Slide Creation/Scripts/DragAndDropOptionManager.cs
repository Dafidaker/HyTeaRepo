using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class DragAndDropOptionManager : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerUpHandler
{
    private bool _placed;
    private Transform _optionTransform;
    private CanvasGroup _optionCanvasGroup;
    private Vector3 _defaultPosition;

    public int Index;

    private void Awake()
    {
        _optionTransform = GetComponent<Transform>();
        _defaultPosition = GetComponent<Transform>().position;
        _optionCanvasGroup = GetComponent<CanvasGroup>();
        _placed = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("test");
        _optionTransform.position = eventData.position;
        _optionCanvasGroup.alpha = .6f;
        SetAsPlaced(false);
        EventManager.GrabOptionEvent.Invoke();
       
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("Begin Drag");
        _optionCanvasGroup.alpha = .6f;
        _optionCanvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("End Drag");
        _optionCanvasGroup.alpha = 1f;
        _optionCanvasGroup.blocksRaycasts = true;
        if (!_placed)
        {
            ResetPosition();
            EventManager.DropOptionEvent.Invoke();
        }
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log(_defaultPosition);
        _optionTransform.position = eventData.position;
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("Drop");
        if (!_placed)
        {
            ResetPosition();
            EventManager.DropOptionEvent.Invoke();
            _optionCanvasGroup.alpha = 1f;
        }
        else
        {
            Debug.Log("Item was placed");
        }
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
