using Commands;
using Components;
using HECSFramework.Core;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace Systems
{
    [Obsolete]
    [Serializable]
    [Documentation(Doc.UI, Doc.Shelter, "RollChestsPanelUISystem")]
    public sealed class RollChestsPanelUISystem : BaseSystem, IAfterEntityInit, IReactGlobalCommand<ShowShelterPanelCommand>
    {
        [Required] private UIAccessProviderComponent accessProvider;

        private const int PANEL_ID = UIAccessIdentifierMap.ShelterStore;

        private TMP_Text chestsCountText;
        private CountersHolderComponent playerCountersHolder;
        private Button rollBtn;

        public override void InitSystem()
        {
        }

        public void AfterEntityInit()
        {
            var panelUIAccess = accessProvider.Get.GetUIAccess(PANEL_ID);
            rollBtn = panelUIAccess.GetButton(PANEL_ID);

            var panelBtn = accessProvider.Get.GetButton(PANEL_ID);
            chestsCountText = panelBtn.GetComponent<UIAccessMonoComponent>().GetTextMeshProUGUI(UIAccessIdentifierMap.Text);

            playerCountersHolder = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<CountersHolderComponent>();

            rollBtn.onClick.AddListener(RollItem);
            UpdateButtonInteractable();
        }

        private void RollItem()
        {
            //if(playerCountersHolder.TryGetIntCounter(CounterIdentifierContainerMap.RollChests, out var counter))
            //{
            //    var rollResult = Owner.World.Request<DropConfigRewardsRequestResult, GetDropConfigRewardsCommand>(
            //        new GetDropConfigRewardsCommand { GradeID = GradeIdentifierMap.Common, DropConfigID = LootDropIdentifierMap.RandomChestRoll });

            //    if (rollResult.ItemRewards == null)
            //        return;

            //    foreach(var item in rollResult.ItemRewards)
            //    {
            //        Owner.World.Request<AddItemRequestResult, AddItemRequestCommand>(new AddItemRequestCommand
            //        {
            //            ItemID = item,
            //            Count = 1,
            //            InventoryID = InventoryTypeIdentifierMap.PlayerStorageInventory
            //        });
            //    }
                                
            //    counter.ChangeValue(-1);
            //    chestsCountText.text = counter.Value.ToString();

            //    UpdateButtonInteractable();
            //}
        }

        private void UpdateButtonInteractable()
        {
            //if(playerCountersHolder.TryGetValue<int>(CounterIdentifierContainerMap.RollChests, out var value))
            //{
            //    rollBtn.interactable = value > 0 ? true : false;
            //}
            //else
            //{
            //    rollBtn.interactable = false;
            //}

        }

        public void CommandGlobalReact(ShowShelterPanelCommand command)
        {
            if (command.PanelID != PANEL_ID)
                return;

            // for dynamic UI elements
        }

        public override void BeforeDispose()
        {
            base.BeforeDispose();
            rollBtn.onClick.RemoveListener(RollItem);
        }
    }

    [Obsolete]
    public struct DropConfigRewardsRequestResult
    {
        public List<int> ItemRewards;
        public List<CurrencyReward> CurrencyRewards;
    }

    [Obsolete]
    public struct CurrencyReward
    {
        public int counterID;
        public int amount;
    }
}


namespace Commands
{
    [Obsolete]
    [Serializable]
    [Documentation(Doc.Loot, Doc.Global, "RollChestCommand")]
    public struct GetDropConfigRewardsCommand : IGlobalCommand
    {
        public int DropConfigID;
        public int GradeID;
    }
}