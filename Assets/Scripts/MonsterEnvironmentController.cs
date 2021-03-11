using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEnvironmentController : MonoBehaviour
{
    public static MonsterEnvironmentController Instance;
    [SerializeField]
    private GameObject plane, monster;

    private void Awake()
    {
        Instance = this;
    }
    public Vector3 GetMonsterPosition()
    {
        return monster.transform.position;
    }
    public GameObject GetMonster()
    {
        return monster;
    }
}
