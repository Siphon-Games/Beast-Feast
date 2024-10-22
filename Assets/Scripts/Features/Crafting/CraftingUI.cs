using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUI : MonoBehaviour
{
    [SerializeField]
    private RecipeItemUI recipeItemPrefab;

    [SerializeField]
    private Transform recipeItemParent;

    [SerializeField]
    private List<RecipeSO> recipes;

    [SerializeField]
    private InventoryUI inventoryPrefab;

    private void Start()
    {
        LoadRecipes();
    }

    private void LoadRecipes()
    {
        // Set initial position
        Vector2 position = new Vector2(0, 98);

        foreach (var recipe in recipes)
        {
            RecipeItemUI recipeItem = Instantiate(recipeItemPrefab, recipeItemParent);

            RectTransform rectTransform = recipeItem.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = position;
            position.y -= rectTransform.rect.height + 10f;

            recipeItem.Initialize(recipe, inventoryPrefab);
            recipeItem.CheckIfHasEnoughIngredients();
        }
    }
}
