using System;
using UnityEngine;

[Serializable]
public class PresentationStartSettings
{
    public Transform lookAtTransform;
    public Transform playerPositionTransform;
}


public class PresentationStartTriggerController : MonoBehaviour
{
    [SerializeField] private PresentationStartSettings _settings;
    public Transform arrow;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.StartPresentation(_settings);
            gameObject.SetActive(false);
            if (arrow != null) arrow.gameObject.SetActive(false);   
        }
    }
}
