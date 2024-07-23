using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishButton : VirtualButton
{
    public override void OnClick()
    {
        UIManager.Instance.ClickedFinishButton();
    }
}
