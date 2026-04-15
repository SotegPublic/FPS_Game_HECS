using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using Helpers;
using BluePrints.Identifiers;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Player, "AvailableRoomsTypesComponent")]
    public sealed class AvailableRoomsTypesComponent : BaseComponent
    {
        [SerializeField][IdentifierDropDown(nameof(RoomIdentifier))] private int[] availableRooms;

        public int[] AvailableRooms => availableRooms;
    }
}