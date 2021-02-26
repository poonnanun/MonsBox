using System.Collections.Generic;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using UnityEngine;
using UnityEngine.EventSystems;

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
    [SerializeField]
    private Camera FirstPersonCamera;
    [SerializeField]
    private GameObject GameObjectHorizontalPlanePrefab;
    [SerializeField]
    private GameObject GameObjectPointPrefab;
    [SerializeField]
    private GameObject mainPanel;
    [SerializeField]
    private GameObject hintPanel;
    [SerializeField]
    private DetectedPlaneGenerator planeGenerator;
    [SerializeField]
    private GameObject planeIndicator;

    private bool isSummoned;
    private bool isGridRemoved;
    private GameObject _currentIndicator;
    private GamePhase _currentGamePhase;

    private const float _prefabRotation = 180.0f;
    public void Awake()
    {
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

                Vector3 position = FirstPersonCamera.transform.position;
                TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon;
                bool found = Frame.Raycast(position.x + (FirstPersonCamera.pixelWidth / 2), position.y + (FirstPersonCamera.pixelHeight / 2), raycastFilter, out hit);
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
                        Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
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
                            prefab = GameObjectPointPrefab;
                        }
                        else if (hit.Trackable is DetectedPlane)
                        {
                            DetectedPlane detectedPlane = hit.Trackable as DetectedPlane;
                            if (detectedPlane.PlaneType == DetectedPlaneType.HorizontalUpwardFacing)
                            {
                                prefab = GameObjectHorizontalPlanePrefab;
                            }
                            else
                            {
                                prefab = null;
                            }
                        }
                        else
                        {
                            prefab = GameObjectHorizontalPlanePrefab;
                        }

                        // Instantiate prefab at the hit pose.
                        if (prefab != null)
                        {
                            var gameObject = Instantiate(prefab, hit.Pose.position, hit.Pose.rotation);
                            gameObject.transform.rotation = Quaternion.Euler(0, _prefabRotation, 0);
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

        }
        #endregion
        #region Feeding
        else if (_currentGamePhase == GamePhase.FeedingPhase)
        {

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
        ChangeGamePhase(GamePhase.FeedingPhase);
    }
}
