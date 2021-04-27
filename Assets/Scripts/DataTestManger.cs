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
    private string currMonId = "6087c5b933fe6900252170bf";

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetAllData(MonsterRawData data)
    {
        //SetName(data.name);
        SetName(data.name);
        SetHungriness(data.status.hungry.value);
        SetCleanliness(data.status.cleanliness.value);
        SetHappiness(data.status.happiness.value);
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
