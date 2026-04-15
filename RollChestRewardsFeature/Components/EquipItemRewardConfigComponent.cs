using BluePrints.Identifiers;
using HECSFramework.Core;
using Helpers;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Rewards, "here we hold base item container id")]
    public sealed class EquipItemRewardConfigComponent : BaseComponent
    {
        [SerializeField][EntityContainerIDDropDown(nameof(UpgradeComponent))] private int baseItemID;
        
        public int BaseItemID => baseItemID;
    }
}