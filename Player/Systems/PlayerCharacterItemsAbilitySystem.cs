using Commands;
using Components;
using HECSFramework.Core;
using HECSFramework.Unity;
using Helpers;
using System;

namespace Systems
{
	[Serializable][Documentation(Doc.Inventory, Doc.Item, Doc.Player, "this system activate items from inventory")]
    public sealed class PlayerCharacterItemsAbilitySystem : BaseSystem,
        IReactCommand<ActivateUtilityItemCommand>, IReactCommand<DeactivateUtilityItemCommand>, IReactGlobalCommand<ActivatePlayerItemsCommand>,
        IReactCommand<ActivateArtefactItemCommand>, IReactCommand<DeactivateArtefactItemCommand>
    {
        [Required] private PlayerCharacterItemAbilitiesComponent itemAbilitiesComponent;
        [Required] private AbilitiesHolderComponent abilitiesHolderComponent;

        public void CommandGlobalReact(ActivatePlayerItemsCommand command)
        {
            var utilityInventory = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<UtilityInventoryComponent>();
            var artefactInventory = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<ArtefactInventoryComponent>();

            using var utilityItems = utilityInventory.GetItems();
            using var artefactItems = artefactInventory.GetItems();

            ActivateItems(utilityItems);
            ActivateItems(artefactItems);
        }

        private void ActivateItems(HECSPooledArray<System.Collections.Generic.KeyValuePair<int, InventoryItem>> items)
        {
            var itemsHolder = Owner.World.GetEntityBySingleComponent<GlobalLootFeatureTagComponent>().GetComponent<EquipItemsHolderComponent>();

            for (var i = 0; i < items.Count; i++)
            {
                if (items.Items[i].Value == null || items.Items[i].Value.IsAvailable)
                    continue;

                if (itemsHolder.TryGetContainerByID(items.Items[i].Value.ContainerIndex, out var itemContainer))
                {
                    var itemType = itemContainer.GetComponent<EquipItemTagComponent>().ItemType;

                    var abilityEntity = itemContainer.GetEntity();

                    switch (itemType)
                    {
                        case LootItemTypeIdentifierMap.Utility:

                            Owner.Command(new AddBuffByEntity
                            {
                                Buff = abilityEntity,
                                From = Owner,
                                To = Owner
                            });
                            break;
                        case LootItemTypeIdentifierMap.Artefact:

                            abilitiesHolderComponent.AddAbility(abilityEntity, true);
                            Owner.World.Command(new UpdateAbilityUIAfterAddCommand { Ability = abilityEntity });
                            break;
                        default:
                            break;
                    }

                    itemAbilitiesComponent.AddAbility(items.Items[i].Value.UniqueID, abilityEntity);
                }
            }
        }

        public void CommandReact(ActivateUtilityItemCommand command)
        {
            var buffEntity = command.ItemContainer.GetEntity();

            Owner.Command(new AddBuffByEntity
            {
                Buff = buffEntity,
                From = Owner,
                To = Owner
            });

            itemAbilitiesComponent.AddAbility(command.UniqueGuid, buffEntity);
        }

        public void CommandReact(DeactivateUtilityItemCommand command)
        {
            var removingBuffs = itemAbilitiesComponent.GetItemAbilities(command.UniqueGuid);

            for(int i = 0; i < removingBuffs.Count; i++)
            {
                var removingBuff = removingBuffs[i];

                Owner.Command(new RemoveBuffByEntity
                {
                    Buff = removingBuff
                });
            }

            itemAbilitiesComponent.RemoveItemAbilities(command.UniqueGuid);
        }

        public override void InitSystem()
        {
        }

        public void CommandReact(ActivateArtefactItemCommand command)
        {
            var abilityEntity = command.ItemContainer.GetEntity();
            abilitiesHolderComponent.AddAbility(abilityEntity, true);

            itemAbilitiesComponent.AddAbility(command.UniqueGuid, abilityEntity);
            Owner.World.Command(new UpdateAbilityUIAfterAddCommand { Ability = abilityEntity });
        }

        public void CommandReact(DeactivateArtefactItemCommand command)
        {
            var removingAbilities = itemAbilitiesComponent.GetItemAbilities(command.UniqueGuid);

            for (int i = 0; i < removingAbilities.Count; i++)
            {
                var removingAbility = removingAbilities[i];
                abilitiesHolderComponent.RemoveAbility(removingAbility);

                Owner.World.Command(new UpdateAbilityUIAfterRemoveCommand { Ability = removingAbility });
            }

            itemAbilitiesComponent.RemoveItemAbilities(command.UniqueGuid);
        }
    }
}