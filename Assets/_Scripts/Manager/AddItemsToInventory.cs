using UnityEngine;

public class AddItemsToInventory : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public ItemData[] items;

    void Start()
    {
        AddPredefinedItems();
    }

    void AddPredefinedItems()
    {
        foreach (var item in items)
        {
            InventoryItem newItem = new InventoryItem
            {
                itemName = item.itemName,
                description = item.itemDescription,
                healthPoints = item.healthPoints,
                itemPrice = item.itemPrice,
                quantity = 1,
                itemImage = item.itemSprite
            };
            inventoryManager.AddItem(newItem);
        }
    }
}

[System.Serializable]
public struct ItemData
{
    public string itemName;
    public string itemDescription;
    public int healthPoints;
    public int itemPrice;
    public Sprite itemSprite;
}

