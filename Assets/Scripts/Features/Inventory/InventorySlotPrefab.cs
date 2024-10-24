using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotPrefab : InventorySlot
{
    [SerializeField]
    Image ItemImage;

    [SerializeField]
    TextMeshProUGUI ItemQuantityField;

    [SerializeField]
    Image ItemQuantityBackground;

    public InventorySlotPrefab(InventorySlot slot)
        : base(slot) { }

    public override void AddItem(InventoryItem item, int quantity)
    {
        base.AddItem(item, quantity);

        ItemQuantityBackground.gameObject.SetActive(quantity > 1);
        ItemQuantityField.text = Quantity.ToString();
        ItemImage.gameObject.SetActive(true);
        ItemImage.sprite = item.Icon;
    }

    public override void RemoveItem()
    {
        base.RemoveItem();
        ItemImage.gameObject.SetActive(false);
        ItemQuantityBackground.gameObject.SetActive(false);
    }

    public override void UpdateQuantity(int newAmount)
    {
        base.UpdateQuantity(newAmount);
        ItemQuantityBackground.gameObject.SetActive(Quantity > 1);
        ItemQuantityField.text = Quantity.ToString();
    }
}
