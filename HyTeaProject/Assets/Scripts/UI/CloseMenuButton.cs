using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CloseMenuButton : VirtualButton
{
    [field: SerializeField] private Canvas canvas;
    public override void OnClick()
    {
        UIManager.Instance.CloseCanvas(canvas);
    }
}
