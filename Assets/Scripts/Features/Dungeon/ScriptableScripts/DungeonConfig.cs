using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dungeon Config", menuName = "Scriptables/Configs/Dungeon")]
public class DungeonConfig : ScriptableObject
{
    public List<RoomConfig> Rooms;
}
