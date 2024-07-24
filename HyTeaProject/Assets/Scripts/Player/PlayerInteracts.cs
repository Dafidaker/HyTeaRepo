using System;
using UnityEngine;
using UnityEngine.Serialization;


public class PlayerInteracts : MonoBehaviour
{
    [field:SerializeField] private float interactRange;
    [field:SerializeField] private bool isDebuggingObjectName;
    
    private bool _interactThoroughMouse;

    private Camera playerCamera;
    
    private GameObject _observedGameObject;
    private GameObject _previousObservedGameObject;

    private void Awake()
    {
        _interactThoroughMouse = false;
        _observedGameObject = null;
        isDebuggingObjectName = false;

        playerCamera = GameManager.Instance.currentCamera;
    }

    private void OnEnable()
    {
        EventManager.CameraWasLocked.AddListener(() => { _interactThoroughMouse = true; });
        EventManager.CameraWasUnlocked.AddListener(() => { _interactThoroughMouse = false; });
        //EventManager.CameraWasChanged.AddListener((cam) => { playerCamera = cam; });
    }

    private void OnDisable()
    {
        EventManager.CameraWasLocked.RemoveListener(() => { _interactThoroughMouse = true; });
        EventManager.CameraWasUnlocked.RemoveListener(() => { _interactThoroughMouse = false; });
        //EventManager.CameraWasChanged.RemoveListener((cam) => { playerCamera = cam; });
    }

    private void Update()
    {
        _observedGameObject = null;

        var potentialRay = CreateRay();

        if (potentialRay == null) return;
        
        var ray = (Ray) potentialRay;
        
        //Debug.DrawRay(ray.origin, ray.direction*100);
        
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

    private Ray? CreateRay()
    {
        if (playerCamera == null)
        {
            return null;
        }
        
        return _interactThoroughMouse 
            ? playerCamera.ScreenPointToRay(Input.mousePosition) 
            : playerCamera.ScreenPointToRay(new Vector2(Screen.width/2, Screen.height/2));
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
