using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterActivity
{
    Idle,
    Wander,
    FindFood,
    Eating
}
public class MonsterController : MonoBehaviour
{
    private MonsterActivity _currentActivity;
    private Vector3 moveTarget;
    private float moveSpeed, turnSpeed;
    void Start()
    {
        _currentActivity = MonsterActivity.Idle;
        moveSpeed = 0.5f;
        turnSpeed = 5f;
    }

    void Update()
    {
        if(_currentActivity == MonsterActivity.Idle)
        {
            Debug.Log("Idle");
        }
        else if (_currentActivity == MonsterActivity.Wander)
        {

        }
        else if (_currentActivity == MonsterActivity.FindFood)
        {
            if(moveTarget != null)
            {
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, moveTarget, Time.deltaTime * moveSpeed);
                Vector3 direction = (moveTarget - gameObject.transform.position);
                Quaternion lookRotation = Quaternion.LookRotation(moveTarget - gameObject.transform.position);
                gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.LookRotation(moveTarget - gameObject.transform.position), Time.deltaTime * turnSpeed);

                if (Vector3.Distance(gameObject.transform.position, moveTarget) < 0.17f)
                {
                    Debug.Log("Eaten");
                    ChanceActivity(MonsterActivity.Idle);
                    return;
                }
            }
        }
        else if (_currentActivity == MonsterActivity.Eating)
        {

        }
    }
    public void ChanceActivity(MonsterActivity act)
    {
        _currentActivity = act;
    }
    public void MoveTo(Vector3 pos)
    {
        Debug.Log("Move to : " + pos);
        moveTarget = new Vector3(pos.x, gameObject.transform.position.y, pos.z);
        ChanceActivity(MonsterActivity.FindFood);
    }
}
