using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecipeItemUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Image recipeImage;

    [SerializeField]
    private TextMeshProUGUI ingredientNameText;

    [SerializeField]
    private TextMeshProUGUI quantityOwnedText;

    [SerializeField]
    private TextMeshProUGUI quantityNeededText;

    private RecipeSO recipe;
    private InventoryUI inventory;

    public void Initialize(RecipeSO recipe, InventoryUI inventory)
    {
        this.recipe = recipe;
        this.inventory = inventory;

        recipeImage.sprite = recipe.ResultingDish.Icon;
        recipeImage.enabled = true;

        UpdateIngredientUI();
        UpdateQuantityOwnedText();
    }

    private void UpdateIngredientUI()
    {
        StringBuilder ingredientsInfo = new StringBuilder();
        StringBuilder quantityNeededInfo = new StringBuilder();

        foreach (var ingredient in recipe.Ingredients)
        {
            ingredientsInfo.AppendLine(ingredient.Item.Name);
            quantityNeededInfo.AppendLine($"/{ingredient.QuantityNeeded}");
        }

        ingredientNameText.text = ingredientsInfo.ToString().TrimEnd('\n');
        quantityNeededText.text = quantityNeededInfo.ToString().TrimEnd('\n');
    }

    private void UpdateQuantityOwnedText()
    {
        StringBuilder quantityOwnedInfo = new StringBuilder();

        foreach (var ingredient in recipe.Ingredients)
        {
            var inventoryItem = GetInventoryItem(ingredient.Item);

            quantityOwnedInfo.AppendLine(
                inventoryItem != null ? inventoryItem.Quantity.ToString() : "0"
            );
        }

        quantityOwnedText.text = quantityOwnedInfo.ToString().TrimEnd('\n');
    }

    private bool HasEnoughIngredients()
    {
        foreach (var ingredient in recipe.Ingredients)
        {
            var inventoryItem = GetInventoryItem(ingredient.Item);

            if (inventoryItem == null || inventoryItem.Quantity < ingredient.QuantityNeeded)
            {
                return false;
            }
        }

        return true;
    }

    private InventoryItem GetInventoryItem(ItemSO item)
    {
        return inventory.items.FirstOrDefault(i => i.Item == item);
    }

    private void CraftRecipe(RecipeSO recipe)
    {
        // Reference inventory to add new dish
        InventoryItem newDish = new InventoryItem(recipe.ResultingDish, 1);
        inventory.AddNewItem(newDish, 1);
    }

    private void ConsumeIngredients(RecipeSO recipe)
    {
        List<IngredientEntry> ingredientsUsed = recipe.Ingredients;

        // Update quantity of ingredients in the inventory
        foreach (var ingredient in ingredientsUsed)
        {
            string ingredientName = ingredient.Item.Name;
            int quantityNeeded = ingredient.QuantityNeeded;

            InventorySlot slot = inventory.Slots.Find(slot => slot.Item?.Id == ingredient.Item.Id);

            slot.UpdateQuantity(-quantityNeeded);
            inventory.UpdateItemsList(ingredient.Item, -quantityNeeded);
        }

        UpdateQuantityOwnedText();
    }

    private void SetRecipeImageTransparent()
    {
        Color color = Color.gray;
        color.a = 0.7f;
        recipeImage.color = color;
    }

    public void CheckIfHasEnoughIngredients()
    {
        if (!HasEnoughIngredients())
        {
            SetRecipeImageTransparent();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (HasEnoughIngredients())
        {
            CraftRecipe(recipe);
            ConsumeIngredients(recipe);
            CheckIfHasEnoughIngredients();
        }
    }
}
