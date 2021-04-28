using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BathScript : MonoBehaviour
{
    private Vector3 desiredPostion;
    private Quaternion desiredRotation;
    private Transform beforeParent;
    private GameObject currentMonster;
    private bool isOnBathTub;
    // Start is called before the first frame update
    void Start()
    {
        isOnBathTub = false;
        LoadData();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isOnBathTub)
        {
            if (other.gameObject.CompareTag("Monster"))
            {
                GetOnBathTub(other.gameObject);
            }
        }
    }
    public void GetOnBathTub(GameObject mons)
    {
        isOnBathTub = true;
        currentMonster = mons;
        beforeParent = mons.transform.parent;
        mons.transform.SetParent(transform);
        mons.GetComponent<MonsterController>().OnArrivedAtBath();
        mons.GetComponent<MonsterController>().SetKinematic(true);
        mons.GetComponent<MonsterController>().ToInteraction();
        mons.transform.localPosition = desiredPostion;
        mons.transform.localRotation = desiredRotation;
    }
    public void ExitBathTub()
    {
        currentMonster.GetComponent<MonsterController>().SetIsFinishBathing(true);
        currentMonster.transform.SetParent(beforeParent);
        currentMonster.GetComponent<MonsterController>().SetKinematic(false);
        ArSceneController.Instance.DespawnBathTub();
    }
    public void LoadData()
    {
        desiredPostion = new Vector3(0, 0.1f, 0);
        desiredRotation = Quaternion.identity;
        //TODO set rotaion to face toward camera
    }
}
