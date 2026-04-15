using System;
using UnityEngine;
using Helpers;
using BluePrints.Identifiers;
using HECSFramework.Core;
using HECSFramework.Unity.Helpers;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using Components;

namespace HECSFramework.Unity.Configs
{
    [Obsolete]
    //[CreateAssetMenu(fileName = nameof(DropByGradeConfig), menuName = "GD/Configs/" + nameof(DropByGradeConfig), order = 1)]
    [Serializable]
    public class DropByGradeConfig : ScriptableObject
    {
        [SerializeField]/*[IdentifierDropDown(nameof(LootDropIdentifier))]*/ private int lootDropID;
        [SerializeField][ValueDropdown(nameof(GetContainers))] private EntityContainer lootVariantContainer;
        [SerializeField] private DropConfig[] dropGradeConfigs;

        public int LootDropID => lootDropID;
        public EntityContainer LootVariantContainer => lootVariantContainer;

        public void InitConfigs()
        {
            for(int i = 0; i < dropGradeConfigs.Length; i++)
            {
                dropGradeConfigs[i].InitConfigParameters();
            }
        }

        public bool TryGetDropConfigByGrade(int grade, out DropConfig config)
        {
            for(int i = 0; i < dropGradeConfigs.Length; i++)
            {
                if (dropGradeConfigs[i].LootGrade == grade)
                {
                    config = dropGradeConfigs[i];
                    return true;
                }
            }

            config = null;
            return false;
        }

        private IEnumerable<EntityContainer> GetContainers()
        {
            var containers = new SOProvider<EntityContainer>().GetCollection().Where(x => x is not PresetContainer && x.IsHaveComponent<LootVariantTagComponent>()
               && !x.ContainsComponent(ComponentProvider<IgnoreReferenceContainerTagComponent>.TypeIndex, true));

            return containers;
        }
    }
}