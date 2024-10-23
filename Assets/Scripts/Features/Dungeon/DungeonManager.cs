using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [SerializeField]
    DungeonConfig dungeonConfig;

    readonly List<DungeonRoom> spawnedRooms = new();

    private void Start()
    {
        GenerateDungeon(0);
    }

    void GenerateDungeon(int roomAmount)
    {
        GenerateRoom(DoorLocation.Right);
    }

    void GenerateRoom(DoorLocation enteredDoor, RoomConfig roomConfig = null)
    {
        var roomConfigToSpawn = roomConfig == null ? GetRoomToSpawn() : roomConfig;
        var spawnedRoom = Instantiate(roomConfigToSpawn.Room);
        var lastRoom = GetLastSpawnedRoom();

        if (lastRoom != null)
        {
            PositionRoom(spawnedRoom, lastRoom.transform, enteredDoor);
            lastRoom.gameObject.SetActive(false);
        }

        spawnedRooms.Add(spawnedRoom);
        List<RoomConfig> roomsToSpawn =
            new() { GetRoomToSpawn(), GetRoomToSpawn(), GetRoomToSpawn() };

        spawnedRoom.Initialize(OnEnterDoor, enteredDoor, roomsToSpawn, roomConfigToSpawn.Type);
    }

    void OnEnterDoor(Collider collider, DoorLocation location, RoomConfig roomConfig)
    {
        GenerateRoom(location, roomConfig);
    }

    RoomConfig GetRoomToSpawn()
    {
        return RNGesus.WeightedRoll(dungeonConfig.Rooms);
    }

    DungeonRoom GetLastSpawnedRoom()
    {
        if (spawnedRooms.Count == 0)
            return null;

        return spawnedRooms.Last();
    }

    void PositionRoom(DungeonRoom spawnedRoom, Transform lastTransform, DoorLocation enteredDoor)
    {
        Vector3 lastRoomSize = lastTransform.GetComponent<Renderer>().bounds.size;
        Vector3 newPosition =
            lastTransform.position + GetModificactionsByEnteredDoor(lastRoomSize, enteredDoor);
        spawnedRoom.transform.position = newPosition;
    }

    Vector3 GetModificactionsByEnteredDoor(Vector3 lastRoomsize, DoorLocation enteredDoor)
    {
        return enteredDoor switch
        {
            DoorLocation.Left => new(lastRoomsize.x * -1, 0, 0),
            DoorLocation.Right => new(lastRoomsize.x, 0, 0),
            DoorLocation.Top => new(0, 0, lastRoomsize.z),
            DoorLocation.Bottom => new(0, 0, lastRoomsize.z * -1),
            _ => new(0, 0, 0),
        };
    }
}

public struct RoomTypeAndSpawnLocation
{
    public RoomTypes type;
}
