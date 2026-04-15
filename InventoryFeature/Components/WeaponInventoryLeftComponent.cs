using System;
using Commands;
using HECSFramework.Core;

namespace Components
{
    [Serializable][Documentation(DocFeature.InventoryFeature, "PlayerWeaponInventoryLeft")]
    public sealed partial class WeaponInventoryLeftComponent : BaseWeaponInventoryComponent, ISavebleComponent
    {
        public override int InventoryID => InventoryTypeIdentifierMap.PlayerWeaponInventoryLeft;
    }
}