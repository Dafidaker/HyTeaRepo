using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MoveCam : MonoBehaviour
{
    [field:SerializeField] private Transform cameraTransform;

    private void Update()
    {
        transform.position = cameraTransform.position;
    }
}
