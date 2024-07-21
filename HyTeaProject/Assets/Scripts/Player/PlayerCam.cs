using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCam : MonoBehaviour
{
    [field: SerializeField] private float sensX;
    [field: SerializeField] private float sensY;
    
    public bool cameraIsLocked;
    private bool cameraIsMoving;
    public Transform orientation;

    private float xRotation;
    private float yRotation;
    
    //locked camera//
    [SerializeField] private Transform lookAtTranformLockedCamera;
    private List<Transform> _lookAtTranforms;
    [SerializeField] private Vector2 easeAngles;
    
    private Vector2 _smoothMousePosition;
    private Vector3 _velocity;
    /////////////////

    private void Awake()
    {
        GameManager.Instance.SetPlayerCam(this);
        _lookAtTranforms = new List<Transform> { lookAtTranformLockedCamera };
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void Update()
    {
        LockInput();
        
        if (cameraIsLocked) return;
        
        CreateRotation();
        
        RotateCamera();
    }

    private void LockInput()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            cameraIsLocked = !cameraIsLocked;
            
            if (cameraIsLocked)
            {
                LockCamera();
            }
            else
            {
                UnlockCamera();
            }
        }
        
    }
    
    private void LateUpdate()
    {
        if (!cameraIsLocked) return;
        
        RotateLockedCamera();
    }

    private void CreateRotation()
    {
        var mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        var mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }

    private void RotateCamera()
    {
        transform.rotation = Quaternion.Euler(xRotation,yRotation,0);
        orientation.rotation = Quaternion.Euler(0,yRotation,0);
    }
    
    public Vector2 MouseOnScreen()
    {
        Vector2 rawPos = Input.mousePosition;
        Vector2 normalizedPos = rawPos / new Vector2(Screen.width, Screen.height);
        return normalizedPos * 2f - Vector2.one;
    }

    private void RotateLockedCamera()
    {
        if (cameraIsMoving || GameManager.Instance.GameState != GameState.Presentation) return;
            
        LookToTransform();
        
        _smoothMousePosition = Vector2.Lerp(MouseOnScreen(), _smoothMousePosition, Mathf.Pow(0.0001f, Time.deltaTime * 0.99f));
        
        transform.Rotate(-Vector3.right, _smoothMousePosition.y * easeAngles.y);
        transform.Rotate(Vector3.up, _smoothMousePosition.x * easeAngles.x);
    }

    private void LookToTransform()
    {
        if(_lookAtTranforms == null || _lookAtTranforms.Count < 0) return;
        
        transform.forward = _lookAtTranforms[^1].position - transform.position;
    }

    private IEnumerator LerpCameraRotation()
    {
        if (_lookAtTranforms == null || _lookAtTranforms.Count <= 0) yield break;

        cameraIsMoving = true;
    
        Transform targetTransform = _lookAtTranforms[^1]; // Target is the last transform in the list
    
        while (Vector3.Angle(transform.forward, targetTransform.position - transform.position) > 5f)
        {
            Vector3 targetDirection = (targetTransform.position - transform.position).normalized;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetDirection), 0.5f * Time.deltaTime * 360); 

            yield return null;
        }
    
        cameraIsMoving = false;
        
    }

    public void LockOnTarget(Transform target)
    { 
        LookAt(target);
        LockCamera();
    }
    
    public void UnlockOnTarget(Transform target)
    { 
        UnlockCamera();
        StopLookingAt(target);
    }
    
    public void LockCamera()
    {
        cameraIsLocked = true;
        EventManager.CameraWasLocked.Invoke();
        _smoothMousePosition = MouseOnScreen();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        StartCoroutine(LerpCameraRotation());
    }
    
    public void UnlockCamera()
    {
        EventManager.CameraWasUnlocked.Invoke();
        cameraIsLocked = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void LookAt(Transform tf)
    {
        if (!_lookAtTranforms.Contains(tf))
        {
            _lookAtTranforms.Add(tf);
            StartCoroutine(LerpCameraRotation());
        }
    }

    public void StopLookingAt(Transform tf)
    {
        if (tf == lookAtTranformLockedCamera || _lookAtTranforms.Count <= 1) return;
        
        _lookAtTranforms.Remove(tf);
        StartCoroutine(LerpCameraRotation());
    }
}
