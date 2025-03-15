// PlayerInventoryActions.cs
using UnityEngine; // Importing Unity engine
using TMPro; // Importing TextMeshPro for UI text handling
using UnityEngine.UI; // Importing UI components
using UnityEngine.InputSystem; // Importing Input System for handling controller input

public class PlayerInventoryActions : MonoBehaviour
{
    public InventoryManager inventoryManager; // Reference to the InventoryManager to manage items
    public int playerGold = 100; // Player's initial gold amount
    public TMP_Text goldText; // UI element for displaying gold count
    public TMP_Text messageText; // UI element for displaying messages (e.g., errors, notifications)
    public GameObject infoPanel; // UI panel for displaying selected item information
    public Image infoItemImage; // UI image component for the selected item
    public TMP_Text infoItemName; // UI text component for the item's name
    public TMP_Text infoItemDescription; // UI text component for the item's description
    public InputActionReference buttonB; // Input action reference for the B button on a controller
    public InventoryItem[] itemsToAdd; // Array of items that can be added to inventory
    private int currentItemIndex = 0; // Keeps track of the next item to be added

    void Start()
    {
        UpdateGoldUI(); // Updates the gold UI when the game starts
        infoPanel.SetActive(false); // Hides the info panel initially
        buttonB.action.performed += ctx => AddItemToSlot(); // Adds event listener for the B button press
    }

    private void AddItemToSlot()
    {
        if (currentItemIndex >= itemsToAdd.Length || currentItemIndex >= inventoryManager.inventorySlots.Length)
        {
            messageText.text = "Inventory is full!";
            return;
        }

        InventoryItem newItem = new InventoryItem
        {
            itemName = itemsToAdd[currentItemIndex].itemName,
            description = itemsToAdd[currentItemIndex].description + "\nPrice: " + itemsToAdd[currentItemIndex].price + " gold",
            healthPoints = itemsToAdd[currentItemIndex].healthPoints,
            quantity = itemsToAdd[currentItemIndex].quantity,
            price = itemsToAdd[currentItemIndex].price,
            itemImage = itemsToAdd[currentItemIndex].itemImage
        };

        inventoryManager.AddItem(newItem);
        currentItemIndex++;
    }

    public void PurchaseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventoryManager.inventorySlots.Length)
        {
            messageText.text = "Invalid slot.";
            return;
        }

        InventorySlot slot = inventoryManager.inventorySlots[slotIndex];
        if (slot.IsEmpty())
        {
            messageText.text = "No item in this slot.";
            return;
        }

        InventoryItem item = slot.GetItem();
        if (playerGold >= item.price)
        {
            playerGold -= item.price;
            UpdateGoldUI();
            messageText.text = "Purchased " + item.itemName + "!";
        }
        else
        {
            messageText.text = "Not enough gold!";
        }
    }

    public void EatItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventoryManager.inventorySlots.Length)
        {
            messageText.text = "Invalid slot.";
            return;
        }

        InventorySlot slot = inventoryManager.inventorySlots[slotIndex];
        if (slot.IsEmpty())
        {
            messageText.text = "No item in this slot.";
            return;
        }

        InventoryItem item = slot.GetItem();
        messageText.text = "Ate " + item.itemName + " and gained " + item.healthPoints + " health!";
        inventoryManager.RemoveItem(slotIndex);
    }

    public void SelectItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventoryManager.inventorySlots.Length)
        {
            return;
        }

        InventorySlot slot = inventoryManager.inventorySlots[slotIndex];
        if (slot.IsEmpty())
        {
            infoPanel.SetActive(false);
            return;
        }

        InventoryItem item = slot.GetItem();
        infoPanel.SetActive(true);
        infoItemImage.sprite = item.itemImage;
        infoItemName.text = item.itemName;
        infoItemDescription.text = item.description + "\nPrice: " + item.price + " gold";
    }

    private void UpdateGoldUI()
    {
        goldText.text = "Gold: " + playerGold;
    }
}
