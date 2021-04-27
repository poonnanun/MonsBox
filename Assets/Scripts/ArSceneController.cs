using System.Collections.Generic;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using Input = GoogleARCore.InstantPreviewInput;
#endif
public enum GamePhase
{
    SummonPhase,
    IdlePhase,
    FeedingPhase,
    CleaningPhase,
    ScanningPhase,
    PlayingPhase,
}
public enum CleaningPhase
{

}
public class ArSceneController : MonoBehaviour
{
    public static ArSceneController Instance;

    [SerializeField]
    private Camera firstPersonCamera;
    [SerializeField]
    private GameObject cameraObjectHoler;
    [SerializeField]
    private GameObject worldObjectHolder;
    [SerializeField]
    private GameObject monsterEnvironmentPrefabs;
    [SerializeField]
    private GameObject mainCanvas, minigamePanel, mainGamePanel;
    [SerializeField]
    private GameObject hintPanel;
    [SerializeField]
    private DetectedPlaneGenerator planeGenerator;
    [SerializeField]
    private GameObject planeIndicator;
    [SerializeField]
    private GameObject feedingCube;
    [SerializeField]
    private GameObject bathTub;
    [SerializeField]
    private TextMeshProUGUI monsterName;
    [SerializeField]
    private Image hungrinessBar, cleanlinessBar, happinessBar;
    [SerializeField]
    private ItemSelectionController itemSelector;

    private bool isSummoned;
    private bool isGridRemoved;
    private GameObject _currentIndicator;
    private GamePhase _currentGamePhase;
    private GameObject currentFeedingCube;
    private GameObject currentBath;
    private MonsterEnvironmentController currentMonsterEnvironment;
    private MonsterController currentMonster;
    private bool isAllowThrowFood;
    private bool isFoodThrown;
    private bool isAllowSpawnBath;
    private bool isBathSpawned;
    private bool isVariableReset;
    private bool isAllowAugmentedImage;

    private List<AugmentedImage> _tempAugmentedImages = new List<AugmentedImage>();
    private List<int> scannedImages = new List<int>();

    private const float _prefabRotation = 180.0f;

    public GameObject MainCanvas { get => mainCanvas; set => mainCanvas = value; }

    public void Awake()
    {
        Instance = this;
        // Enable ARCore to target 60fps camera capture frame rate on supported devices.
        // Note, Application.targetFrameRate is ignored when QualitySettings.vSyncCount != 0.
        Application.targetFrameRate = 60;
    }
    private void Start()
    {
        isSummoned = false;
        isGridRemoved = false;
        mainCanvas.SetActive(false);
        hintPanel.SetActive(true);
        minigamePanel.SetActive(true);
        MinigameController.Instance.ClosePanel();
        SoundInitializer.Instance.Init();
        SoundManager.Instance.Init();
        SoundManager.Instance.TurnOnBGM(BGSoundName.ArScene);

        LoadMonsterData();
    }
    // Update is called once per frame
    void Update()
    {
        UpdateApplicationLifecycle();

        #region Summon
        if (_currentGamePhase == GamePhase.SummonPhase)
        {
            if (!isSummoned)
            {
                if (ActiveGridAndSpawnObject(monsterEnvironmentPrefabs, out GameObject tmp) && tmp != null)
                {
                    currentMonsterEnvironment = tmp.GetComponent<MonsterEnvironmentController>();
                    currentMonsterEnvironment.Init(currentMonster);
                    currentMonster = currentMonsterEnvironment.GetMonster();
                    isSummoned = true;
                    mainCanvas.SetActive(true);
                    hintPanel.SetActive(false);
                    _currentIndicator.SetActive(false);
                }
            }
            else if (!isGridRemoved)
            {
                planeGenerator.DeactiveGridDisplay();
                isGridRemoved = true;
                ChangeGamePhase(GamePhase.IdlePhase);
            }
        }
        #endregion
        #region Idle
        else if (_currentGamePhase == GamePhase.IdlePhase)
        {
            Debug.Log("Idling...");
            if (!isVariableReset)
            {
                ResetVariable();
            }
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                if (Physics.Raycast(ray, out RaycastHit hit, 100))
                {
                    if (hit.transform.CompareTag("Monster"))
                    {
                        hit.transform.GetComponent<MonsterController>().AddHappiness();
                        // do something
                    }
                    SetNameText(hit.transform.tag);
                }
            }
        }
        #endregion
        #region Feeding
        else if (_currentGamePhase == GamePhase.FeedingPhase)
        {
            if (currentFeedingCube == null)
            {
                SpawnFood(currentFeedingCube);
            }
            else
            {
                if (isFoodThrown)
                {
                    if (!ThrowableObject.Instance.IsMoving())
                    {
                        currentMonster.MoveTo(ThrowableObject.Instance.transform.position);
                        currentMonster.ChanceActivity(MonsterActivity.FindFood);
                        ChangeGamePhase(GamePhase.IdlePhase);
                        isFoodThrown = false;
                    }
                }
                if (isAllowThrowFood)
                {
                    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        ThrowableObject.Instance.StartTouch(Input.GetTouch(0).position);
                    }
                    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
                    {
                        currentFeedingCube.transform.SetParent(worldObjectHolder.transform);
                        //SetNameText("zDiff : " + (currentMonsterEnvironment.GetMonsterPosition().z - firstPersonCamera.transform.position.z).ToString() + " xDiff : " + (currentMonsterEnvironment.GetMonsterPosition().x - firstPersonCamera.transform.position.x).ToString());
                        ThrowableObject.Instance.StopTouch(Input.GetTouch(0).position, (currentMonsterEnvironment.GetMonsterPosition().z - firstPersonCamera.transform.position.z) * 50f, (currentMonsterEnvironment.GetMonsterPosition().x - firstPersonCamera.transform.position.x) * 25f);
                        // find the better algorithm for realistic aimming. may be just get distance and get the angle?
                        isAllowThrowFood = false;
                        isFoodThrown = true;
                    }
                }
            }
        }
        #endregion
        #region Cleaning
        else if (_currentGamePhase == GamePhase.CleaningPhase)
        {
            if (!isBathSpawned)
            {
                if (ActiveGridAndSpawnObject(bathTub, out GameObject tmp) && tmp != null)
                {
                    currentBath = tmp;
                    isBathSpawned = true;
                    planeGenerator.DeactiveGridDisplay();
                    isGridRemoved = true;
                    _currentIndicator.SetActive(false);
                }
            }
            else
            {
                currentMonster.MoveTo(currentBath.transform.position);
                currentMonster.ChanceActivity(MonsterActivity.FindBath);
                ChangeGamePhase(GamePhase.IdlePhase);
            }
        }
        #endregion
        #region Scanning
        else if (_currentGamePhase == GamePhase.ScanningPhase)
        {
            Session.GetTrackables<AugmentedImage>(_tempAugmentedImages, TrackableQueryFilter.Updated);
            if(scannedImages.Count < _tempAugmentedImages.Count)
            {
                foreach (var image in _tempAugmentedImages)
                {
                    if (scannedImages.Contains(image.DatabaseIndex))
                    {
                        continue;
                    }
                    if (image.TrackingState == TrackingState.Tracking)
                    {
                        // Create an anchor to ensure that ARCore keeps tracking this augmented image.
                        Anchor anchor = image.CreateAnchor(image.CenterPose);
                        GameObject visualizer = Instantiate(feedingCube, anchor.transform);
                        visualizer.transform.localPosition = Vector3.zero;
                        scannedImages.Add(image.DatabaseIndex);
                    }
                }
            }
        }
        #endregion
        #region Playing
        else if (_currentGamePhase == GamePhase.PlayingPhase)
        {
            
        }
        #endregion
    }
    public void LoadMonsterData()
    {
        MonsterAssetController.Instance.Init();
        MonsterRawData tmp = DataManager.Instance.GetMonsterById(PlayerController.Instance.CurrentMonsterId);
        MonsterAsset monsterAsset = DataManager.Instance.StringToMonsteAsset(tmp.asset);
        currentMonster = MonsterAssetController.Instance.GetModelById(monsterAsset.model);
    }
    private void UpdateApplicationLifecycle()
    {
        // Exit the app when the 'back' button is pressed.
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        // Only allow the screen to sleep when not tracking.
        if (Session.Status != SessionStatus.Tracking)
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
    private bool ActiveGridAndSpawnObject(GameObject prefabs, out GameObject assignValue)
    {
        Touch touch;
        assignValue = null;
        Vector3 position = firstPersonCamera.transform.position;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon;
        bool found = Frame.Raycast(position.x + (firstPersonCamera.pixelWidth / 2), position.y + (firstPersonCamera.pixelHeight / 2), raycastFilter, out TrackableHit hit);
        if (found)
        {
            if (_currentIndicator == null)
            {
                _currentIndicator = Instantiate(planeIndicator, hit.Pose.position, hit.Pose.rotation);
            }
            else
            {
                _currentIndicator.transform.position = hit.Pose.position;
                _currentIndicator.transform.rotation = hit.Pose.rotation;
            }
            _currentIndicator.SetActive(true);
        }
        // TODO make the rotation change to camera
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return false;
        }
        // Should not handle input if the player is pointing on UI.
        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            return false;
        }

        if (found)
        {
            // Use hit pose and camera pose to check if hittest is from the
            // back of the plane, if it is, no need to create the anchor.
            if ((hit.Trackable is DetectedPlane) &&
                Vector3.Dot(firstPersonCamera.transform.position - hit.Pose.position,
                    hit.Pose.rotation * Vector3.up) < 0)
            {
                Debug.Log("Hit at back of the current DetectedPlane");
            }
            else
            {
                // Choose the prefab based on the Trackable that got hit.
                GameObject prefab;
                if (hit.Trackable is FeaturePoint)
                {
                    prefab = prefabs;
                }
                else if (hit.Trackable is DetectedPlane)
                {
                    DetectedPlane detectedPlane = hit.Trackable as DetectedPlane;
                    if (detectedPlane.PlaneType == DetectedPlaneType.HorizontalUpwardFacing)
                    {
                        prefab = prefabs;
                    }
                    else
                    {
                        prefab = null;
                    }
                }
                else
                {
                    prefab = prefabs;
                }
                // Instantiate prefab at the hit pose.
                if (prefab != null)
                {
                    var tmpObject = Instantiate(prefab, hit.Pose.position, hit.Pose.rotation);
                    tmpObject.transform.rotation = Quaternion.Euler(0, _prefabRotation, 0);
                    assignValue = tmpObject;
                    //gameObject.transform.Rotate(0, _prefabRotation, 0, Space.Self);
                    var anchor = hit.Trackable.CreateAnchor(hit.Pose);
                    // Make game object a child of the anchor.
                    gameObject.transform.parent = anchor.transform;
                    // Initialize Instant Placement Effect.
                    if (hit.Trackable is InstantPlacementPoint)
                    {
                        gameObject.GetComponentInChildren<InstantPlacementEffect>().InitializeWithTrackable(hit.Trackable);
                    }
                    return true;
                }
            }
        }
        return false;
    }
    public void ChangeGamePhase(GamePhase gamePhase)
    {
        if(gamePhase == GamePhase.IdlePhase)
        {
            isVariableReset = false;
        }
        _currentGamePhase = gamePhase;
    }
    public void SetCurrentMonster(MonsterController mon)
    {
        currentMonster = mon;
    }
    public MonsterController GetCurrentMonster()
    {
        return currentMonster;
    }
    public void EnterFeeding()
    {
        itemSelector.OpenSelection(SelectionType.Food);
    }
    public void EnterSelectMinigame()
    {
        itemSelector.OpenSelection(SelectionType.Minigame);
    }
    public void OnEnterMinigame()
    {
        mainGamePanel.SetActive(false);
        ChangeGamePhase(GamePhase.PlayingPhase);
    }
    public void OnFinishMinigame()
    {
        mainGamePanel.SetActive(true);
        MinigameController.Instance.ClosePanel();
        ChangeGamePhase(GamePhase.IdlePhase);
    }
    public void StartThrowFood(GameObject food)
    {
        isAllowThrowFood = false;
        ChangeGamePhase(GamePhase.FeedingPhase);
        SpawnFood(food);
        Invoke("SetIsAllowThrowFood", 0.2f);
    }
    public void EnterCleaning()
    {
        if (currentBath == null)
        {
            ChangeGamePhase(GamePhase.CleaningPhase);
            planeGenerator.ActiveGridDisplay();
            isGridRemoved = false;
        }
    }
    public void EnterScanning()
    {
        if (isAllowAugmentedImage)
        {
            ChangeGamePhase(GamePhase.ScanningPhase);
        }
    }
    public void SpawnFood(GameObject food)
    {
        isFoodThrown = false;
        currentFeedingCube = Instantiate(feedingCube, cameraObjectHoler.transform);
        Instantiate(food, currentFeedingCube.transform);
        currentFeedingCube.transform.SetParent(cameraObjectHoler.transform);
        currentFeedingCube.transform.localPosition = new Vector3(0, -0.05f, 0.2f);
        currentFeedingCube.transform.localRotation = Quaternion.identity;
        currentFeedingCube.GetComponent<ThrowableObject>().SetLocation();
    }
    public void ResetVariable()
    {
        if(cameraObjectHoler.transform.childCount > 0)
        {
            for(int i = cameraObjectHoler.transform.childCount-1; i>=0; i--)
            {
                Destroy(cameraObjectHoler.transform.GetChild(i).gameObject);
            }
        }
        isBathSpawned = false;
        isFoodThrown = false;
        isVariableReset = true;
    }
    public void SetAllowAugmentedImage(bool boo)
    {
        isAllowAugmentedImage = boo;
    }
    public void DespawnBathTub()
    {
        GameObject tmp = currentBath;
        currentBath = null;
        Destroy(tmp);
        isBathSpawned = false;
    }
    public void SetIsAllowThrowFood()
    {
        isAllowThrowFood = true;
    }
    public Vector3 GetCurrentCameraPostion()
    {
        return firstPersonCamera.transform.position;
    }
    public Camera GetCurrentCamera()
    {
        return firstPersonCamera;
    }
    public void SetHungriness(int amount, int max)
    {
        float fillAmount = (float)amount / (float)max;
        hungrinessBar.fillAmount = fillAmount;
    }
    public void SetCleanliness(int amount, int max)
    {
        float fillAmount = (float)amount / (float)max;
        cleanlinessBar.fillAmount = fillAmount;
    }
    public void SetHappiness(int amount, int max)
    {
        float fillAmount = (float)amount / (float)max;
        happinessBar.fillAmount = fillAmount;
    }
    public FoodScript GetCurrentFood()
    {
        return itemSelector.GetFoodFromModelId(currentFeedingCube.transform.GetChild(0).GetComponent<ItemModel>().AssetId);
    }
    public void DestroyCurrentFood()
    {
        GameObject tmp = currentFeedingCube;
        currentFeedingCube = null;
        Destroy(tmp);
    }
    public void SetNameText(string word)
    {
        monsterName.text = word;
    }
    public void DevDropFood()
    {
        if(currentFeedingCube != null)
        {
            currentFeedingCube.transform.SetParent(worldObjectHolder.transform);
            ThrowableObject.Instance.DevDrop();
            isAllowThrowFood = false;
            isFoodThrown = true;
        }
    }
    public void DevFinishBath()
    {
        if (currentBath != null)
        {
            currentBath.GetComponent<BathScript>().ExitBathTub();
        }
    }
    public void DevRemoveMonster()
    {
        if(currentMonsterEnvironment != null)
        {
            GameObject tmp = currentMonsterEnvironment.gameObject;
            currentMonsterEnvironment = null;
            Destroy(tmp);
        }
    }
    public GameObject GetWorldObjectHolder()
    {
        return worldObjectHolder;
    }
    public GameObject GetCameraObjectHolder()
    {
        return cameraObjectHoler;
    }
}
