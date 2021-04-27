using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestGameController : MonoBehaviour
{
    [SerializeField]
    private GameObject feedingCube;
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private MonsterController monsterPre;
    [SerializeField]
    private GameObject cameraObjectHoler;
    [SerializeField]
    private GameObject worldObjectHolder;
    [SerializeField]
    private GameObject monEnvironment; 

    private GameObject currentFeedingCube;
    private bool allowFeeding;
    private bool isThrown;
    private MonsterEnvironmentController currentMonEnvironment;
    private MonsterController currentMonster;
    // Start is called before the first frame update
    void Start()
    {
        GameConfig.isTest = true;
        SpawnMonster();
        SoundInitializer.Instance.Init();
        SoundManager.Instance.Init();
        SoundManager.Instance.TurnOnBGM(BGSoundName.ArScene);
    }

    // Update is called once per frame
    void Update()
    {
        if (allowFeeding)
        {
            if (currentFeedingCube == null)
            {
                SpawnFood();
            }
            else
            {
                if (isThrown)
                {
                    if (!ThrowableObject.Instance.IsMoving())
                    {
                        currentMonster.MoveTo(ThrowableObject.Instance.transform.position);
                        currentMonster.ChanceActivity(MonsterActivity.FindFood);
                        allowFeeding = false;
                    }
                }
                if (Input.GetMouseButtonDown(0))
                {
                    ThrowableObject.Instance.StartTouch(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                }
                if (Input.GetMouseButtonUp(0))
                {
                    currentFeedingCube.transform.SetParent(worldObjectHolder.transform);
                    ThrowableObject.Instance.StopTouch(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0), Mathf.Abs(monsterPre.transform.position.z - cam.transform.position.z) * 50f, 0);
                    isThrown = true;
                }
            }
        }
    }

    public void SpawnMonster()
    {
        currentMonEnvironment = Instantiate(monEnvironment).GetComponent<MonsterEnvironmentController>();
        currentMonEnvironment.Init(monsterPre);
        currentMonster = currentMonEnvironment.GetMonster();
    }
    public void EnterFeeding()
    {
        allowFeeding = false;
        SpawnFood();
        Invoke("SetAllowFeeding", 0.2f);
    }
    public void SpawnFood()
    {
        isThrown = false;
        if (currentFeedingCube == null)
        {
            currentFeedingCube = Instantiate(feedingCube, cameraObjectHoler.transform);
        }
        currentFeedingCube.transform.SetParent(cameraObjectHoler.transform);
        currentFeedingCube.transform.localPosition = new Vector3(0, -0.05f, 0.2f);
        currentFeedingCube.transform.localRotation = Quaternion.identity;
        currentFeedingCube.GetComponent<ThrowableObject>().SetLocation();
        currentFeedingCube.GetComponent<ThrowableObject>().Sensitivity = 0.75f;
    }
    public void SpawnTub()
    {
        currentFeedingCube.transform.SetParent(worldObjectHolder.transform);
    }
    public void SetAllowFeeding()
    {
        allowFeeding = true;
    }
}
