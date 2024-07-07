using System;
using Imports.QuickOutline.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Outline))]
public abstract class Interactable : MonoBehaviour
{
    protected Outline _outline;
    protected bool isInteractable = true;
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


public class PlayerInteracts : MonoBehaviour
{
    [field:SerializeField] private Transform directionTranform;
    [field:SerializeField] private float interactRange;
    [field:SerializeField] private bool isDebuggingObjectName;
    
    private bool _interactThoroughMouse;
    
    private GameObject _observedGameObject;
    private GameObject _previousObservedGameObject;

    private void Awake()
    {
        _interactThoroughMouse = false;
        _observedGameObject = null;
        isDebuggingObjectName = false;

        directionTranform = GameManager.Instance.currentCamera.transform;
    }

    private void OnEnable()
    {
        EventManager.CameraWasLocked.AddListener(() => { _interactThoroughMouse = true; });
        EventManager.CameraWasUnlocked.AddListener(() => { _interactThoroughMouse = false; });
        EventManager.CameraWasChanged.AddListener((cam) => { directionTranform = cam.transform; });
    }

    private void OnDisable()
    {
        EventManager.CameraWasLocked.RemoveListener(() => { _interactThoroughMouse = true; });
        EventManager.CameraWasUnlocked.RemoveListener(() => { _interactThoroughMouse = false; });
        EventManager.CameraWasChanged.RemoveListener((cam) => { directionTranform = cam.transform; });
    }

    private void Update()
    {
        _observedGameObject = null;

        Ray ray = CreateRay();
        Debug.DrawRay(ray.origin, ray.direction*100);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, interactRange))
        {
            _observedGameObject = hitInfo.collider.gameObject;
        }

        if (isDebuggingObjectName) DebugObjectBeingSeen();

        if (_observedGameObject == null)
        {
            EventManager.InteractableIsBeingWatched.Invoke(null);
            _previousObservedGameObject = null;
            return;
        }   

        if (_observedGameObject != _previousObservedGameObject)
        {
            if (_observedGameObject.TryGetComponent(out Interactable _interactable))
            {
                EventManager.InteractableIsBeingWatched.Invoke(_interactable);
            }
            else
            {
                EventManager.InteractableIsBeingWatched.Invoke(null);
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && !_interactThoroughMouse)
        {
            if (_observedGameObject.TryGetComponent(out Interactable _interactable))
            {
                _interactable.Interact();
            }

        }
        
        if (Input.GetKeyDown(KeyCode.Mouse0) && _interactThoroughMouse)
        {
            if (_observedGameObject.TryGetComponent(out Interactable _interactable))
            {
                _interactable.Interact();
            }
        }
        
    }

    private Ray CreateRay()
    {
        if (_interactThoroughMouse)
        {
             return GameManager.Instance.currentCamera.ScreenPointToRay(Input.mousePosition);
        }
        
        //return new Ray(transform.position, directionTranform.forward);
        
        return GameManager.Instance.currentCamera.ScreenPointToRay(new Vector2(Screen.width/2,Screen.height/2));
    }

    private void DebugObjectBeingSeen()
    {
        if (_observedGameObject != null)
        {
            Debug.Log("Object Being Seen: " + _observedGameObject.name);
        }
        else
        {
            Debug.Log("No is Object Being Seen");
        }
    }
    
    
}
