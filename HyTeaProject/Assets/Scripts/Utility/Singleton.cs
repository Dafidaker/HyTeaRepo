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

            DontDestroyOnLoad(singleton);

            return _instance;
            
        }
    }

    private void OnDestroy()
    {
        
    }
}
