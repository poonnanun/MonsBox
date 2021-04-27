using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class DataManager : MonoBehaviour
{
    private static DataManager _instance;
    public static DataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("EquipmentPoolManager");
                DataManager comp = obj.AddComponent<DataManager>();
                _instance = comp;
            }
            return _instance;
        }
    }

    public bool IsDataLoaded { get => isDataLoaded; set => isDataLoaded = value; }

    private string url;
    private bool isDataLoaded;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        GetUrl();
        DontDestroyOnLoad(this.gameObject);
    }
    private void Start()
    {
        StartCoroutine(GetItemData());
    }
    private void GetUrl()
    {
        url = "https://monsbox-backend-e64sflkoha-as.a.run.app/";
    }
    private MonsterPool monsterPool;
    public IEnumerator GetMonsterData(string uid, MainMenuController con)
    {
        string path = "monster";
        if(uid.Length > 0)
        {
            path += "?userId=" + uid;
        }
        using (UnityWebRequest req = UnityWebRequest.Get(url + path))
        {
            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.LogError(req.error + " ( " + (url + path) + " )");
            }
            else
            {
                monsterPool = JsonUtility.FromJson<MonsterPool>(req.downloadHandler.text);
                PlayerController.Instance.MonsterAmount = monsterPool.size;
                con.LoadAllMonster();
            }
        }
    }
    public IEnumerator GetMonsterDataFromId(string id, MonsterController con)
    {
        string path = "monster";
        if (id.Length > 0)
        {
            path += "/" + id;
        }
        using (UnityWebRequest req = UnityWebRequest.Get(url + path))
        {
            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.LogError(req.error + " ( " + (url + path) + " )");
            }
            else
            {
                monsterPool = JsonUtility.FromJson<MonsterPool>(req.downloadHandler.text);
                con.LoadData();
            }
        }
    }
    private ItemPool itemPool;
    public IEnumerator GetItemData()
    {
        string path = "item";
        using (UnityWebRequest req = UnityWebRequest.Get(url + path))
        {
            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.LogError(req.error + " ( " + (url + path) + " )");
            }
            else
            {
                itemPool = JsonUtility.FromJson<ItemPool>(req.downloadHandler.text);
            }
        }
    }
    private WalletPool walletPool;
    public IEnumerator GetWalletData(string uid)
    {
        string path = "wallet";
        if (uid.Length > 0)
        {
            path += "?userId=" + uid;
        }
        using (UnityWebRequest req = UnityWebRequest.Get(url + path))
        {
            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.LogError(req.error + " ( " + (url + path) + " )");
            }
            else
            {
                walletPool = JsonUtility.FromJson<WalletPool>(req.downloadHandler.text);
                PlayerController.Instance.CurrentWallet = walletPool.GetFirstWallet();
            }
        }
    }
    public ItemPool GetItemPool()
    {
        return itemPool;
    }
    public ItemRawData GetItemById(string id)
    {
        return itemPool.GetItemById(id);
    }
    public WalletPool GetWalletPool()
    {
        return walletPool;
    }
    public WalletRawData GetFirstWallet()
    {
        return walletPool.GetFirstWallet();
    }
    public MonsterPool GetMonsterPool()
    {
        return monsterPool;
    }
    public MonsterRawData GetMonsterById(string id)
    {
        return monsterPool.GetMonsterById(id);
    }
    public string MonsterAssetToString(MonsterAsset asset)
    {
        return JsonUtility.ToJson(asset);
    }
    public MonsterAsset StringToMonsteAsset(string str)
    {
        return JsonUtility.FromJson<MonsterAsset>(str);
    }
    public IEnumerator UpdateWallet(string id, int amount)
    {
        string path = "monster";
        WWWForm form = new WWWForm();
        using (UnityWebRequest req = UnityWebRequest.Post(url + path, form))
        {
            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.LogError(req.error + " ( " + (url + path) + " )");
            }
            else
            {
                Debug.Log("finish : " + req.downloadHandler.text);
            }
        }
    }
    public IEnumerator CreateMonster(CreatingMonster sendData, MainMenuController con)
    {
        string path = "monster";
        WWWForm form = new WWWForm();
        form.AddField("userId", sendData.userId);
        form.AddField("name", sendData.name);
        form.AddField("asset", sendData.asset);
        using (UnityWebRequest req = UnityWebRequest.Post(url + path, form))
        {
            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.LogError(req.error + " ( " + (url + path) + " )");
            }
            else
            {
                Debug.Log("finish : " + req.downloadHandler.text);
                con.OnFinishCreateMonster();
            }
        }
    }
    public IEnumerator FinishEating(int amount, MonsterController con)
    {
        string path = "monster/";

        MonsterCare sendData = new MonsterCare(new MonsterCareActivities("hungry", amount), amount);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(sendData));
        WWWForm form = new WWWForm();
        form.AddField("activities", JsonUtility.ToJson(new MonsterCareActivities("hungry",amount)));
        form.AddField("experience", amount);
        Debug.Log(form);
        string finalPath = url + path + PlayerController.Instance.CurrentMonsterId + "/care";
        using (UnityWebRequest req = UnityWebRequest.Put(finalPath, JsonUtility.ToJson(sendData)))
        {
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.LogError(req.error + " ( " + finalPath + " )");
            }
            else
            {
                Debug.Log("Complete");
                con.RefreshData();
            }
        }
    }
    public IEnumerator Evolve(MonsterController con)
    {
        string path = "monster/";

        WWWForm form = new WWWForm();
        form.AddField("userId", PlayerController.Instance.Uid);
        form.AddField("name", con.Name);
        form.AddField("asset", MonsterAssetToString(con.MonsterAsset));
        string finalPath = url + path + PlayerController.Instance.CurrentMonsterId + "/evolve";
        using (UnityWebRequest req = UnityWebRequest.Post(finalPath, form))
        {
            req.method = "PUT";
            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.LogError(req.error + " ( " + finalPath + " )");
            }
            else
            {
                Debug.Log("Complete");
                con.RefreshData();
            }
        }
    }
    public IEnumerator SignIn(string type, SignInForm sendData, MainMenuController con)
    {
        string path = "user";
        WWWForm form = new WWWForm();
        form.AddField("username", sendData.username);
        form.AddField("password", sendData.password);
        string finalUrl = url + path + "/" + type;
        using (UnityWebRequest req = UnityWebRequest.Post(finalUrl, form))
        {
            yield return req.SendWebRequest();

            if (req.isNetworkError || req.isHttpError)
            {
                Debug.LogError(req.error + " ( " + finalUrl + " )");
                
            }
            else
            {
                con.OnFinishSignIn(req.downloadHandler.text);
            }
        }
    }
}
