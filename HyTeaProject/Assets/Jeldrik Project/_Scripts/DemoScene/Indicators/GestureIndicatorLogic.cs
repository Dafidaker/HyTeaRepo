using UnityEngine;

public class GestureIndicatorLogic : MonoBehaviour
{
    private void Awake()
    {
        MovementAnalysis.OnGestureDetected += ReactToGesture;
    }

    private void OnDestroy()
    {
        MovementAnalysis.OnGestureDetected -= ReactToGesture;
    }

    private void ReactToGesture(GestureHolder gesture)
    {
        
        Debug.Log("gesture done: " + gesture.Name);
        NotificationProvider.Instance.SpawnNotification(gesture.Name, 1.5f);
    }
}
