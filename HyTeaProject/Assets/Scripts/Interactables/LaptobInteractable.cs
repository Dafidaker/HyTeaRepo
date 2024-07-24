using System;
using Unity.VisualScripting;
using UnityEngine;

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

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (GameManager.Instance.GameState == GameState.PrePresentation)
            {
                GameManager.Instance.StartPresentation(null);
            }
            else if (GameManager.Instance.GameState == GameState.Presentation)
            {
                GameManager.Instance.EndPresentation();
            }
        }
    }*/
}
