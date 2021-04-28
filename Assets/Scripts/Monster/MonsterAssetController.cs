using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterId
{
    Boxkun = 0,
    Aves,
    Cear,
    Bobfish,
    Eyench,
    Pengy,
    Mellow,
    Caffler,
    Turty,
    Wellwhale
}
public class MonsterAssetController : MonoBehaviour
{
    private static MonsterAssetController _instance;
    public static MonsterAssetController Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("MonsterAssetController");
                MonsterAssetController comp = obj.AddComponent<MonsterAssetController>();
                _instance = comp;
            }
            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        isInit = false;
        DontDestroyOnLoad(this.gameObject);
    }

    private List<MonsterController> modelPool;
    private bool isInit;
    public void Init()
    {
        if (!isInit)
        {
            modelPool = new List<MonsterController>();
            LoadData();
            isInit = true;
        }
    }
    private void LoadData()
    {
        modelPool = new List<MonsterController>();
        foreach(Object a in Resources.LoadAll("Models"))
        {
            GameObject b = (GameObject)a;
            if (!b.CompareTag("Monster")) continue;
            modelPool.Add(b.GetComponent<MonsterController>());
        }
        
    }
    public MonsterController GetModelById(string id)
    {
        MonsterController model = null;
        foreach(MonsterController a in modelPool)
        {
            if(a.Id == id)
            {
                model = a;
                break;
            }
        }
        return model;
    }
    public Color GetColorById(string id)
    {
        if (ColorUtility.TryParseHtmlString(id, out Color newCol))
            return newCol;
        else
            return Color.white;
    }
    public void CreateNewMonster(string uid, string name, MainMenuController con)
    {
        MonsterAsset asset = new MonsterAsset("0", "0");
        CreatingMonster tmp = new CreatingMonster(uid, name, DataManager.Instance.MonsterAssetToString(asset));
        StartCoroutine(DataManager.Instance.CreateMonster(tmp, con));
    }
}
