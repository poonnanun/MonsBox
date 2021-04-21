using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterActivity
{
    Idle,
    Wander,
    FindFood,
    Eating,
    FindBath,
    Bathing
}
public class MonsterController : MonoBehaviour
{
    [SerializeField]
    private float walkSoundThreshold;

    private MonsterActivity _currentActivity;
    private Vector3 moveTarget;
    private float moveSpeed, turnSpeed;
    private float hungrinessTickSpeed, cleanlinessTickSpeed, happinessTickSpeed;
    private float hungrinessCount, cleanlinessCount, happinessCount;
    private float currentWalkDist;

    private int hungriness, maxHungriness;
    private int cleanliess, maxCleanliness;
    private int happiness, maxHappiness;
    private int overEat;

    private bool isFinishBathing;
    private readonly int rubThreshold = 10;
    private int rubCount;

    private Rigidbody rb;
    public int Hungriness { get => hungriness; set => hungriness = value; }
    public int MaxHungriness { get => maxHungriness; set => maxHungriness = value; }
    public int Cleanliness { get => cleanliess; set => cleanliess = value; }
    public int MaxCleanliness { get => maxCleanliness; set => maxCleanliness = value; }
    public int Happiness { get => happiness; set => happiness = value; }
    public int MaxHappiness { get => maxHappiness; set => maxHappiness = value; }

    void Start()
    {
        _currentActivity = MonsterActivity.Idle;
        moveSpeed = 0.5f;
        turnSpeed = 5f;
        rubCount = 0;

        hungrinessCount = cleanlinessCount = 0;
        rb = GetComponent<Rigidbody>();
        LoadData();
    }

    void Update()
    {
        #region Decrease per tick
        if (hungrinessCount >= hungrinessTickSpeed)
        {
            if(Hungriness > 0)
            {
                Hungriness--;
                if (Hungriness % 5 == 0)
                {
                    ArSceneController.Instance.SetHungriness(Hungriness, MaxHungriness);
                }
                hungrinessCount = 0;
            }
            else
            {
                // very hungry
            }
        }
        if (cleanlinessCount >= cleanlinessTickSpeed)
        {
            if (Cleanliness > 0)
            {
                Cleanliness--;
                if (Cleanliness % 5 == 0)
                {
                    ArSceneController.Instance.SetCleanliness(Cleanliness, MaxCleanliness);
                }
                cleanlinessCount = 0;
            }
            else
            {
                // very dirty
            }
        }
        if (happinessCount >= happinessTickSpeed)
        {
            if (Happiness > 0)
            {
                Happiness--;
                if(Happiness % 5 == 0)
                {
                    ArSceneController.Instance.SetHappiness(Happiness, MaxHappiness);
                }
                happinessCount = 0;
            }
            else
            {
                // very sad
            }
        }
        hungrinessCount += Time.deltaTime;
        cleanlinessCount += Time.deltaTime;
        happinessCount += Time.deltaTime;
        #endregion
        #region Idle
        if (_currentActivity == MonsterActivity.Idle)
        {
            //ArSceneController.Instance.SetNameText("IDLE");
            ResetVariable();
        }
        #endregion
        #region Wander
        else if (_currentActivity == MonsterActivity.Wander)
        {
            //ArSceneController.Instance.SetNameText("WANDER");
        }
        #endregion
        #region FindFood
        else if (_currentActivity == MonsterActivity.FindFood)
        {
            //ArSceneController.Instance.SetNameText("FINDFOOD");
            if (moveTarget != null)
            {
                Vector3 before = gameObject.transform.position;
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, moveTarget, Time.deltaTime * moveSpeed);
                Vector3 after = gameObject.transform.position;
                gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.LookRotation(moveTarget - gameObject.transform.position), Time.deltaTime * turnSpeed);
                currentWalkDist += Vector3.Distance(before, after);
                if(currentWalkDist >= walkSoundThreshold)
                {
                    SoundManager.Instance.PlayWalkSound();
                }
                if (Vector3.Distance(gameObject.transform.position, moveTarget) < 0.17f)
                {
                    ChanceActivity(MonsterActivity.Eating);
                    return;
                }
            }
        }
        #endregion
        #region Eating
        else if (_currentActivity == MonsterActivity.Eating)
        {
            // play animation here
            //ArSceneController.Instance.SetNameText("EATING");
            FoodScript food = ArSceneController.Instance.GetCurrentFood();
            OnEat(food.FullnessAmount);
            ArSceneController.Instance.DestroyCurrentFood();
            ChanceActivity(MonsterActivity.Idle);
            return;
        }
        #endregion
        #region Find bath
        else if (_currentActivity == MonsterActivity.FindBath)
        {
            if (moveTarget != null)
            {
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, moveTarget, Time.deltaTime * moveSpeed);
                gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.LookRotation(moveTarget - gameObject.transform.position), Time.deltaTime * turnSpeed);
            }
        }
        #endregion
        #region Bathing
        else if (_currentActivity == MonsterActivity.Bathing)
        {
            if (isFinishBathing)
            {
                ChanceActivity(MonsterActivity.Idle);
            }
        }
        #endregion
    }
    public void OnArrivedAtBath()
    {
        isFinishBathing = false;
        ChanceActivity(MonsterActivity.Bathing);
    }
    public void ResetVariable()
    {
        isFinishBathing = false;
    }
    public void ChanceActivity(MonsterActivity act)
    {
        _currentActivity = act;
    }
    public void SetKinematic(bool boo)
    {
        rb.isKinematic = boo;
    }
    public void MoveTo(Vector3 pos)
    {
        Debug.Log("Move to : " + pos);
        moveTarget = new Vector3(pos.x, gameObject.transform.position.y, pos.z);
    }
    public void AddHappiness()
    {
        rubCount++;
        if(rubCount == rubThreshold)
        {
            rubCount = 0;
            Happiness++;
            ArSceneController.Instance.SetHappiness(Happiness, MaxHappiness);
        }
    }
    public void SetIsFinishBathing(bool boo)
    {
        isFinishBathing = boo;
    }
    public void OnEat(int amount)
    {
        Hungriness += amount;
        if(Hungriness > MaxHungriness)
        {
            overEat += Hungriness - MaxHungriness;
            Hungriness = MaxHungriness;
        }
        ArSceneController.Instance.SetHungriness(Hungriness, MaxHungriness);
    }
    public void LoadData()
    {
        MaxHungriness = 100;
        MaxCleanliness = 100;
        MaxHappiness = 100;
        Hungriness = MaxHungriness;
        Cleanliness = MaxCleanliness;
        Happiness = MaxHappiness;
        overEat = 0;
        hungrinessTickSpeed = 1f;
        cleanlinessTickSpeed = 10f;
        happinessTickSpeed = 1f;
    }
}
