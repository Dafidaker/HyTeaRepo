using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance != null) return _instance;
            
            _instance = (T)FindObjectOfType(typeof(T));

            if (FindObjectsOfType(typeof(T)).Length > 1)
            {
                return _instance;
            }

            if (_instance != null) return _instance;
            
            GameObject singleton = new GameObject();
            _instance = singleton.AddComponent<T>();
            singleton.name =  typeof(T) + " Singleton" ;

            if (!_instance.gameObject.scene.name.Equals("DontDestroyOnLoad"))
            {
                DontDestroyOnLoad(_instance.gameObject);
            }
            
            return _instance;
            
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = (T)FindObjectOfType(typeof(T));
        }
        
        if (!_instance.gameObject.scene.name.Equals("DontDestroyOnLoad"))
        {
            DontDestroyOnLoad(_instance.gameObject);
        }
    }
}
