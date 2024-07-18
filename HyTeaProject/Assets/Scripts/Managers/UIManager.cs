using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    [field: SerializeField] private Canvas baseCanvas;
    [field: SerializeField] private List<Canvas> activeCanvas;
    [field: SerializeField] private Canvas configureMicrohphoneCanvas;
    [field: SerializeField] private Canvas dialogueCanvas;
    [field: SerializeField] private GameObject baseCanvasPrefab;
    

    public void CloseCanvas(Canvas canvas)
    {
        activeCanvas.Remove(canvas);
        Destroy(canvas.gameObject);

        if (activeCanvas.Count > 0)
        {
            activeCanvas[^1].gameObject.SetActive(true);
        }
    }
    
    public void CloseUpperCanvas()
    {
        if (activeCanvas.Count <= 0) return;
        
        Canvas temp = activeCanvas[^1];
        activeCanvas.Remove(temp);
        Destroy(temp.gameObject);

        if (activeCanvas.Count > 0)
        {
            activeCanvas[^1].gameObject.SetActive(true);
        }
    }
    
    public void ClickedStartButton()
    {
        SceneManager.LoadScene(1);
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
        
        activeCanvas.Add(Instantiate(configureMicrohphoneCanvas, baseCanvas.transform));
    }
    
    public void ClickedExitButton()
    {
        
    }

    public DialogueUIController CreateDialogueCanvas()
    {
        var temp = Instantiate(dialogueCanvas, baseCanvas.transform);
        
        activeCanvas.Add(temp);

        var component = temp.gameObject.GetComponent<DialogueUIController>();

        return component;
    }
    
    
    public IEnumerator CheckForMousePress()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                EventManager.MouseWasPressed.Invoke();
            }
            yield return null;
        }
    }

    public void ShowActiveCanvas(bool value)
    {
        activeCanvas[^1].gameObject.SetActive(value);
    }
}
