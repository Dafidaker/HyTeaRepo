using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Serialization;

public class UIManager : Singleton<UIManager>
{
    [field: SerializeField] private Canvas BaseCanvas;
    [field: SerializeField] private List<Canvas> activeCanvas;
    [field: SerializeField] private Canvas configureMicrohphoneCanvas;


    public void CloseCanvas(Canvas canvas)
    {
        activeCanvas.Remove(canvas);
        Destroy(canvas.gameObject);

        if (activeCanvas.Count > 0)
        {
            activeCanvas[^1].gameObject.SetActive(true);
        }
    }
    
    
    public void ClickedStartButton()
    {
        
    }
    
    public void ClickedOptionsButton()
    {
        
    }
    
    public void ClickedConfigureMicrophone()
    {
        foreach (var canva in activeCanvas)
        {
            canva.gameObject.SetActive(false);
        }
        
        activeCanvas.Add(Instantiate(configureMicrohphoneCanvas, BaseCanvas.transform));
    }
    
    public void ClickedExitButton()
    {
        
    }
}
