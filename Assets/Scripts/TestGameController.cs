using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGameController : MonoBehaviour
{
    [SerializeField]
    private GameObject feedingCube;
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private GameObject currentMonster;
    [SerializeField]
    private GameObject cameraObjectHoler;
    [SerializeField]
    private GameObject worldObjectHolder;

    private GameObject currentFeedingCube;
    private bool allowFeeding;
    private bool isThrown;
    // Start is called before the first frame update
    void Start()
    {
        
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
                        currentMonster.GetComponent<MonsterController>().MoveTo(ThrowableObject.Instance.transform.position);
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
                    ThrowableObject.Instance.StopTouch(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0), Mathf.Abs(currentMonster.transform.position.z - cam.transform.position.z) * 50f, 0);
                    isThrown = true;
                }
            }
        }
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
