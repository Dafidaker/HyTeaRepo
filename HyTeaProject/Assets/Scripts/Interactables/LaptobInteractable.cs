public class LaptobInteractable : Interactable
{
    public override void Interact()
    {
        EventManager.ChangeToNextSlide.Invoke();
    }
    
}
