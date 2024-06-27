using UnityEngine;

public class InteractableObj : Interactable
{

    [field: SerializeField] private string InteractedMsg;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public override void Interact()
    {
        Debug.Log(InteractedMsg);
    }
}
