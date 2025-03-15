using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventorySlot[] inventorySlots;
    private List<InventoryItem> items = new List<InventoryItem>();

    private void Start()
    {
        ClearInventory();
    }

    public void AddItem(InventoryItem newItem)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].IsEmpty())
            {
                items.Add(newItem);
                inventorySlots[i].UpdateSlot(newItem);
                return;
            }
        }
        Debug.LogWarning("Inventory is full!");
    }

    public void RemoveItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventorySlots.Length || inventorySlots[slotIndex].IsEmpty())
        {
            return;
        }

        InventoryItem item = inventorySlots[slotIndex].GetItem();
        items.Remove(item);
        inventorySlots[slotIndex].ClearSlot();
        ShiftItems();
    }

    private void ShiftItems()
    {
        ClearInventory();
        for (int i = 0; i < items.Count; i++)
        {
            inventorySlots[i].UpdateSlot(items[i]);
        }
    }

    public void ClearInventory()
    {
        items.Clear();
        foreach (var slot in inventorySlots)
        {
            slot.ClearSlot();
        }
    }
}
