using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Scriptable/Food", order = 1)]
public class FoodScript : ScriptableObject
{
    [SerializeField]
    private string itemId;
    [SerializeField]
    private Sprite image;
    [SerializeField]
    private int effect;
    [SerializeField]
    private int price;
  
    private string itemName;
    private int itemQuantity;
    private int itemPrice;
    private string assetId;
    private string status;
    public string ItemName { get => itemName; set => itemName = value; }
    public int ItemQuantity { get => itemQuantity; set => itemQuantity = value; }
    public int ItemPrice { get => itemPrice; set => itemPrice = value; }
    public string AssetId { get => assetId; set => assetId = value; }
    public string Status { get => status; set => status = value; }
    public int Effect { get => effect; set => effect = value; }
    public string ItemId { get => itemId; set => itemId = value; }
    public Sprite Image { get => image; set => image = value; }

    public void LoadData()
    {
        ItemRawData data = DataManager.Instance.GetItemById(ItemId);
        ItemName = data.name;
        ItemQuantity = data.quantity;
        ItemPrice = price;
        AssetId = data.asset;
        Status = data.status;
    }
}
