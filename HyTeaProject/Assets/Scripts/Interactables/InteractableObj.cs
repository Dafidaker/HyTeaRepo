using UnityEngine;

public class InteractableObj : Interactable
{
    [field: SerializeField] private string InteractedMsg;
    
    public override void Interact()
    {
        Debug.Log(InteractedMsg);
    }
}
