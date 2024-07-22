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

    protected override void Awake()
    {
        base.Awake();
        activeCanvas ??= new List<Canvas>();
    }

    public void DeactivateCanvas(Canvas canvas)
    {
        canvas.gameObject.SetActive(false);
    }
    
    public void ActivateCanvas(Canvas canvas)
    {
        canvas.gameObject.SetActive(true);
    }
    
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
        if (PresentationManager.Instance.isCalibrationDone)
        {
            GameManager.Instance.SetGameState(GameState.Walking);
            SceneManager.LoadScene(1);
        }
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

    public Canvas CreateDialogueCanvas(Canvas DialogueCanvas = null)
    {
        if (DialogueCanvas == null) DialogueCanvas = dialogueCanvas;

        var temp = Instantiate(DialogueCanvas, baseCanvas == null ? transform : baseCanvas.transform);
        
        DeactivateCanvas(temp);
        
        //activeCanvas.Add(temp);

        //var component = temp.gameObject.GetComponent<DialogueUIController>();

        return temp;
    }

    public DialogueUIController GetDialogueUIController(Canvas DialogueCanvas)
    {
        var component = DialogueCanvas.gameObject.GetComponent<DialogueUIController>();

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
    
    public IEnumerator CheckForSpacePress()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                EventManager.SpaceWasPressed.Invoke();
            }
            yield return null;
        }
    }

    public void ShowActiveCanvas(bool value)
    {
        activeCanvas[^1].gameObject.SetActive(value);
    }
}
