using System.Collections.Generic;
using UnityEngine;

public class NotificationProvider : MonoBehaviour
{
    public static NotificationProvider Instance { get; private set; }
    [SerializeField] private  GameObject _notificationPrefab;
    [SerializeField] private  Transform _notificationParent;
    [SerializeField] private  float _notificationHeight;

    private readonly List<Notification> _activeNotifications = new List<Notification>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Notification.OnNotificationDisable += RemoveNotification;
    }

    private void OnDestroy()
    {
        Notification.OnNotificationDisable -= RemoveNotification;
    }

    public void SpawnNotification(string text, float lifeTime)
    {
        var notification = Instantiate(_notificationPrefab, _notificationParent);
        _activeNotifications.Add(notification.GetComponent<Notification>());
        _activeNotifications[^1].Initialize(text, lifeTime);
        SetNotificationPos(notification.transform);
    }

    private void SetNotificationPos(Transform notificationTransform)
    {
        notificationTransform.localPosition = new Vector3(0, - _notificationHeight * (_activeNotifications.Count - 1), 0);
    }

    private void RemoveNotification(Notification notification)
    {
        _activeNotifications.Remove(notification);
        Destroy(notification.gameObject);
    }
}
