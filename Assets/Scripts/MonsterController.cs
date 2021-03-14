﻿using System.Collections;
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
    private MonsterActivity _currentActivity;
    private Vector3 moveTarget;
    private float moveSpeed, turnSpeed;
    private float hungrinessTickSpeed, cleanlinessTickSpeed;
    private float hungrinessCount, cleanlinessCount;

    private int hungriness, maxHungriness;
    private int cleanliess, maxCleanliness;
    private int overEat;

    private bool isFinishBathing;

    private Rigidbody rb;
    public int Hungriness { get => hungriness; set => hungriness = value; }
    public int MaxHungriness { get => maxHungriness; set => maxHungriness = value; }
    public int Cleanliess { get => cleanliess; set => cleanliess = value; }
    public int MaxCleanliness { get => maxCleanliness; set => maxCleanliness = value; }

    void Start()
    {
        _currentActivity = MonsterActivity.Idle;
        moveSpeed = 0.5f;
        turnSpeed = 5f;

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
            if (Cleanliess > 0)
            {
                Cleanliess--;
                if (Cleanliess % 5 == 0)
                {
                    ArSceneController.Instance.SetCleanliness(Cleanliess, MaxCleanliness);
                }
                cleanlinessCount = 0;
            }
            else
            {
                // very dirty
            }
        }
        hungrinessCount += Time.deltaTime;
        cleanlinessCount += Time.deltaTime;
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
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, moveTarget, Time.deltaTime * moveSpeed);
                gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.LookRotation(moveTarget - gameObject.transform.position), Time.deltaTime * turnSpeed);

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

                if (Vector3.Distance(gameObject.transform.position, moveTarget) < 0.17f)
                {
                    isFinishBathing = false;
                    ChanceActivity(MonsterActivity.Bathing);
                    return;
                }
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
        Hungriness = MaxHungriness;
        Cleanliess = MaxCleanliness;
        overEat = 0;
        hungrinessTickSpeed = 1;
        cleanlinessTickSpeed = 10f;

    }
}
