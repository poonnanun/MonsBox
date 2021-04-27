using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using Input = GoogleARCore.InstantPreviewInput;
#endif
public class CatchingMinigame : MinigameBase
{
    [SerializeField]
    private GameObject ballPrefabs;
    [SerializeField]
    private float respawnTime;

    private MonsterController currentMonster;
    private GameObject currentBall;
    private bool isBallOnHand;
    private GameObject worldObjectHolder;
    private GameObject cameraObjectHolder;
    private Camera cam;
    

    private readonly float changePosInterval = 2f;
    private float intervalCount;

    private float limitRight, limitLeft, limitTop, limitBot;
    public override void OnSetUp()
    {
        worldObjectHolder = ArSceneController.Instance.GetWorldObjectHolder();
        cameraObjectHolder = ArSceneController.Instance.GetCameraObjectHolder();
        cam = ArSceneController.Instance.GetCurrentCamera();

        currentMonster = ArSceneController.Instance.GetCurrentMonster();
        currentMonster.StartWander();
        currentMonster.MoveTo(currentMonster.transform.position);
        SetWanderLimitation(currentMonster.transform.position);
        isBallOnHand = false;
        intervalCount = 0;
        IsStarted = false;
        RespawnBall();
        base.OnSetUp();
    }
    public override void OnFinished()
    {
        currentMonster.StopWander();
        base.OnFinished();
    }
    public override void OnEveryFrame()
    {
        intervalCount += Time.deltaTime;
        if (intervalCount >= changePosInterval)
        {
            currentMonster.MoveTo(GetRandomPosition(currentMonster.transform.position));
            intervalCount = 0;
        }

        if (isBallOnHand && currentBall != null)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                currentBall.GetComponent<ThrowableBall>().StartTouch(Input.GetTouch(0).position);
            }
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                currentBall.transform.SetParent(worldObjectHolder.transform);
                //SetNameText("zDiff : " + (currentMonsterEnvironment.GetMonsterPosition().z - firstPersonCamera.transform.position.z).ToString() + " xDiff : " + (currentMonsterEnvironment.GetMonsterPosition().x - firstPersonCamera.transform.position.x).ToString());
                currentBall.GetComponent<ThrowableBall>().StopTouch(Input.GetTouch(0).position, (currentMonster.transform.position.z - cam.transform.position.z) * 50f, (currentMonster.transform.position.x - cam.transform.position.x) * 25f);
                // find the better algorithm for realistic aimming. may be just get distance and get the angle?
                OnThrow();
            }
        }
    }
    public void DevDropBall()
    {
        if (isBallOnHand)
        {
            currentBall.transform.SetParent(worldObjectHolder.transform);
            currentBall.GetComponent<ThrowableBall>().DevDrop();
            isBallOnHand = false;
        }
    }
    private void SetWanderLimitation(Vector3 pos)
    {
        limitRight = pos.x + 1f;
        limitLeft = pos.x - 1f;
        limitTop = pos.z + 1f;
        limitBot = pos.z - 1f;
    }
    private Vector3 GetRandomPosition(Vector3 pos)
    {
        float randomZ = Random.Range(limitBot, limitTop);
        float randomX = Random.Range(limitLeft, limitRight);
        return new Vector3(randomX, pos.y, randomZ);
    }
    public void OnCatch()
    {
        AddScore(1);
        RespawnCooldown();
    }
    public void RespawnCooldown()
    {
        DestroyDroppedBall();
        Invoke("RespawnBall", respawnTime);
    }
    public void RespawnBall()
    {
        GameObject tmp = Instantiate(ballPrefabs, cameraObjectHolder.transform);
        tmp.transform.SetParent(cameraObjectHolder.transform);
        tmp.transform.localPosition = new Vector3(0, -0.05f, 0.2f);
        tmp.transform.localRotation = Quaternion.identity;
        tmp.GetComponent<ThrowableBall>().SetLocation();
        tmp.GetComponent<ThrowableBall>().SetController(this);
        currentBall = tmp;
        Invoke("AllowToThrow", 0.1f);
    }
    public void AllowToThrow()
    {
        isBallOnHand = true;
    }
    public void OnThrow()
    {
        isBallOnHand = false;
        // do something particle or sound
    }
    public void OnBallDropped()
    {
        RespawnCooldown();
    }
    public void DestroyDroppedBall()
    {
        GameObject tmp = currentBall;
        currentBall = null;
        Destroy(tmp);
    }
    public override void SetNewHighScore(int amount)
    {
        LocalSave.SaveCatchHighscore(amount);
    }
    public override int GetCurrentHighScore()
    {
        return LocalSave.GetCatchHighScore();
    }
}
