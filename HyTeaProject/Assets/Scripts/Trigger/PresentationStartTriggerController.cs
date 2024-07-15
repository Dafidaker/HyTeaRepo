using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentationStartTriggerController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PresentationManager.Instance.StartPresentation();
            gameObject.SetActive(false);
        }
    }
}
