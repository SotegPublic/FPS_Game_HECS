using BluePrints.Identifiers;
using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using System.Collections.Generic;
using Systems;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Shelter, Doc.Player, "here we hold players rooms in shelter")]
    public sealed class ShelterRoomsComponent : BaseComponent
    {
        [SerializeField][HideInInspector] private RoomInfo[] roomsArray;

        public RoomInfo[] RoomsArray => roomsArray;

        public int GetRoomIdentifier(Vector2Int roomIndex)
        {
            for (int i = 0; i < roomsArray.Length; i++)
            {
                if (roomsArray[i].RoomMatrixIndex == roomIndex)
                {
                    return roomsArray[i].RoomID;
                }
            }

            return 0;
        }

        public void UpdateRoomsArray(RoomIdentifier[,] matrix)
        {
            roomsArray = new RoomInfo[matrix.GetLength(0) * matrix.GetLength(1)];

            var index = 0;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    roomsArray[index] = new RoomInfo
                    {
                        RoomMatrixIndex = new Vector2Int(i, j)
                    };

                    if (matrix[i, j] != null)
                        roomsArray[index].RoomID = matrix[i, j].Id;

                    index++;
                }
            }
        }

        public void AddNewRoom(Vector2Int gridCoordinate, int roomID)
        {
            var targetInfo = FindInfo(gridCoordinate);

            if (targetInfo != null)
                targetInfo.RoomID = roomID;
        }

        private RoomInfo FindInfo(Vector2Int gridCoordinate)
        {
            int left = 0;
            int right = roomsArray.Length - 1;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                var current = roomsArray[mid];

                if (gridCoordinate.x == current.RoomMatrixIndex.x)
                {
                    if (gridCoordinate.y == current.RoomMatrixIndex.y)
                    {
                        return current;
                    }
                    else if (gridCoordinate.y < current.RoomMatrixIndex.y)
                    {
                        right = mid - 1;
                    }
                    else
                    {
                        left = mid + 1;
                    }
                }
                else if (gridCoordinate.x < current.RoomMatrixIndex.x)
                {
                    right = mid - 1;
                }
                else
                {
                    left = mid + 1;
                }
            }

            return null;
        }
    }

    [Serializable]
    public class RoomInfo
    {
        public int RoomID;
        public Vector2Int RoomMatrixIndex;
    }
}