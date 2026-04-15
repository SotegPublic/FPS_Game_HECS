using BluePrints.Identifiers;
using HECSFramework.Core;
using Helpers;
using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Rewards, Doc.Visual, "by this tag we mark rewards for filtring in drop configs")]
    public sealed class EquipItemRewardTagComponent : BaseComponent
    {
        [SerializeField][IdentifierDropDown(nameof(LootItemTypeIdentifier))] private int equipTypeID;

        public int EquipTypeID => equipTypeID;
    }
}