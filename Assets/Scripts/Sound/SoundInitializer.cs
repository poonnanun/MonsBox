using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundInitializer : MonoBehaviour
{
    private bool isInitialize;
    private static SoundInitializer _instance;
    public static SoundInitializer Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("SoundInitializer");
                SoundInitializer comp = obj.AddComponent<SoundInitializer>();
                _instance = comp;
            }
            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this.gameObject);

    }
    public void Init()
    {
        if (!isInitialize)
        {
            // load data in save fill
            DontDestroyOnLoad(Instantiate(Resources.Load<GameObject>("SoundManager")));
            isInitialize = true;
        }
    }
}
