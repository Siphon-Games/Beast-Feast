using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI
    : Inventory,
        IBeginDragHandler,
        IEndDragHandler,
        IDragHandler,
        IPointerClickHandler
{
    [SerializeField]
    InventorySlotPrefab _slotPrefab;

    [SerializeField]
    RectTransform _parentRectTransform;

    static InventorySlot selectedSlot;
    InventorySlot originSlot;

    Image draggingItem;

    [field: SerializeField]
    public List<InventoryItem> items { get; private set; }

    [SerializeField]
    Button SaveButton,
        LoadButton;

    enum ItemMoveType
    {
        DragAndDrop,
        ClickHold,
    }

    [SerializeField]
    List<ItemMoveType> itemMoveTypes;

    bool isDragging = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!itemMoveTypes.Contains(ItemMoveType.ClickHold) || isDragging)
        {
            return;
        }

        bool isRightClick = eventData.button == PointerEventData.InputButton.Right;
        if (selectedSlot == null)
        {
            PickUpItem(eventData);
        }
        else
        {
            TryMoveItem(eventData, isRightClick);
        }
    }

    private void Update()
    {
        if (selectedSlot != null && draggingItem != null)
        {
            if (draggingItem != null)
            {
                var screenPoint = Input.mousePosition;
                screenPoint.z = 100;
                draggingItem.transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
            }
        }
    }

    public override void InitializeInventory()
    {
        LoadButton.onClick.AddListener(LoadInventory);
        SaveButton.onClick.AddListener(Save);

        for (int i = 0; i < InventorySize; i++)
        {
            var spawnedSlot = Instantiate(_slotPrefab);
            spawnedSlot.GetComponent<RectTransform>().SetParent(_parentRectTransform, false);
            Slots.Add(spawnedSlot);
        }

        List<(InventoryItem item, int quantity)> itemsWithQuantities = new();

        items.ForEach(item =>
        {
            if (item.Quantity > 0)
            {
                itemsWithQuantities.Add(new(item, item.Quantity));
            }
        });

        AddItems(itemsWithQuantities);
    }

    public void AddNewItem(InventoryItem item, int quantity)
    {
        List<(InventoryItem, int)> newDishList = new List<(InventoryItem, int)>
        {
            (item, quantity),
        };

        AddItems(newDishList);
        UpdateItemsList(item.Item, quantity);
    }

    public void UpdateItemsList(ItemSO item, int quantity)
    {
        var existingItem = items.FirstOrDefault(i => i.Item.Id == item.Id);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            items.Add(new InventoryItem(item, quantity));
        }
    }

    public void LoadInventory()
    {
        var savedInventory = Load() ?? throw new System.Exception("No saved inventory present");

        Slots.ForEach(slot => slot.RemoveItem());
        savedInventory.ForEach(savedSlot =>
        {
            Slots[savedSlot.Index].AddItem(savedSlot.Item, savedSlot.Quantity);
        });
    }

    public void PickUpItem(PointerEventData eventData)
    {
        var targetSlot =
            eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<InventorySlot>();
        if (targetSlot != null && targetSlot.Item != null && targetSlot.Item.Id != null)
        {
            if (draggingItem != null)
            {
                Destroy(draggingItem.gameObject);
            }

            selectedSlot = Instantiate(_slotPrefab);
            selectedSlot.AddItem(targetSlot.Item, targetSlot.Quantity);
            draggingItem = selectedSlot.GetComponent<Image>();
            draggingItem.transform.SetParent(
                _parentRectTransform.GetComponentInParent<Canvas>().transform,
                false
            );
            draggingItem.gameObject.AddComponent<CanvasGroup>().blocksRaycasts = false;
            draggingItem.enabled = false;
            var screenPoint = Input.mousePosition;
            screenPoint.z = 100;
            draggingItem.transform.position = Camera.main.ScreenToWorldPoint(screenPoint);

            targetSlot.RemoveItem();
            originSlot = targetSlot;
            return;
        }
    }

    public void MoveDraggedItem(PointerEventData eventData)
    {
        var pointerObject = eventData.pointerCurrentRaycast.gameObject;
        if (pointerObject == null)
        {
            if (originSlot != null)
            {
                ReturnToOrigin();
            }
            return;
        }

        var targetSlot = pointerObject.GetComponentInParent<InventorySlot>();
        if (targetSlot != null)
        {
            if (selectedSlot != null)
            {
                var targetItem = targetSlot.Item;
                var originItem = selectedSlot.Item;

                // if target has no item we dump
                if (targetItem == null)
                {
                    targetSlot.AddItem(originItem, selectedSlot.Quantity);
                    ResetSelectedSlot();
                    return;
                }

                // if both are the same we update amount
                if (targetItem.Id == originItem.Id)
                {
                    targetSlot.UpdateQuantity(selectedSlot.Quantity);
                    ResetSelectedSlot();
                    return;
                }

                // else we switch
                InventoryItem item = targetSlot.Item;
                int quantity = targetSlot.Quantity;
                targetSlot.AddItem(originItem, selectedSlot.Quantity);
                originSlot.AddItem(item, quantity);
                ResetSelectedSlot();
            }
        }
    }

    void ResetSelectedSlot()
    {
        Destroy(selectedSlot.gameObject);
        selectedSlot = null;
    }

    void ReturnToOrigin()
    {
        originSlot.AddItem(selectedSlot.Item, selectedSlot.Quantity);
        Destroy(selectedSlot.gameObject);
        selectedSlot = null;
    }

    public void TryMoveItem(PointerEventData eventData, bool isRightClick = false)
    {
        var pointerObject = eventData.pointerCurrentRaycast.gameObject;

        if (pointerObject == null)
        {
            if (originSlot != null)
            {
                ReturnToOrigin();
            }
            return;
        }

        var targetSlot = pointerObject.GetComponentInParent<InventorySlot>();
        if (targetSlot != null)
        {
            if (selectedSlot != null)
            {
                var targetItem = targetSlot.Item;
                var originItem = selectedSlot.Item;
                int quantityAmount = isRightClick ? 1 : selectedSlot.Quantity;
                int originQuantityChange = selectedSlot.Quantity - quantityAmount;

                // if target has no item we dump
                if (targetItem == null)
                {
                    targetSlot.AddItem(originItem, quantityAmount);
                    if (isRightClick)
                    {
                        selectedSlot.UpdateQuantity(-quantityAmount);
                    }

                    if (originQuantityChange == 0)
                    {
                        ResetSelectedSlot();
                    }
                    return;
                }

                // if both are the same we update amount
                if (targetItem.Id == originItem.Id)
                {
                    targetSlot.UpdateQuantity(quantityAmount);
                    selectedSlot.UpdateQuantity(-quantityAmount);
                    if (originQuantityChange == 0)
                    {
                        ResetSelectedSlot();
                    }
                    return;
                }

                // else we switch
                if (!isRightClick)
                {
                    InventoryItem item = targetItem;
                    int quantity = targetSlot.Quantity;
                    targetSlot.AddItem(originItem, selectedSlot.Quantity);
                    selectedSlot.AddItem(item, quantity);
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!itemMoveTypes.Contains(ItemMoveType.DragAndDrop))
        {
            return;
        }

        if (selectedSlot == null)
        {
            PickUpItem(eventData);
            isDragging = true;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        MoveDraggedItem(eventData);
        isDragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggingItem != null)
        {
            var screenPoint = Input.mousePosition;
            screenPoint.z = 100;
            draggingItem.transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
        }
    }

    public override void MoveItem(
        InventorySlot originSlot,
        InventorySlot targetSlot,
        int moveAmount
    )
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
}
