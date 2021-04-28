using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenuIconPrefabs;
    [SerializeField]
    private GameObject monsterHolder;
    [SerializeField]
    private GameObject LoginPanel, loginLoadPanel, monsterPanel, monsterLoadPanel;
    [SerializeField]
    private TextMeshProUGUI userText, passText, monsterNameText;
    [SerializeField]
    private TextMeshProUGUI loginHeader;
    [SerializeField]
    private TextMeshProUGUI uidDisplayText;
    [SerializeField]
    private List<Sprite> monsterImages;
    [SerializeField]
    private GameObject loginLoadingIconImage, monsterLoadingIconImage;

    private readonly int maximumMonster = 12;
    private List<GameObject> monstersGrid;
    private int currentSelectedMonster;
    private void Start()
    {
        OpenLoginPanel();
        monstersGrid = new List<GameObject>();
        currentSelectedMonster = 0;
    }
    private void Update()
    {
        if (loginLoadPanel == true)
        {
            loginLoadingIconImage.transform.Rotate(new Vector3(0, 0, -10));
        }
        if (monsterLoadPanel == true)
        {
            monsterLoadingIconImage.transform.Rotate(new Vector3(0, 0, -10));
        }
    }
    public void GetAllMonsterData()
    {
        StartCoroutine(DataManager.Instance.GetMonsterData(PlayerController.Instance.Uid, this));
        StartCoroutine(DataManager.Instance.GetWalletData(PlayerController.Instance.Uid));
    }
    public void LoadAllMonster()
    {
        MonsterPool a = DataManager.Instance.GetMonsterPool();
        foreach(GameObject b in monstersGrid)
        {
            Destroy(b);
        }
        monstersGrid.Clear();
        for (int i = 0; i < PlayerController.Instance.MonsterAmount; i++)
        {
            if (i > maximumMonster) return;
            MonsterRawData tmp = a.GetMonsterByNumber(i);
            GameObject obj = Instantiate(mainMenuIconPrefabs, monsterHolder.transform);
            monstersGrid.Add(obj);
            obj.transform.Find("MonsterName").GetComponent<TextMeshProUGUI>().text = tmp.name;
            obj.transform.Find("MonsterImage").GetComponent<Image>().sprite = monsterImages[int.Parse(DataManager.Instance.StringToMonsteAsset(tmp.asset).model)];
            int num = i;
            obj.GetComponent<Button>().onClick.AddListener(delegate { OnSelectMonster(num); });
        }
        
    }
    public void OnSelectMonster(int number)
    {
        currentSelectedMonster = number;
    }
    public void ChangeToArScene()
    {
        if(monstersGrid.Count > 0)
        {
            PlayerController.Instance.CurrentMonsterId = DataManager.Instance.GetMonsterPool().GetMonsterByNumber(currentSelectedMonster).id;
            SceneManager.LoadScene("ArScene", LoadSceneMode.Single);
        }        
    }
    public void OpenLoginPanel()
    {
        if(PlayerController.Instance != null && PlayerController.Instance.Uid != null && PlayerController.Instance.Uid.Length > 0)
        {
            GetAllMonsterData();
        }
        else
        {
            LoginPanel.SetActive(true);
            loginLoadPanel.SetActive(false);
            loginHeader.text = "Login";
            loginLoadingIconImage.transform.Rotate(new Vector3(0, 0, 0));
        }
    }
    public void CloseLoginPanel()
    {
        LoginPanel.SetActive(false);
    }
    public void OpenLoginLoading()
    {
        loginLoadPanel.SetActive(true);

    }
    public void OpenCreateMonPanel()
    {
        monsterPanel.SetActive(true);
    }
    public void CloseCreateMonPanel()
    {
        monsterPanel.SetActive(false);
    }
    public void OpenCreateMonLoadPanel()
    {
        monsterLoadPanel.SetActive(true);
    }
    public void SwitchLoginMode()
    {
        if(loginHeader.text == "Login")
        {
            loginHeader.text = "Sign up";
        }
        else
        {
            loginHeader.text = "Login";
        }
    }
    public void OnConfirmLogin()
    {
        if(userText.text.Length>0 && passText.text.Length > 0)
        {
            SignInForm data = new SignInForm(userText.text, passText.text);
            string type;
            if (loginHeader.text == "Login")
            {
                type = "login";
            }
            else
            {
                type = "register";
            }
            StartCoroutine(DataManager.Instance.SignIn(type, data, this));
            OpenLoginLoading();
        }
    }
    public void OnFinishSignIn(string res)
    {
        string uid = JsonUtility.FromJson<SignInResponse>(res).id;
        PlayerController.Instance.Uid = uid;
        ShowCurrentUid();
        GetAllMonsterData();
        CloseLoginPanel();
    }
    public void OnConfirmCreateMonster()
    {
        string id = PlayerController.Instance.Uid;
        MonsterAssetController.Instance.Init();
        MonsterAssetController.Instance.CreateNewMonster(id, monsterNameText.text, this);
        OpenCreateMonLoadPanel();
    }
    public void OnCancelCreateMonster()
    {
        CloseCreateMonPanel();
    }
    public void OnFinishCreateMonster()
    {
        GetAllMonsterData();
        CloseCreateMonPanel();
    }
    public void ShowCurrentUid()
    {
        uidDisplayText.text = "Uid: " + PlayerController.Instance.Uid;
    }
}
