using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
public class DragAndDropSlideManager : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerUpHandler, IDropHandler
{
    private bool _placed;
    private Transform _slideTransform;
    private CanvasGroup _slideCanvasGroup;
    private Vector3 _defaultPosition; 
    private int _indexInSlideOrder;
    private int _numOfSiblings;
    private void Awake()
    {
        _slideTransform = GetComponent<Transform>();
        _defaultPosition = GetComponent<Transform>().position;
        _slideCanvasGroup = GetComponent<CanvasGroup>();
        _placed = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("Slide's Index: " + GetIndex());
        _slideCanvasGroup.alpha = .6f;
        _slideTransform.position = eventData.position;
        _slideTransform.SetSiblingIndex(_numOfSiblings);
        SetAsPlaced(false);
        //EventManager.GrabOptionEvent.Invoke();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("Begin Drag");
        EventManager.GetIndexOfHeldSlideEvent.Invoke(_indexInSlideOrder);
        _slideCanvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("End Drag");
        _slideCanvasGroup.alpha = 1f;
        //EventManager.DropOptionEvent.Invoke();
        if(!_placed) ResetPosition();
        _slideCanvasGroup.blocksRaycasts = true;
        
        if (eventData.pointerCurrentRaycast.gameObject.GetComponent<DragAndDropSlideManager>() != null)
        {
            GameObject hitObj = eventData.pointerCurrentRaycast.gameObject;
            Debug.Log("Slide was dropped on a slide of index: " + hitObj.GetComponent<DragAndDropSlideManager>().GetIndex());
            EventManager.SwapSlidesEvent.Invoke(_indexInSlideOrder, hitObj.GetComponent<DragAndDropSlideManager>().GetIndex());
        }
        else
        {
            Debug.Log("Object doesn't have drag and drop manager.");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log(_defaultPosition);
        _slideTransform.position = eventData.position;
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("Drop");
        if (!_placed)
        {
            ResetPosition();
            //EventManager.DropOptionEvent.Invoke();
            _slideCanvasGroup.alpha = 1f;
        }
        else
        {
            Debug.Log("Item was placed");
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        /*if (eventData.pointerCurrentRaycast.gameObject.GetComponent<DragAndDropSlideManager>() != null)
        {
            GameObject hitObj = eventData.pointerCurrentRaycast.gameObject;
            Debug.Log("Slide was dropped on a slide of index: " + hitObj.GetComponent<DragAndDropSlideManager>().GetIndex());
            EventManager.SwapSlidesEvent.Invoke(_indexInSlideOrder, hitObj.GetComponent<DragAndDropSlideManager>().GetIndex());
        }
        else
        {
            Debug.Log("Object doesn't have drag and drop manager.");
        }*/
    }

    public void SetAsPlaced(bool placed)
    {
        _placed = placed;
    }

    public int GetIndex()
    {
        return _indexInSlideOrder;
    }

    public void SetIndex(int index)
    {
        _indexInSlideOrder = index;
    }

    public void ResetPosition()
    {
        _slideTransform.position = _defaultPosition;
    }

    private void UpdateNumOfSiblings(int num)
    {
        _numOfSiblings = num;
        //Debug.Log("Num of children: " + num);
    }

    public Vector3 GetDefaultPos()
    {
        return _defaultPosition;
    }

    public void SetDefaultPos(Vector3 newPos)
    {
        _defaultPosition = newPos;
    }
    
    private void OnEnable()
    {
        EventManager.GetNumberOfSiblings.AddListener(UpdateNumOfSiblings);
    }

    private void OnDisable()
    {
        EventManager.GetNumberOfSiblings.RemoveListener(UpdateNumOfSiblings);
    }
    
}
