using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodScript : MonoBehaviour
{
    private int fullnessAmount;
    private int price;
    public int FullnessAmount { get => fullnessAmount; set => fullnessAmount = value; }
    public int Price { get => price; set => price = value; }

    // Start is called before the first frame update
    void Start()
    {
        LoadData();
    }

    public void LoadData()
    {
        FullnessAmount = 10;
        Price = 10;
    }
}
