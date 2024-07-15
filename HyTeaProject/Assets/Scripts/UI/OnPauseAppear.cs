using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OnPauseAppear : MonoBehaviour
{
    [field: SerializeField] private TextMeshProUGUI text;


    private void OnEnable()
    {
        EventManager.PauseStarted.AddListener(EnableText);
        EventManager.OnPauseDone.AddListener((n ) => DisableText());
    }
    
    private void OnDisable()
    {
        EventManager.PauseStarted.RemoveListener(EnableText);
        EventManager.OnPauseDone.AddListener((n ) => DisableText());
    }

    private void EnableText()
    {
        text.enabled = true;
    }

    private void DisableText()
    {
        text.enabled = false;
    }
}
