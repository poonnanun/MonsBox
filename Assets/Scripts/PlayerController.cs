using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static PlayerController _instance;
    public static PlayerController Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("PlayerController");
                PlayerController comp = obj.AddComponent<PlayerController>();
                _instance = comp;
            }
            return _instance;
        }
    }

    private string currentMonsterId;
    private string uid;

    private int currentGold;
    private int monsterAmount;
    private WalletRawData currentWallet;

    public string CurrentMonsterId { get => currentMonsterId; set => currentMonsterId = value; }
    public string Uid { get => uid; set => uid = value; }
    public int CurrentGold
    {
        get => currentGold; set
        {
            currentGold = value;
            if(ArSceneController.Instance!=null)
                ArSceneController.Instance.SetPlayerCurrency(currentGold);
        }
    }
    public int MonsterAmount { get => monsterAmount; set => monsterAmount = value; }
    public WalletRawData CurrentWallet
    {
        get => currentWallet; 
        set
        {
            currentWallet = value;
            currentGold = currentWallet.currencies[0].value;
        }
    }
    public void OnUseMoney(int amount)
    {
        CurrentGold -= amount;
    }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

}
