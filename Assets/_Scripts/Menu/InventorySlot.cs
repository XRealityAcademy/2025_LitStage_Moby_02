// InventorySlot.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Image itemImage;
    public TMP_Text itemQuantity;
    private bool isEmpty = true;
    private InventoryItem currentItem;

    public void UpdateSlot(InventoryItem item)
    {
        currentItem = item;
        isEmpty = false;
        itemImage.sprite = item.itemImage;
        itemImage.enabled = true;
        itemQuantity.text = item.quantity > 1 ? item.quantity.ToString() : "";
    }

    public void ClearSlot()
    {
        isEmpty = true;
        currentItem = default;
        itemImage.sprite = null;
        itemImage.enabled = false;
        itemQuantity.text = "";
    }

    public bool IsEmpty()
    {
        return isEmpty;
    }

    public InventoryItem GetItem()
    {
        return currentItem;
    }
}