using System;
using System.Xml;
using Commands;
using HECSFramework.Core;
using HECSFramework.Unity;

namespace Components
{
    [Serializable]
    [Documentation(DocFeature.InventoryFeature, "utility inventory component")]
    public sealed partial class UtilityInventoryComponent : InventoryComponent, ISavebleComponent
    {
        public override int InventoryID => InventoryTypeIdentifierMap.PlayerUtilityInventory;

        public override void AddItem(int inventorySlot, InventoryItem item)
        {
            base.AddItem(inventorySlot, item);
            ActivateItem(item);
        }

        public override void RemoveItem(int inventorySlot)
        {
            DeactivateItem(inventorySlot);
            base.RemoveItem(inventorySlot);
        }

        private void DeactivateItem(int inventorySlot)
        {
            var itemsHolder = Owner.World.GetEntityBySingleComponent<GlobalLootFeatureTagComponent>().GetComponent<EquipItemsHolderComponent>();
            var item = items[inventorySlot];

            if (itemsHolder.TryGetContainerByID(item.ContainerIndex, out var itemContainer))
            {
                RemoveBuffs(item.UniqueID);
            }
        }

        private void RemoveBuffs(Guid itemGuid)
        {
            if (Owner.World.TryGetEntityByComponent<PlayerCharacterComponent>(out var playerCharacter))
            {
                playerCharacter.Command(new DeactivateUtilityItemCommand
                {
                    UniqueGuid = itemGuid
                });
            }
        }

        private void ActivateItem(InventoryItem item)
        {
            var itemsHolder = Owner.World.GetEntityBySingleComponent<GlobalLootFeatureTagComponent>().GetComponent<EquipItemsHolderComponent>();

            if (itemsHolder.TryGetContainerByID(item.ContainerIndex, out var itemContainer))
            {
                AddBuffs(item.UniqueID, itemContainer);
            }
        }

        private void AddBuffs(Guid itemGuid, EntityContainer itemContainer)
        {
            if (Owner.World.TryGetEntityByComponent<PlayerCharacterComponent>(out var playerCharacter))
            {
                playerCharacter.Command(new ActivateUtilityItemCommand
                {
                    ItemContainer = itemContainer,
                    UniqueGuid = itemGuid
                });
            }
        }
    }
}