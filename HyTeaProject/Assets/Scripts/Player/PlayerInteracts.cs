using System;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public abstract class Interactable : MonoBehaviour
{
    private Outline _outline;
    
    private void OnEnable()
    {
        EventManager.InteractableIsBeingWatched.AddListener(LookedAt);
        EventManager.InteractableIsBeingWatched.AddListener(StoppedBeingLookedAt);
    }

    private void OnDisable()
    {
        EventManager.InteractableIsBeingWatched.RemoveListener(LookedAt);
        EventManager.InteractableIsBeingWatched.RemoveListener(StoppedBeingLookedAt);
    }

    private void Awake()
    {
        _outline = GetComponent<Outline>();
        _outline.enabled = false;
    }

    public abstract void Interact();

    protected virtual void LookedAt(Interactable obj)
    {
        if (obj == this)
        {
            _outline.enabled = true;
        }
    }

    protected virtual void StoppedBeingLookedAt(Interactable obj)
    {
        if (obj != this)
        {
            _outline.enabled = false;
        }
    }
}

interface IInteractable
{
    public void Interact();
    public void LookedAt();
    public void StoppedBeingLookedAt();
}



public class PlayerInteracts : MonoBehaviour
{
    [field:SerializeField] private Transform directionTranform;
    [field:SerializeField] private float interactRange;
    private GameObject _observedGameObject;
    private GameObject _previousObservedGameObject;

    private void Awake()
    {
        _observedGameObject = null;
    }

    private void Update()
    {
        _observedGameObject = null;
        
        Ray ray = new Ray(transform.position, directionTranform.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, interactRange))
        {
            _observedGameObject = hitInfo.transform.gameObject;
            
        }

        if (_observedGameObject == null)
        {
            _previousObservedGameObject = null;
            return;
        }

        if (_observedGameObject != _previousObservedGameObject)
        {
            if (_observedGameObject.TryGetComponent(out Interactable _interactable))
            {
                EventManager.InteractableIsBeingWatched.Invoke(_interactable);
            }
        }

        if (Input.GetKey(KeyCode.E))
        {
            if (_observedGameObject.TryGetComponent(out Interactable _interactable))
            {
                _interactable.Interact();
            }

        }

    }
}
