using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using Input = GoogleARCore.InstantPreviewInput;
#endif

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
    [SerializeField]
    private string id;
    [SerializeField]
    private Animator monsterAnimator ;

    private MonsterActivity _currentActivity;
    private Vector3 moveTarget;
    private float moveSpeed, turnSpeed;
    private float currentWalkDist;

    private int hungriness, maxHungriness;
    private int cleanliess, maxCleanliness;
    private int happiness, maxHappiness;

    private bool isFinishBathing;
    private readonly int rubThreshold = 100;
    private int rubCount;

    private MonsterRawData monsterData;
    private MonsterAsset monsterAsset;

    private readonly float LoadDataInteraval = 60f;
    private float loadDataCount;
    private Vector3 idleFaceTo;
    private bool allowLookCam;

    private Rigidbody rb;
    public int Hungriness { get => hungriness; set => hungriness = value; }
    public int MaxHungriness { get => maxHungriness; set => maxHungriness = value; }
    public int Cleanliness { get => cleanliess; set => cleanliess = value; }
    public int MaxCleanliness { get => maxCleanliness; set => maxCleanliness = value; }
    public int Happiness { get => happiness; set => happiness = value; }
    public int MaxHappiness { get => maxHappiness; set => maxHappiness = value; }
    public string Id { get => id; set => id = value; }
    public string Name { get => monsterData.name; }
    public MonsterAsset MonsterAsset { get => monsterAsset; set => monsterAsset = value; }

    void Start()
    {
        _currentActivity = MonsterActivity.Idle;
        moveSpeed = 0.5f;
        turnSpeed = 5f;
        rubCount = 0;
        loadDataCount = 0;

        rb = GetComponent<Rigidbody>();
        if (!GameConfig.isTest)
        {
            LoadData();
        }
        //StartCoroutine(DataManager.Instance.Evolve(this));
        ToIdleAnimation();
    }

    void Update()
    {
        Debug.Log("Monster State : " + _currentActivity.ToString());
        #region Decrease per tick
        //TODO change to pull data from scoket
        loadDataCount += Time.deltaTime;
        if (loadDataCount >= LoadDataInteraval)
        {
            loadDataCount = 0;
            RefreshData();
        }
        #endregion
        #region Idle
        if (_currentActivity == MonsterActivity.Idle)
        {
            //ArSceneController.Instance.SetNameText("IDLE");
            ResetVariable();
            if (allowLookCam)
            {
                gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.LookRotation(idleFaceTo - gameObject.transform.position), Time.deltaTime * turnSpeed);
            }
            
            
        }
        #endregion
        #region Wander
        else if (_currentActivity == MonsterActivity.Wander)
        {
            //ArSceneController.Instance.SetNameText("WANDER");
            if (moveTarget != null)
            {
                if (Vector3.Distance(gameObject.transform.position, moveTarget) < 0.17f)
                {
                    ToIdleAnimation();
                }
                else
                {
                    ToWalkAnimation();
                    Vector3 before = gameObject.transform.position;
                    gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, moveTarget, Time.deltaTime * moveSpeed/2f);
                    Vector3 after = gameObject.transform.position;
                    gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.LookRotation(moveTarget - gameObject.transform.position), Time.deltaTime * turnSpeed);
                    currentWalkDist += Vector3.Distance(before, after);
                    if (currentWalkDist >= walkSoundThreshold)
                    {
                        SoundManager.Instance.PlayWalkSound();
                    }
                }
            }
        }
        #endregion
        #region FindFood
        else if (_currentActivity == MonsterActivity.FindFood)
        {
            //ArSceneController.Instance.SetNameText("FINDFOOD");
            if (moveTarget != null)
            {
                ToWalkAnimation();
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
            ToIdleAnimation();
            ToEat();
            //ArSceneController.Instance.SetNameText("EATING");
            if (!GameConfig.isTest)
            {
                FoodScript food = ArSceneController.Instance.GetCurrentFood();
                //OnEat(food.Effect);
                OnEat(10);
                ArSceneController.Instance.DestroyCurrentFood();
            }
            Invoke("ToIdleAnimation", 1.5f);
            ChanceActivity(MonsterActivity.Idle);
            return;
        }
        #endregion
        #region Find bath
        else if (_currentActivity == MonsterActivity.FindBath)
        {
            if (moveTarget != null)
            {
                ToWalkAnimation();
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
                ToIdleAnimation();
                ChanceActivity(MonsterActivity.Idle);
            }
            else
            {
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    if (Physics.Raycast(ray, out RaycastHit hit, 100))
                    {
                        if (hit.transform.CompareTag("Monster"))
                        {
                            rubCount += 1;
                            if(rubCount >= rubThreshold)
                            {
                                ToIdleAnimation();
                                ArSceneController.Instance.FinishBating();
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
    public void StopLooking()
    {
        allowLookCam = false;
    }
    public void RefreshData()
    {
        StartCoroutine(DataManager.Instance.GetMonsterDataFromId(monsterData.id, this));
    }
    public void StartWander()
    {
        ChanceActivity(MonsterActivity.Wander);
    }
    public void StopWander()
    {
        ChanceActivity(MonsterActivity.Idle);
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
        if(act == MonsterActivity.Idle)
        {
            allowLookCam = true;
            idleFaceTo = ArSceneController.Instance.GetCurrentCameraPostion();
            idleFaceTo = new Vector3(idleFaceTo.x, gameObject.transform.position.y, idleFaceTo.z);
            Invoke("StopLooking", 0.5f);
        }
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
        //StartCoroutine(DataManager.Instance.FinishEating(amount, this));
    }
    public void LoadData()
    {
        monsterData = DataManager.Instance.GetMonsterById(PlayerController.Instance.CurrentMonsterId);
        MaxHungriness = monsterData.status.hungry.maxValue;
        MaxCleanliness = monsterData.status.cleanliness.maxValue;
        MaxHappiness = monsterData.status.happiness.maxValue;
        Hungriness = monsterData.status.hungry.value;
        Cleanliness = monsterData.status.cleanliness.value;
        Happiness = monsterData.status.happiness.value;

        SetStatusValue();

        MonsterAsset = DataManager.Instance.StringToMonsteAsset(monsterData.asset);
        LoadAsset();
    }
    public void SetStatusValue()
    {
        ArSceneController.Instance.SetHappiness(happiness, maxHappiness);
        ArSceneController.Instance.SetHungriness(Hungriness, MaxHungriness);
        ArSceneController.Instance.SetCleanliness(Cleanliness, MaxCleanliness);
    }
    public void LoadAsset()
    {
        
    }
    public void ToIdleAnimation()
    {
        monsterAnimator.SetBool("isWalk", false);
        monsterAnimator.SetBool("isEating", false);
    }
    public void ToWalkAnimation()
    {
        monsterAnimator.SetBool("isWalk", true);
    }
    public void ToInteraction()
    {
        monsterAnimator.SetTrigger("IsInteract");
    }
    public void ToEat()
    {
        monsterAnimator.SetBool("isEating", true);
    }
}
