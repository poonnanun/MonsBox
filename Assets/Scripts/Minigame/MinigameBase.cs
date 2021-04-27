using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinigameBase : MonoBehaviour
{
    [SerializeField]
    private string minigameId;
    [SerializeField]
    private string minigameName;
    [SerializeField]
    private Sprite image;
    [SerializeField]
    private int scoreMultiplication;
    [SerializeField]
    private int timelimit;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI timeText;
    [SerializeField]
    private Button backButton;

    private int score;
    private int highScore;
    private float currentTime;
    private bool isFinished;
    private bool isFinishSetup;
    private bool isStarted;

    public string MinigameId { get => minigameId; set => minigameId = value; }
    public bool IsFinishSetup { get => isFinishSetup; set => isFinishSetup = value; }
    public int Score { get => score; set => score = value; }
    public int HighScore { get => highScore; set => highScore = value; }
    public int CoinReceieved { get => Mathf.RoundToInt(score * scoreMultiplication); }
    public bool IsStarted { get => isStarted; set => isStarted = value; }
    public string MinigameName { get => minigameName; set => minigameName = value; }
    public Sprite Image { get => image; set => image = value; }

    private void Awake()
    {
        currentTime = timelimit;
    }
    private void Update()
    {
        if (IsStarted)
        {
            TimeDecreasing();
            OnEveryFrame();
        }
    }
    public virtual void OnSetUp()
    {
        IsFinishSetup = true;
        IsStarted = false;
        HighScore = GetCurrentHighScore();
    }
    public virtual void OnEveryFrame() { }
    public void OnStart()
    {
        IsStarted = true;
    }
    public virtual void OnFinished()
    {
        IsStarted = false;
        isFinished = true;
        ComputeHighScore();
        MinigameController.Instance.OnFinish();
    }
    public void TimeDecreasing()
    {
        //put this in every update
        if (!isFinished)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                OnFinished();
                isFinished = true;
            }
            else
            {
                timeText.text = Mathf.FloorToInt(currentTime).ToString();
            }
        }
    }
    public void AddScore(int amount)
    {
        Score += amount;
        scoreText.text = Score.ToString();
    }
    public void ComputeHighScore()
    {
        if(Score > HighScore)
        {
            HighScore = Score;
            SetNewHighScore(HighScore);
        }
    }
    public virtual void SetNewHighScore(int amount)
    {

    }
    public virtual int GetCurrentHighScore()
    {
        return 0;
    }
}
