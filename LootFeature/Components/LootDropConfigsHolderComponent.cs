using System;
using UnityEngine;
using HECSFramework.Core;
using HECSFramework.Unity.Configs;
using HECSFramework.Unity;

namespace Components
{
    [Obsolete]
    [Serializable][Documentation(Doc.Loot, Doc.Holder, "here we hold loot drop configs")]
    public class LootDropConfigsHolderComponent : BaseComponent
    {
        [SerializeField] private DropByGradeConfig[] dropConfigs;

        public override void Init()
        {
            base.Init();

            for(int i = 0; i < dropConfigs.Length; i++)
            {
                dropConfigs[i].InitConfigs();
            }
        }

        public bool TryGetDropConfig(int lootDropID, int lootGrade, out (EntityContainer container, DropConfig config) dropTuple)
        {
            for (int i = 0; i < dropConfigs.Length; i++)
            {
                if (dropConfigs[i].LootDropID != lootDropID)
                    continue;

                if (dropConfigs[i].TryGetDropConfigByGrade(lootGrade, out var gradeConfig))
                {
                    dropTuple.container = dropConfigs[i].LootVariantContainer;
                    dropTuple.config = gradeConfig;
                    return true;
                }
            }

            dropTuple.container = null;
            dropTuple.config = null;
            return false;
        }
    }
}