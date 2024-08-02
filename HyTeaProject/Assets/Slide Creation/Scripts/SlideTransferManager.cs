using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlideTransferManager : MonoBehaviour
{
    public static SlideTransferManager Instance { get; private set; }

    public List<GameObject> gameObjectsToPersist; // List of GameObjects to persist

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            foreach (GameObject obj in gameObjectsToPersist)
            {
                DontDestroyOnLoad(obj);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
