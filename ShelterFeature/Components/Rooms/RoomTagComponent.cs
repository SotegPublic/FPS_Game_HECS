using HECSFramework.Core;
using System;
using Helpers;
using UnityEngine;
using BluePrints.Identifiers;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Shelter, Doc.Tag, "Room Tag Component")]
    public sealed class RoomTagComponent : BaseComponent
    {
        [SerializeField][IdentifierDropDown(nameof(RoomIdentifier))] private int roomID;
        [SerializeField] private Vector2Int roomGridIndex; 

        public int RoomID => roomID;
        public Vector2Int RoomGridIndex => roomGridIndex;

        public void SetRoomGridIndex(Vector2Int index)
        {
            roomGridIndex = index;
        }
    }
}