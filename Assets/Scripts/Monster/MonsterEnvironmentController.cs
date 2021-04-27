using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEnvironmentController : MonoBehaviour
{
    public static MonsterEnvironmentController Instance;
    [SerializeField]
    private GameObject plane;

    private MonsterController currentMonster;
    private void Awake()
    {
        Instance = this;
    }
    public void Init(MonsterController mon)
    {
        currentMonster = Instantiate(mon, transform.position, Quaternion.identity, transform);
    }
    public Vector3 GetMonsterPosition()
    {
        return currentMonster.transform.position;
    }
    public MonsterController GetMonster()
    {
        return currentMonster;
    }
}
