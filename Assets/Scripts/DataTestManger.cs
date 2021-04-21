using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;

public class DataTestManger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI hungrinessText;
    [SerializeField] private TextMeshProUGUI happinessText;
    [SerializeField] private TextMeshProUGUI cleanlinessText;
    [SerializeField] private Image monsterImage;
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Sprite babyImage;

    private string url;

    // Start is called before the first frame update
    void Start()
    {
        GetURL();
        StartCoroutine(GetData());
    }

    private IEnumerator GetData()
    {
        string path = "monster";
        Debug.Log("Start");
        using (UnityWebRequest req = UnityWebRequest.Get(url+path))
        {
            yield return req.SendWebRequest();

            if(req.isNetworkError || req.isHttpError)
            {
                Debug.Log(req.error + " ( " + (url+path) + " )");
            }
            else
            {
                Debug.Log(req.downloadHandler.text);
                Debug.Log("Success");
                MonsterPool tmp = JsonUtility.FromJson<MonsterPool>(req.downloadHandler.text);
                Debug.Log(tmp.GetMonsterByNumber(0).ToString());
                SetAllData(tmp.GetMonsterByNumber(1));
            }
            
        }
    }
    public void SetAllData(MonsterRawData data)
    {
        //SetName(data.name);
        SetName(data.name);
        SetHungriness(data.status.hungry);
        SetCleanliness(data.status.cleanliness);
        SetHappiness(data.status.happiness);
        SetImage(data.tier);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetName(string name)
    {
        nameText.text = name;
    }
    public void SetHungriness(int value)
    {
        hungrinessText.text = "Hungry: " + value.ToString();
    }
    public void SetCleanliness(int value)
    {
        cleanlinessText.text = "Cleanliness: " + value.ToString();
    }
    public void SetHappiness(int value)
    {
        happinessText.text = "Happiness: " + value.ToString();
    }
    public void SetImage(string value)
    {
        if(value == "BABY")
        {
            monsterImage.sprite = babyImage;
        }
    }
    public void GetURL()
    {
        url = "https://monsbox-backend-e64sflkoha-as.a.run.app/";
    }
}
