using System;
using HECSFramework.Core;

namespace Components
{
    [Serializable]
    [Documentation(DocFeature.InventoryFeature, "drone inventory component")]
    public sealed partial class DroneInventoryComponent : InventoryComponent, ISavebleComponent
    {
        public override int InventoryID => InventoryTypeIdentifierMap.LooterDroneInventory;
    }
}