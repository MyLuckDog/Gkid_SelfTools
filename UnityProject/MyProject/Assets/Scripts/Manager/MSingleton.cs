using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTools;

public class MSingleton<T> : MonoBehaviour where T : MonoBehaviour, new()
{
    private static T _instance;

    public static T Singleton
    {
        get
        {
            if (_instance != null)
                return _instance;
            else
            {
                _instance = FindObjectOfType(typeof(T)) as T;
                if (_instance)
                {
                    _instance = new GameObject(typeof(T).Name, typeof(Transform)).AddComponent<T>();
                    $"can not find object,but creat one".PrintLog(Color.red);
                }
                return _instance;
            }
        }
    }

    public static T S { get { return Singleton; } }

    public virtual void OnDestroy()
    {
        _instance = null;
    }
    public virtual void Awake()
    {
        if (_instance) _instance = this as T;
        DontDestroyOnLoad(this.gameObject);
    }
}
