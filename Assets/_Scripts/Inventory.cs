using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Inventory Settings")]
    [Tooltip("Assign your three UI Image components that represent the inventory slots.")]
    public Image[] inventorySlots; // Expect 3 slots (e.g., assigned in Inspector)
    [Tooltip("Assign the corresponding Text components that display the count for each slot.")]
    public Text[] slotNumbers;     // Corresponding UI Text elements
    [Tooltip("Assign the whale sprite that will appear in the inventory.")]
    public Sprite whaleSprite;     // The whale sprite to use in the inventory
    // A simple flag to track if the whale is still alive.
    private bool whaleAlive = true;
    void Update()
    {
        // Listen for the space key press.
        if (Input.GetKeyDown(KeyCode.Space) && whaleAlive)
        {
            ShootWhale();
        }
    }
    // Simulates shooting the whale
    void ShootWhale()
    {
        whaleAlive = false;  // Mark the whale as dead.
        Debug.Log("Whale shot! It is now dead.");
        // After the whale dies, add it to the inventory.
        AddWhaleToInventory();
    }
    // Finds the first empty slot and updates it with the whale sprite and count.
    void AddWhaleToInventory()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            // Assuming an empty slot has no sprite assigned (you could also check for a default "empty" sprite)
            if (inventorySlots[i].sprite == null)
            {
                inventorySlots[i].sprite = whaleSprite;
                // Update the UI number to "1" indicating the whale count.
                slotNumbers[i].text = "1";
                Debug.Log("Whale added to inventory slot " + (i + 1));
                break;  // Stop after filling the first empty slot.
            }
        }
    }
}
