using System;
using Commands;
using HECSFramework.Core;
using HECSFramework.Unity;

namespace Components
{
    [Serializable]
    [Documentation(DocFeature.InventoryFeature, "artefact inventory component")]
    public sealed partial class ArtefactInventoryComponent : InventoryComponent, ISavebleComponent
    {
        public override int InventoryID => InventoryTypeIdentifierMap.PlayerArtefactInventory;

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
                RemoveAbilities(item.UniqueID);
            }
        }

        private void RemoveAbilities(Guid itemGuid)
        {
            if (Owner.World.TryGetEntityByComponent<PlayerCharacterComponent>(out var playerCharacter))
            {
                playerCharacter.Command(new DeactivateArtefactItemCommand
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
                AddAbilities(item.UniqueID, itemContainer);
            }
        }

        private void AddAbilities(Guid itemGuid, EntityContainer itemContainer)
        {
            if (Owner.World.TryGetEntityByComponent<PlayerCharacterComponent>(out var playerCharacter))
            {
                playerCharacter.Command(new ActivateArtefactItemCommand
                {
                    ItemContainer = itemContainer,
                    UniqueGuid = itemGuid
                });
            }
        }
    }
}