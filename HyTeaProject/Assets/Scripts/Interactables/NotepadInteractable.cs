using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

internal enum LookingAtNotepad
{
    CameraMoves,
    NotepadMoves
}

public class NotepadInteractable : Interactable
{
    [SerializeField] private LookingAtNotepad LookingAtNotepadMode;
    [Header("Camera Moves"),SerializeField] private Transform CameraLookAt;
    [Header("Notepad Moves"), SerializeField] private Transform pickedUpTransform;
    [ SerializeField] private Transform holdingTransform;
    
    [Header("Notepad Scrolling"),  SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float scrollSpeed = 50f;


    private bool IsBeingLookedAt = false;
    private PlayerCam playerCam;
    private bool notepadMoving;
    
    public override void Interact()
    {
        
    }
    
   protected override void LookedAt(Interactable obj)
    {
        if (obj == this && isInteractable)
        {
            
            isBeingWatched = true;
            
            //_outline.enabled = true;
            
            if (playerCam == null)
            {
                playerCam = GameManager.Instance.PlayerCam;
            }
            
            if (playerCam != null && playerCam.cameraIsLocked)
            {
                EventManager.StartedLookingAtTheNotes.Invoke();
                if (LookingAtNotepadMode == LookingAtNotepad.CameraMoves)
                {
                    StartCoroutine(UninteractableForSeconds(0.5f));
                    playerCam.LookAt(CameraLookAt);
                }
                else if (LookingAtNotepadMode == LookingAtNotepad.NotepadMoves)
                {
                    StartCoroutine(LerpToTransform(pickedUpTransform,0.5f));
                }
            }
            
        }
        
    }

    private void CheckToMoveScroll()
    {
        if(Input.mouseScrollDelta.y != 0)
        {
            // Get the current scroll position
            float newY = scrollRect.verticalNormalizedPosition + (Input.mouseScrollDelta.y * scrollSpeed * Time.deltaTime);

            // Clamp the value between 0 and 1
            newY = Mathf.Clamp(newY, 0f, 1f);

            // Set the new scroll position
            scrollRect.verticalNormalizedPosition = newY;
        }
    }

    private void Update()
    {
        if (isBeingWatched)
        {
            CheckToMoveScroll();
        }
    }

    protected override void StoppedBeingLookedAt(Interactable obj)
    {
        if (obj != this && isInteractable && isBeingWatched)
        {
            //_outline.enabled = false;
            isBeingWatched = false;
            
            if (playerCam == null)
            {
                playerCam = GameManager.Instance.PlayerCam; 
            }
            
            if (playerCam != null && playerCam.cameraIsLocked)
            {
                EventManager.StoppedLookingAtTheNotes.Invoke();
                if (LookingAtNotepadMode == LookingAtNotepad.CameraMoves)
                {
                    playerCam.StopLookingAt(CameraLookAt);
                }
                else if (LookingAtNotepadMode == LookingAtNotepad.NotepadMoves)
                {
                    StartCoroutine(LerpToTransform(holdingTransform,0.5f));
                }
            }
        }
        
    }

    private IEnumerator UninteractableForSeconds(float duration)
    {
        isInteractable = false;
        yield return new WaitForSeconds(duration);
        isInteractable = true;
    }

    private IEnumerator LerpToTransform(Transform target, float duration)
    {
        if (notepadMoving) yield break; 
        notepadMoving = true;
        isInteractable = false;
        
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / duration;

            transform.position = Vector3.Lerp(startPosition, target.position, t);
            transform.rotation = Quaternion.Slerp(startRotation, target.rotation, t);

            yield return null;
        }

        transform.position = target.position;
        transform.rotation = target.rotation;
        
        isInteractable = true;
        notepadMoving = false;
        StartCoroutine(UninteractableForSeconds(0.2f));
    }
}
