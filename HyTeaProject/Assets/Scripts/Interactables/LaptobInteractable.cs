public class LaptobInteractable : Interactable
{
    public override void Interact()
    {
        if (GameManager.Instance.GameState == GameState.PrePresentation)
        {
            GameManager.Instance.StartPresentation(null);
        }
        else
        {
            GameManager.Instance.EndPresentation();
        }
        
        //EventManager.ChangeToNextSlide.Invoke();
    }
    
}
