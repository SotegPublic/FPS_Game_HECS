using System;
using HECSFramework.Core;

namespace Components
{
    [Serializable][Documentation(DocFeature.InventoryFeature, "PlayerWeaponInventoryRight")]
    public sealed partial class WeaponInventoryRightComponent : BaseWeaponInventoryComponent, ISavebleComponent
    {
        public override int InventoryID => InventoryTypeIdentifierMap.PlayerWeaponInventoryRight;
    }
}