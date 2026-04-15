using System;
using HECSFramework.Core;

namespace Components
{
    [Serializable]
    [Documentation(DocFeature.InventoryFeature, "shelter storage inventory component")]
    public sealed partial class ShelterStorageInventoryComponent : InventoryComponent, ISavebleComponent
    {
        public override int InventoryID => InventoryTypeIdentifierMap.ShelterStorageInventory;
    }
}