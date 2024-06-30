using System;
using TMPro;
using UnityEngine;

public class Notification : MonoBehaviour
{
    private TMP_Text _notificationText;
    private float _lifeTime;
    private float _spawnTime;
    private bool _activated;

    public static event Action<Notification> OnNotificationDisable; 

    public void Initialize(string text, float lifeTime)
    {
        _notificationText = GetComponentInChildren<TMP_Text>();
        _notificationText.text = text;
        _lifeTime = lifeTime;
        _spawnTime = Time.time;
        _activated = true;
    }

    private void Update()
    {
        if (! _activated) return;
        if (HasLifeTimeRunOut())
        {
            Deactivate();
        }
    }

    private bool HasLifeTimeRunOut()
    {
        return Time.time - _spawnTime > _lifeTime;
    }

    private void Deactivate()
    {
        OnNotificationDisable.Invoke(this);
        gameObject.SetActive(false);
    }
}
