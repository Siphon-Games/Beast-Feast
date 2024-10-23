using System.Collections.Generic;
using UnityEngine;

public class WeightedItem : IChanceScore
{
    public float ChanceScore { get; }
    public string itemName { get; }

    public WeightedItem(string itemName, float chanceScore)
    {
        this.ChanceScore = chanceScore;
        this.itemName = itemName;
    }
}
