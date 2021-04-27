using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinigameController : MonoBehaviour
{
    public static MinigameController Instance;
    [SerializeField]
    private GameObject blackpanel;
    [SerializeField]
    private GameObject ResultPanel;
    [SerializeField]
    private TextMeshProUGUI CountdownText;
    [SerializeField]
    private TextMeshProUGUI scoreText, highScoreText, coinReceiveText;
    [SerializeField]
    private List<MinigameBase> allMinigames;

    private MinigameBase currentMinigame;
    private bool isCountdown;
    private float countdown;

    private void Awake()
    {
        Instance = this;
        ClosePanel();
    }
    public void ClosePanel()
    {
        blackpanel.SetActive(false);
        foreach (MinigameBase a in allMinigames)
        {
            a.gameObject.SetActive(false);
        }
    }
    public void SelectMinigame(string minigameId)
    {
        foreach(MinigameBase a in allMinigames)
        {
            if(minigameId == a.MinigameId)
            {
                currentMinigame = a;
                CountdownStart();
                break;
            }
        }
    }
    public void CountdownStart()
    {
        currentMinigame.gameObject.SetActive(true);
        currentMinigame.OnSetUp();
        OnStartMinigame();
    }
    public void OnStartMinigame()
    {
        if (currentMinigame.IsFinishSetup)
        {
            blackpanel.SetActive(false);
            ResultPanel.SetActive(false);
            currentMinigame.OnStart();
        }
    }
    public void DisplayResultText()
    {
        blackpanel.SetActive(true);
        ResultPanel.SetActive(true);
        scoreText.text = currentMinigame.Score.ToString();
        highScoreText.text = currentMinigame.HighScore.ToString();
        coinReceiveText.text = currentMinigame.CoinReceieved.ToString();
    }
    public void BackToMain()
    {
        ArSceneController.Instance.OnFinishMinigame();
    }
    public void OnFinish()
    {
        DisplayResultText();
        // TODO send monster care request
        currentMinigame.gameObject.SetActive(false);
    }
    public List<MinigameBase> GetMinigameList()
    {
        return allMinigames;
    }
}
