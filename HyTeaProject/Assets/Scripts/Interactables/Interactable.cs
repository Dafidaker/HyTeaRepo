using Imports.QuickOutline.Scripts;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public abstract class Interactable : MonoBehaviour
{
    protected Outline _outline;
    protected bool isInteractable = true;
    protected bool isBeingWatched = false;
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