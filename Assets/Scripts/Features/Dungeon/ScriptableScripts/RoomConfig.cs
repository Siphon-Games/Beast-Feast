using System;
using UnityEngine;

[Serializable]
public class RoomConfig : IChanceScore
{
    public RoomTypes Type;
    public RoomSpawnOrder spawnOrder;

    [SerializeField]
    float spawnChance;
    public float ChanceScore => spawnChance;

    public DungeonRoom Room;
}
