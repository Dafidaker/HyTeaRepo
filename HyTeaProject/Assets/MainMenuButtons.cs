using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ClickStart()
    {
        MSceneManager.Instance.FadeToScene("02_Introduction");
    }
    
    public void ClickOptions()
    {
        
    }
    
    public void ClickQuit()
    {
        Application.Quit();
    }
}
