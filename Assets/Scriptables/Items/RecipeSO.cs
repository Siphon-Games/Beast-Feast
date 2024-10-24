using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IngredientEntry
{
    public ItemSO Item;
    public int QuantityNeeded;
}

[CreateAssetMenu(fileName = "RecipeSO", menuName = "Recipe")]
public class RecipeSO : ScriptableObject
{
    [field: SerializeField, ReadOnly]
    public string Id { get; private set; }
    public string Name;
    public string Description;
    public List<IngredientEntry> Ingredients;
    public ItemSO ResultingDish;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(Id))
        {
            Id = IDGenerator.GenerateRandomID();
        }
    }
}
