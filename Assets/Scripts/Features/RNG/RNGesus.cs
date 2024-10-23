using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

/// <summary>
/// This class provides two main methods:
/// - Roll: Returns a random item from a list based on regular distribution.
/// - WeightedRoll: Returns a random item from a list based on weighted distribution.
///
/// Both methods require the input list to implement the IChanceScore interface.
/// </summary>
public static class RNGesus
{
    /// <summary>
    /// Selects and returns a random item from the given list using a uniform distribution.
    /// </summary>
    /// <typeparam name="T">The type of items in the list.</typeparam>
    /// <param name="items">The list of items to choose from.</param>
    /// <returns>A randomly selected item from the list.</returns>
    /// <remarks>
    /// Example usage:
    /// <code>
    ///     List&lt;string&gt; fruits = new List&lt;string&gt; { "Apple", "Banana", "Cherry", "Date" };
    ///     string randomFruit = RNGesus.Roll(fruits);
    ///     Debug.Log($"Randomly selected fruit: {randomFruit}");
    /// </code>
    /// </remarks>
    public static T Roll<T>(IList<T> items)
    {
        CheckValidity(items);
        return items[Random.Range(0, items.Count)];
    }

    /// <summary>
    /// Returns a random item from the given list using a weighted distribution, where each item's chance of being selected
    /// is based on its individual <see cref="IChanceScore.chanceScore"/> value.
    /// </summary>
    /// <typeparam name="T">
    /// The type of items in the list. Each item must implement <see cref="IChanceScore"/> to define its chance of being selected.
    /// </typeparam>
    /// <param name="items">The list of items to choose from. Each item must have a defined chance score.</param>
    /// <returns>A randomly selected item from the list, based on the weighted chance scores of the items.</returns>
    /// <remarks>
    /// <code>
    ///     // Example list of items with weighted scores
    ///     public static List&lt;WeightedItem&gt; weightedItems = new List&lt;WeightedItem&gt;
    ///     {
    ///         new WeightedItem("Common", 70f),
    ///         new WeightedItem("Uncommon", 20f),
    ///         new WeightedItem("Rare", 9f),
    ///         new WeightedItem("Legendary", 1f)
    ///     };
    /// </code>
    ///
    /// Usage example:
    /// <code>
    ///     WeightedItem result = RNGesus.WeightedRoll(WeightedItem.weightedItems);
    ///     Debug.Log($"Weighted random item: {result.itemName}");
    /// </code>
    /// </remarks>
    public static T WeightedRoll<T>(IList<T> items)
        where T : IChanceScore
    {
        CheckValidity(items);

        float totalScore = items.Sum(item => item.ChanceScore);
        float randomValue = Random.Range(0f, totalScore);
        float currentScore = 0f;

        foreach (T item in items)
        {
            currentScore += item.ChanceScore;

            if (currentScore > randomValue)
            {
                return item;
            }
        }

        // This is a fallback to ensure that the method always returns a value.
        return items[Random.Range(0, items.Count)];
    }

    private static void CheckValidity<T>(IList<T> items)
    {
        if (items == null || items.Count == 0)
        {
            throw new ArgumentException("The collection cannot be null or empty");
        }
    }
}
