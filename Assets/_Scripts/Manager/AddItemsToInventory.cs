using UnityEngine;

public class AddItemsToInventory : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public InventoryItem[] items;

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
                description = item.description + "\nPrice: " + item.price + " gold",
                healthPoints = item.healthPoints,
                quantity = item.quantity,
                price = item.price,
                itemImage = item.itemImage
            };
            inventoryManager.AddItem(newItem);
        }
    }
}

