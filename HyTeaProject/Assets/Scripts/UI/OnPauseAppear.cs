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
        EventManager.OnPauseStateChanged.AddListener(UpdateVisability);
    }
    
    private void OnDisable()
    {
        EventManager.OnPauseStateChanged.RemoveListener(UpdateVisability);
    }

    private void UpdateVisability(bool pausing)
    {
        text.enabled = pausing;
    }
}
