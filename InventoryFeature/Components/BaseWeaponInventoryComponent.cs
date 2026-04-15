using System;
using Commands;
using HECSFramework.Core;

namespace Components
{
    [Serializable][Documentation(DocFeature.InventoryFeature, "BaseWeaponInventory")]
    public abstract class BaseWeaponInventoryComponent : InventoryComponent
    {
        public override void AddItem(int inventorySlot, InventoryItem item)
        {
            base.AddItem(inventorySlot, item);
            AddWeapon(item, inventorySlot);
        }

        public override void RemoveItem(int inventorySlot)
        {
            RemoveWeapon(inventorySlot);
            base.RemoveItem(inventorySlot);
        }

        public override void SwapItems(int firstSlot, int secondSlot)
        {
            base.SwapItems(firstSlot, secondSlot);

            if (Owner.World.TryGetEntityByComponent<PlayerCharacterComponent>(out var playerCharacter))
            {
                var firstItem = items[firstSlot];
                var secondItem = items[secondSlot];

                playerCharacter.Command(new UpdateWeaponSlotIndexCommand
                {
                    UniqueGuid = firstItem.UniqueID,
                    SlotIndex = firstSlot,
                    InventoryIndex = InventoryID,
                });

                playerCharacter.Command(new UpdateWeaponSlotIndexCommand
                {
                    UniqueGuid = secondItem.UniqueID,
                    SlotIndex = secondSlot,
                    InventoryIndex = InventoryID,
                });
            }


        }

        public override void MoveItem(int toSlot, int fromSlot)
        {
            base.MoveItem(toSlot, fromSlot);

            if (Owner.World.TryGetEntityByComponent<PlayerCharacterComponent>(out var playerCharacter))
            {
                playerCharacter.Command(new UpdateWeaponSlotIndexCommand
                {
                    UniqueGuid = items[toSlot].UniqueID,
                    SlotIndex = toSlot,
                    InventoryIndex = InventoryID
                });
            }
        }

        private void RemoveWeapon(int inventorySlot)
        {
            var itemsHolder = Owner.World.GetEntityBySingleComponent<GlobalLootFeatureTagComponent>().GetComponent<EquipItemsHolderComponent>();
            var item = items[inventorySlot];

            if (itemsHolder.TryGetContainerByID(item.ContainerIndex, out var itemContainer))
            {
                if (Owner.World.TryGetEntityByComponent<PlayerCharacterComponent>(out var playerCharacter))
                {
                    playerCharacter.Command(new RemoveWeaponCommand
                    {
                        UniqueID = item.UniqueID
                    });
                }
            }
        }

        private void AddWeapon(InventoryItem item, int inventorySlot)
        {
            var itemsHolder = Owner.World.GetEntityBySingleComponent<GlobalLootFeatureTagComponent>().GetComponent<EquipItemsHolderComponent>();

            if (itemsHolder.TryGetContainerByID(item.ContainerIndex, out var itemContainer))
            {
                if (Owner.World.TryGetEntityByComponent<PlayerCharacterComponent>(out var playerCharacter))
                {
                    playerCharacter.Command(new AddWeaponCommand
                    {
                        SlotIndex = inventorySlot,
                        InventoryID = InventoryID,
                        WeaponContainer = itemContainer,
                        UniqueGuid = item.UniqueID
                    });
                }
            }
        }
    }
}