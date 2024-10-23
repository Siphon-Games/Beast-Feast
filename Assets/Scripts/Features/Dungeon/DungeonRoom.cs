using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DungeonRoom : MonoBehaviour
{
    [SerializeField]
    DungeonRoomDoor leftDoor,
        rightDoor,
        topDoor,
        bottomDoor;

    [SerializeField]
    TextMeshProUGUI roomTitle;

    public void Initialize(
        Action<Collider, DoorLocation, RoomConfig> _onEnterDoor,
        DoorLocation lastEnteredDoor,
        List<RoomConfig> roomConfigs,
        RoomTypes type
    )
    {
        roomTitle.text = type.ToString();
        InitializeDoorTriggers(_onEnterDoor, lastEnteredDoor, roomConfigs);
    }

    void InitializeDoorTriggers(
        Action<Collider, DoorLocation, RoomConfig> _onEnterDoor,
        DoorLocation lastEnteredDoor,
        List<RoomConfig> roomConfigs
    )
    {
        var activeDoors = DisableDoor(
            lastEnteredDoor,
            new() { leftDoor, rightDoor, topDoor, bottomDoor }
        );

        int index = 0;

        activeDoors.ForEach(door =>
        {
            door.SetOnEnterDoor(_onEnterDoor, roomConfigs[index]);
            index++;
        });
    }

    private List<DungeonRoomDoor> DisableDoor(
        DoorLocation lastEnteredDoor,
        List<DungeonRoomDoor> doors
    )
    {
        switch (lastEnteredDoor)
        {
            case DoorLocation.Left:
                doors.Remove(rightDoor);
                rightDoor.gameObject.SetActive(false);
                break;
            case DoorLocation.Right:
                doors.Remove(leftDoor);
                leftDoor.gameObject.SetActive(false);
                break;
            case DoorLocation.Top:
                doors.Remove(bottomDoor);
                bottomDoor.gameObject.SetActive(false);
                break;
            case DoorLocation.Bottom:
                doors.Remove(topDoor);
                topDoor.gameObject.SetActive(false);
                break;
        }

        return doors;
    }
}
