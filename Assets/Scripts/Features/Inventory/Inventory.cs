using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class SavedSlot
{
    public InventoryItem Item;
    public int Quantity;
    public int Index;

    public SavedSlot(InventoryItem inventoryItem, int quantity, int index)
    {
        Item = inventoryItem;
        Quantity = quantity;
        Index = index;
    }
}

public class SavedSlots
{
    public List<SavedSlot> Slots;
}

public class Inventory : MonoBehaviour
{
    [field: SerializeField]
    public int InventorySize { get; private set; }
    public List<InventorySlot> Slots { get; private set; } = new();

    [SerializeField]
    string fileSaveString;

    private void Start()
    {
        InitializeInventory();
    }

    public virtual void InitializeInventory()
    {
        Slots = new List<InventorySlot>(new InventorySlot[InventorySize]);
    }

    public void Save()
    {
        string filePath = Path.Combine(Application.persistentDataPath, $"{fileSaveString}.json");

        int index = 0;
        List<SavedSlot> savedSlots = new();

        Slots.ForEach(slot =>
        {
            if (slot.Item != null)
            {
                savedSlots.Add(new SavedSlot(slot.Item, slot.Quantity, index));
            }
            index++;
        });
        JsonSerializerSettings settings =
            new() { TypeNameHandling = TypeNameHandling.All, Formatting = Formatting.Indented };
        File.WriteAllText(
            filePath,
            JsonConvert.SerializeObject(new SavedSlots() { Slots = savedSlots }, settings)
        );
    }

    public virtual List<SavedSlot> Load()
    {
        string filePath = Path.Combine(Application.persistentDataPath, $"{fileSaveString}.json");
        string savedInventory = File.ReadAllText(filePath);
        JsonSerializerSettings settings = new() { TypeNameHandling = TypeNameHandling.All };
        Debug.Log($"loaded from {filePath}");
        return (JsonConvert.DeserializeObject<SavedSlots>(savedInventory, settings).Slots);
    }

    public virtual void AddItems(List<(InventoryItem item, int quantity)> newItems)
    {
        newItems.ForEach(newItem =>
        {
            // Find existing item by id, if exists then update quantity
            var existingItem = Slots.Find(slot => slot.Item?.Id == newItem.item.Id);
            if (existingItem != null)
            {
                existingItem.UpdateQuantity(newItem.quantity);
                return;
            }

            var (item, quantity) = newItem;
            var slot = FindItemSlot(item);

            if (slot == null)
            {
                throw new Exception($"No Free Slot Found For Item {item.Id}");
            }

            slot.AddItem(item, quantity);
        });
    }

    public bool IsAllowedToMove(InventorySlot originSlot, InventorySlot targetSlot)
    {
        bool isTargetSlotEmpty = targetSlot.Item == null;
        bool isTargetItemSameAsOrigin =
            targetSlot.Item != null && targetSlot.Item.Id == originSlot.Item.Id;

        return isTargetItemSameAsOrigin || isTargetSlotEmpty;
    }

    public virtual void MoveItem(InventorySlot originSlot, InventorySlot targetSlot, int moveAmount)
    {
        bool isTargetItemSameAsOrigin =
            targetSlot.Item != null && targetSlot.Item.Id == originSlot.Item.Id;

        if (isTargetItemSameAsOrigin)
        {
            targetSlot.UpdateQuantity(moveAmount);
        }
        else
        {
            targetSlot.AddItem(originSlot.Item, moveAmount);
        }

        originSlot.UpdateQuantity(-moveAmount);
    }

    public virtual void RemoveItems(List<InventoryItem> removedItems)
    {
        removedItems.ForEach(item =>
        {
            var slot = FindItemSlot(item);
            if (slot == null)
            {
                throw new Exception($"No Free Slot Found For Item {item.Id}");
            }
            slot.RemoveItem();
        });
    }

    InventorySlot FindItemSlot(InventoryItem item)
    {
        var existingSlotItem = Slots.Find(slot => slot.Item?.Id == item.Id);

        if (existingSlotItem != null)
        {
            return existingSlotItem;
        }

        return Slots.Find(slot => slot.Item == null);
    }
}
