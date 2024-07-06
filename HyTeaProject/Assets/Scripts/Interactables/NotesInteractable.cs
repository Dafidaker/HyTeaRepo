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
        if (obj == this)
        {
            _outline.enabled = true;
            
            PlayerCam playerCam = GameManager.Instance.currentCamera.gameObject.GetComponent<PlayerCam>();
            if (playerCam.cameraIsLocked)
            {
                playerCam.AddLookAtTransform(CameraLookAt);
            }
        }
        
    }

    protected override void StoppedBeingLookedAt(Interactable obj)
    {
        
        if (obj != this)
        {
            _outline.enabled = false;
            
            GameManager.Instance.currentCamera.gameObject.GetComponent<PlayerCam>().RemoveLookAtTransform(CameraLookAt);
        }
        
    }

    
}
