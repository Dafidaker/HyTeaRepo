using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NotesInteractable : Interactable
{
    [field: SerializeField] private Transform CameraLookAt;
    
    public override void Interact()
    {
        
    }
    
   protected override void LookedAt(Interactable obj)
    {
        if (obj == this && isInteractable)
        {
            StartCoroutine(UninteractableForSeconds(0.5f));
            //_outline.enabled = true;
            
            PlayerCam playerCam = GameManager.Instance.currentCamera.gameObject.GetComponent<PlayerCam>();
            if (playerCam.cameraIsLocked)
            {
                playerCam.AddLookAtTransform(CameraLookAt);
            }
        }
        
    }

    protected override void StoppedBeingLookedAt(Interactable obj)
    {
        if ((obj != this || obj == null) && isInteractable)
        {
            //_outline.enabled = false;
            
            GameManager.Instance.currentCamera.gameObject.GetComponent<PlayerCam>().RemoveLookAtTransform(CameraLookAt);
        }
        
    }

    private IEnumerator UninteractableForSeconds(float duration)
    {
        isInteractable = false;
        yield return new WaitForSeconds(duration);
        isInteractable = true;
    }

    
}
