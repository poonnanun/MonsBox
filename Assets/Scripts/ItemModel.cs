using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemModel : MonoBehaviour
{
    [SerializeField]
    private string assetId;

    public string AssetId { get => assetId; set => assetId = value; }
}
