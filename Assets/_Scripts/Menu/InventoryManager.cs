using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventorySlot[] inventorySlots;
    private InventoryItem[] items;

    private void Start()
    {
        items = new InventoryItem[inventorySlots.Length];
    }

    public void AddItem(InventoryItem newItem)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].IsEmpty())
            {
                items[i] = newItem;
                inventorySlots[i].UpdateSlot(newItem);
                return;
            }
        }
        Debug.LogWarning("Inventory is full!");
    }

    public void ClearInventory()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            items[i] = default;
            inventorySlots[i].ClearSlot();
        }
    }
}
