using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum SelectionType
{
    Food = 0,
    Bath,
    Minigame,
}
public class ItemSelectionController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI ItemSelectionHeader;
    [SerializeField]
    private GameObject ItemHolder;
    [SerializeField]
    private GameObject defaultFood;
    [SerializeField]
    private GameObject prefabs;
    [SerializeField]
    private GameObject purchaseConfrimPanel;

    private List<FoodScript> itemPool;
    private int selectedItem;
    private List<ItemModel> itemModels;
    private List<MinigameBase> minigamePool;
    private List<GameObject> createdItem;
    private SelectionType currentType;
    private void Awake()
    {
        createdItem = new List<GameObject>();
    }
    public void OpenSelection(SelectionType type)
    {
        gameObject.SetActive(true);
        currentType = type;
        selectedItem = 0;
        ItemSelectionHeader.text = type.ToString();
        if (type == SelectionType.Food)
        {
            LoadAllFood();
            LoadAllFoodModel();
        }
        else if(type == SelectionType.Bath)
        {

        }
        else if(type == SelectionType.Minigame)
        {
            LoadAllMinigame();
        }
        
    }
    public void CloseSelection()
    {
        gameObject.SetActive(false);
    }
    public void DisplayFoodItems()
    {
        foreach (GameObject des in createdItem)
        {
            Debug.Log("deleted");
            Destroy(des);
        }
        createdItem.Clear();
        int count = 0;
        foreach(FoodScript a in itemPool)
        {
            GameObject tmp = Instantiate(prefabs, ItemHolder.transform);
            tmp.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = a.ItemName;
            tmp.transform.GetChild(1).GetComponent<Image>().sprite = a.Image;
            tmp.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = a.ItemPrice + "G";
            int num = count;
            count++;
            tmp.GetComponent<Button>().onClick.AddListener(delegate { OnSelectItem(num); });
            createdItem.Add(tmp);
        }
    }
    public void DisplayMinigame()
    {
        foreach (GameObject des in createdItem)
        {
            Destroy(des);
        }
        createdItem.Clear();
        int count = 0;

        foreach (MinigameBase a in minigamePool)
        {
            GameObject tmp = Instantiate(prefabs, ItemHolder.transform);
            tmp.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = a.MinigameName;
            tmp.transform.GetChild(1).GetComponent<Image>().sprite = a.Image;
            tmp.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
            int num = count;
            count++;
            tmp.GetComponent<Button>().onClick.AddListener(delegate { OnSelectItem(num); });
            createdItem.Add(tmp);
        }
    }
    public void OnSelectItem(int num)
    {
        selectedItem = num;
    }
    public void OnConfirmSelection()
    {
        if(currentType == SelectionType.Food)
        {
            string assetId = itemPool[selectedItem].AssetId;
            GameObject asset = null;
            foreach (ItemModel a in itemModels)
            {
                if (a.AssetId == assetId)
                {
                    asset = a.gameObject;
                }
            }
            if (asset == null)
            {
                asset = defaultFood;
            }
            CloseSelection();
            ArSceneController.Instance.StartThrowFood(asset);
        }
        else if (currentType == SelectionType.Bath)
        {

        }
        else if (currentType == SelectionType.Minigame)
        {
            CloseSelection();
            MinigameController.Instance.SelectMinigame(minigamePool[selectedItem].MinigameId);
            ArSceneController.Instance.OnEnterMinigame();
        }

    }
    public void PurchaseItem(FoodScript food)
    {
        purchaseConfrimPanel.SetActive(true);
        purchaseConfrimPanel.transform.GetChild(3).GetComponent<Button>().interactable = true;
        purchaseConfrimPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Buy " + food.name + " for";
        purchaseConfrimPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = food.ItemPrice + "G";
        if(food.ItemPrice > PlayerController.Instance.CurrentGold)
        {
            purchaseConfrimPanel.transform.GetChild(3).GetComponent<Button>().interactable = false;
        }
    }
    public void OnCancelSelection()
    {
        CloseSelection();
    }
    public void LoadAllFood()
    {
        itemPool = new List<FoodScript>();
        foreach (Object a in Resources.LoadAll("Scriptable/Food"))
        {
            FoodScript tmp = (FoodScript)a;
            tmp.LoadData();
            itemPool.Add(tmp);
        }
        DisplayFoodItems();
    }
    public void LoadAllFoodModel()
    {
        itemModels = new List<ItemModel>();
        foreach (Object a in Resources.LoadAll("items"))
        {
            GameObject b = (GameObject)a;
            itemModels.Add(b.GetComponent<ItemModel>());
        }
    }
    public void LoadAllMinigame()
    {
        minigamePool = new List<MinigameBase>();
        minigamePool.AddRange(MinigameController.Instance.GetMinigameList());
        DisplayMinigame();
    }
    public FoodScript GetFoodFromModelId(string id)
    {
        FoodScript result = itemPool[0];
        foreach(FoodScript a in itemPool)
        {
            if (a.AssetId == id)
            {
                result = a;
                break;
            }
        }
        return result;
    }
}
