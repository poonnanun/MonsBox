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
    private GameObject gameObjectHorizontalPlanePrefab;
    [SerializeField]
    private GameObject gameObjectPointPrefab;
    [SerializeField]
    private GameObject mainPanel;
    [SerializeField]
    private GameObject hintPanel;
    [SerializeField]
    private DetectedPlaneGenerator planeGenerator;
    [SerializeField]
    private GameObject planeIndicator;
    [SerializeField]
    private GameObject feedingCube;
    [SerializeField]
    private TextMeshProUGUI monsterName;
    [SerializeField]
    private Slider sensitiveXY;

    private bool isSummoned;
    private bool isGridRemoved;
    private GameObject _currentIndicator;
    private GamePhase _currentGamePhase;
    private GameObject currentFeedingCube;
    private MonsterEnvironmentController currentMonsterEnvironment;
    private bool isAllowThrowFood;
    private bool isFoodThrown;

    private const float _prefabRotation = 180.0f;
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
        mainPanel.SetActive(false);
        hintPanel.SetActive(true);
        sensitiveXY.gameObject.SetActive(false);
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
                Touch touch;
                TrackableHit hit;

                Vector3 position = firstPersonCamera.transform.position;
                TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon;
                bool found = Frame.Raycast(position.x + (firstPersonCamera.pixelWidth / 2), position.y + (firstPersonCamera.pixelHeight / 2), raycastFilter, out hit);
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
                }
                // TODO make the rotation change to camera
                if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
                {
                    return;
                }
                // Should not handle input if the player is pointing on UI.
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    return;
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
                            prefab = gameObjectPointPrefab;
                        }
                        else if (hit.Trackable is DetectedPlane)
                        {
                            DetectedPlane detectedPlane = hit.Trackable as DetectedPlane;
                            if (detectedPlane.PlaneType == DetectedPlaneType.HorizontalUpwardFacing)
                            {
                                prefab = gameObjectHorizontalPlanePrefab;
                            }
                            else
                            {
                                prefab = null;
                            }
                        }
                        else
                        {
                            prefab = gameObjectHorizontalPlanePrefab;
                        }

                        // Instantiate prefab at the hit pose.
                        if (prefab != null)
                        {
                            var monsterEnvironment = Instantiate(prefab, hit.Pose.position, hit.Pose.rotation);
                            monsterEnvironment.transform.rotation = Quaternion.Euler(0, _prefabRotation, 0);
                            currentMonsterEnvironment = monsterEnvironment.GetComponent<MonsterEnvironmentController>();
                            //gameObject.transform.Rotate(0, _prefabRotation, 0, Space.Self);
                            var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                            // Make game object a child of the anchor.
                            gameObject.transform.parent = anchor.transform;

                            // Initialize Instant Placement Effect.
                            if (hit.Trackable is InstantPlacementPoint)
                            {
                                gameObject.GetComponentInChildren<InstantPlacementEffect>().InitializeWithTrackable(hit.Trackable);
                            }
                            OnSummon();
                        }

                    }
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
        }
        #endregion
        #region Feeding
        else if (_currentGamePhase == GamePhase.FeedingPhase)
        {
            if (currentFeedingCube == null)
            {
                SpawnFood();
            }
            else
            {
                if (isFoodThrown)
                {
                    if (!ThrowableObject.Instance.IsMoving())
                    {
                        currentMonsterEnvironment.GetMonster().GetComponent<MonsterController>().MoveTo(ThrowableObject.Instance.transform.position);
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
                        ThrowableObject.Instance.StopTouch(Input.GetTouch(0).position, Mathf.Abs(currentMonsterEnvironment.GetMonsterPosition().z - firstPersonCamera.transform.position.z) * 50f);
                        monsterName.text = ThrowableObject.Instance.GetLastThrownZ().ToString();
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

        }
        #endregion
    }
    private void OnSummon()
    {
        isSummoned = true;
        mainPanel.SetActive(true);
        hintPanel.SetActive(false);
        _currentIndicator.SetActive(false);
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
    public void ChangeGamePhase(GamePhase gamePhase)
    {
        _currentGamePhase = gamePhase;
    }
    public void EnterFeeding()
    {
        isAllowThrowFood = false;
        ChangeGamePhase(GamePhase.FeedingPhase);
        SpawnFood();
        Invoke("SetIsAllowThrowFood", 0.2f);
    }
    public void SpawnFood()
    {
        isFoodThrown = false;
        if (currentFeedingCube == null)
        {
            currentFeedingCube = Instantiate(feedingCube, cameraObjectHoler.transform);
        }
        currentFeedingCube.transform.SetParent(cameraObjectHoler.transform);
        currentFeedingCube.transform.localPosition = new Vector3(0, -0.05f, 0.2f);
        currentFeedingCube.transform.localRotation = Quaternion.identity;
        currentFeedingCube.GetComponent<ThrowableObject>().SetLocation();
        sensitiveXY.gameObject.SetActive(true);
        sensitiveXY.value = currentFeedingCube.GetComponent<ThrowableObject>().Sensitivity;
    }
    public void SpawnTub()
    {
        currentFeedingCube.transform.SetParent(worldObjectHolder.transform);
    }
    public void SetIsAllowThrowFood()
    {
        isAllowThrowFood = true;
    }
    public Vector3 GetCurrentCameraPostion()
    {
        return firstPersonCamera.transform.position;
    }
    public void SetSensitivity()
    {
        if(currentFeedingCube != null)
        {
            currentFeedingCube.GetComponent<ThrowableObject>().Sensitivity = sensitiveXY.value;
            monsterName.text = sensitiveXY.value.ToString();
        }
        
    }
}
