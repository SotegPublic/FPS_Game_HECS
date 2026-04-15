using System;
using HECSFramework.Core;

namespace Components
{
    [Serializable]
    [Documentation(DocFeature.InventoryFeature, "storage inventory component")]
    public sealed partial class StorageInventoryComponent : InventoryComponent, ISavebleComponent
    {
        public override int InventoryID => InventoryTypeIdentifierMap.PlayerStorageInventory;
    }
}