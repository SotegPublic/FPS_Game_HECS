using System;
using Commands;
using Components;
using HECSFramework.Core;
using HECSFramework.Unity;
using UnityEngine;

namespace Systems
{
    [Serializable]
    [Documentation(Doc.Character, Doc.Inventory, "this system responding for adding and removing weapon abilities")]
    public sealed class PlayerCharacterWeaponAbilitiesSystem : BaseSystem,
        IReactCommand<AddWeaponCommand>, IReactCommand<RemoveWeaponCommand>,
        IReactCommand<UpdateWeaponSlotIndexCommand>, IReactGlobalCommand<ActivateWeaponsCommand>
    {
        [Required] private AbilitiesHolderComponent abilitiesHolder;
        [Required] private PlayerCharacterWeaponAbilitiesComponent weaponAbilitiesComponent;

        public void CommandReact(RemoveWeaponCommand command)
        {
            Owner.Command(new BeforeCommand<RemoveWeaponCommand> { Value = command });
            
            var removingAbility = weaponAbilitiesComponent.AbilitiesByWeapon[command.UniqueID];
            DeactivateBuffs(removingAbility);
            abilitiesHolder.RemoveAbility(removingAbility);
            weaponAbilitiesComponent.AbilitiesByWeapon.Remove(command.UniqueID);
        }

        public void CommandReact(UpdateWeaponSlotIndexCommand command)
        {
            var abilityForUpdate = weaponAbilitiesComponent.AbilitiesByWeapon[command.UniqueGuid];
            
            abilityForUpdate.GetOrAddComponent<InventorySlotIndexHolderComponent>()
                .SetValues(command.SlotIndex, command.InventoryIndex);
        }

        public void CommandReact(AddWeaponCommand command)
        {
            AddWeapon(command);
            ActivateBuffs(command);
            Owner.Command(new AfterCommand<AddWeaponCommand> { Value = command });
        }

        private void ActivateBuffs(AddWeaponCommand command)
        {
            var weaponEntity = weaponAbilitiesComponent.AbilitiesByWeapon[command.UniqueGuid];
            using var abilities = abilitiesHolder.GetAllAbilities();

            for (int i = 0; i < abilities.Count; i++)
            {
                if (abilities.Items[i].ContainsMask<WeaponBuffTagComponent>())
                    abilities.Items[i].Command(new AddBuffToWeaponCommand { Weapon = weaponEntity });
            }
        }

        private void DeactivateBuffs(Entity removingAbility)
        {
            using var abilities = abilitiesHolder.GetAllAbilities();

            for (int i = 0; i < abilities.Count; i++)
            {
                if (abilities.Items[i].ContainsMask<WeaponBuffTagComponent>())
                    abilities.Items[i].Command(new RemoveBuffFromWeaponCommand { Weapon = removingAbility });
            }
        }

        private void AddWeapon(AddWeaponCommand command)
        {
            AddAbility(command.WeaponContainer, command.UniqueGuid, command.SlotIndex, command.InventoryID);

        }

        public void CommandGlobalReact(ActivateWeaponsCommand command)
        {
            var weaponLeftInventory = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<WeaponInventoryLeftComponent>();
            var weaponRightInventory = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<WeaponInventoryRightComponent>();
            var itemsHolder = Owner.World.GetEntityBySingleComponent<GlobalLootFeatureTagComponent>().GetComponent<EquipItemsHolderComponent>();

            InitWeapons(weaponLeftInventory, itemsHolder);
            InitWeapons(weaponRightInventory, itemsHolder);
        }

        public override void InitSystem()
        {
        }

        private void InitWeapons(IInventoryComponent inventory, EquipItemsHolderComponent itemsHolder)
        {
            using var items = inventory.GetItems();

            for (var i = 0; i < items.Count; i++)
            {
                if (items.Items[i].Value == null || items.Items[i].Value.IsAvailable)
                    continue;

                if (itemsHolder.TryGetContainerByID(items.Items[i].Value.ContainerIndex, out var itemContainer))
                {
                    var itemType = itemContainer.GetComponent<EquipItemTagComponent>().ItemType;

                    if (itemType == LootItemTypeIdentifierMap.Weapon)
                    {
                        AddAbility(itemContainer, items.Items[i].Value.UniqueID, items.Items[i].Key, inventory.InventoryID);
                    }
                }
            }
        }

        private void AddAbility(EntityContainer weaponAbilitiy, Guid uniqueID, int slotIndex, int inventoryID)
        {
            var ability = weaponAbilitiy.GetEntity();
            ability.GetOrAddComponent<InventorySlotIndexHolderComponent>().SetValues(slotIndex, inventoryID);
            
            abilitiesHolder.AddAbility(ability, true);
            weaponAbilitiesComponent.AbilitiesByWeapon.Add(uniqueID, ability);
        }
    }
}

