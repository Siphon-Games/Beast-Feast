using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public ItemSO Item;
    public int Quantity;

    public string Id => Item.Id;
    public Sprite Icon => Item.Icon;

    public InventoryItem(ItemSO item, int quantity)
    {
        Item = item;
        Quantity = quantity;
    }
}
