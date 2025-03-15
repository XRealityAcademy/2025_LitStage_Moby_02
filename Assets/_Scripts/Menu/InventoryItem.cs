// InventoryItem.cs
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct InventoryItem
{
    public Sprite itemImage;
    public string itemName;
    public string description;
    public int healthPoints;
    public int quantity;
    public int price; // Added price field to store item cost
}