using BluePrints.Identifiers;
using HECSFramework.Core;
using HECSFramework.Unity;
using Helpers;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Inventory, DocFeature.InventoryFeature, "here we set the rules for automove items by click behavior in UI inventory windows")]
    public sealed class AutoMoveByClickUIRulesComponent : BaseComponent
    {
        [SerializeField] private UIRule[] uiRules;

        public int[]  GetInventortiesIDs(int uiWindowID, int fromInventoryID)
        {
            for(int i = 0; i < uiRules.Length; i++)
            {
                if (uiRules[i].UiWindowID != uiWindowID)
                    continue;

                return uiRules[i].GetRuleByTargetInventory(fromInventoryID);
            }

            return null;
        }
    }

    [Serializable]
    public sealed class UIRule
    {
        [SerializeField][IdentifierDropDown(nameof(UIIdentifier))] private int uiWindowID;
        [SerializeField] private InventoryRule[] inventoryRules;

        public int UiWindowID => uiWindowID;
        
        public int[] GetRuleByTargetInventory(int fromInventoryID)
        {
            for(int i = 0; i < inventoryRules.Length; i++)
            {
                if (inventoryRules[i].FromInventoryID != fromInventoryID)
                    continue;

                return inventoryRules[i].ToInventoryIDs;
            }

            return null;
        }
    }

    [Serializable]
    public sealed class InventoryRule
    {
        [SerializeField][IdentifierDropDown(nameof(InventoryTypeIdentifier))] private int fromInventoryID;
        [SerializeField][IdentifierDropDown(nameof(InventoryTypeIdentifier))] private int[] toInventoryIDs;

        public int FromInventoryID => fromInventoryID;
        public int[] ToInventoryIDs => toInventoryIDs;
    }
}