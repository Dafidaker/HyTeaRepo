using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaybackButton : VirtualButton
{
    [field: SerializeField] private Toggle toggle;

    private void Start()
    {
        EventManager.ClickedPlaybackButton.Invoke(toggle.isOn);
    }

    public override void OnClick()
    {
        EventManager.ClickedPlaybackButton.Invoke(toggle.isOn);
    }
}
