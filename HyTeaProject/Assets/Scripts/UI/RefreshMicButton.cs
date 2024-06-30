using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshMicButton : VirtualButton
{
    public override void OnClick()
    {
        EventManager.PopulateMicrophoneList.Invoke();
    }
}
