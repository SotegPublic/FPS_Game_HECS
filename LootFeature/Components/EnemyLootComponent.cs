using BluePrints.Identifiers;
using HECSFramework.Core;
using HECSFramework.Unity;
using HECSFramework.Unity.Helpers;
using Helpers;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Loot, "here we hold loot drop ID")]
    public sealed class EnemyLootComponent : BaseComponent
    {
        [SerializeField][EntityContainerIDDropDown(nameof(EnemyLootVariantTagComponent))] private int lootDropID;
        [SerializeField][IdentifierDropDown(nameof(GradeIdentifier))] private int lootGrade;

        public int LootDropID => lootDropID;
        public int LootGrade => lootGrade;

        public void SetLoot(int dropID, int grade)
        {
            lootDropID = dropID;
            lootGrade = grade;
        }
    }
}