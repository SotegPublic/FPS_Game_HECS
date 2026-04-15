using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Components
{
    [Serializable][Documentation(Doc.Shelter, Doc.Holder, "here we hold room containers")]
    public sealed class ShelterRoomContainersHolderComponent : BaseContainerHolderComponent<RoomTagComponent>
    {
        public bool TryGetContainerByRoomIdentifier(int roomIdentifier, out EntityContainer entityContainer)
        {
            entityContainer = containers.FirstOrDefault(x => x.GetComponent<RoomTagComponent>().RoomID == roomIdentifier);
            return entityContainer != null;
        }
    }
}