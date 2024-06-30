using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaybackButton : VirtualButton
{
    private Toggle toggle;

    private void Start()
    {
        toggle = GetComponent<Toggle>();
        EventManager.ClickedPlaybackButton.Invoke(toggle.isOn);
    }

    public override void OnClick()
    {
        EventManager.ClickedPlaybackButton.Invoke(toggle.isOn);
    }
}
