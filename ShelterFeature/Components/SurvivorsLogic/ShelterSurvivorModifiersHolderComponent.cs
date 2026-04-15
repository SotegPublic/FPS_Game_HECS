using HECSFramework.Core;
using HECSFramework.Unity;
using Helpers;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Shelter, "here we hold modifiers for survivors")]
    public sealed class ShelterSurvivorModifiersHolderComponent : BaseComponent
    {
        public UnityIntModifier[] Modifiers;
        private HECSList<ActiveModifiers> activeModifiersByID;

        public override void Init()
        {
            base.Init();
            activeModifiersByID = new HECSList<ActiveModifiers>(Modifiers.Length);

            for(int i = 0; i < Modifiers.Length; i++)
            {
                var activeModifiers = new ActiveModifiers();
                activeModifiers.ModifierID = Modifiers[i].ModifierCounterID;
                activeModifiersByID.Add(activeModifiers);
            }
        }
        
        public void AddActiveModifier(DefaultIntModifier modifier)
        {
            for(int i = 0; i < activeModifiersByID.Count; i++)
            {
                if (activeModifiersByID[i].ModifierID != modifier.ModifierCounterID)
                    continue;

                activeModifiersByID[i].ModifierGuids.Enqueue(modifier);
                return;
            }

            var activeModifiers = new ActiveModifiers();
            activeModifiers.ModifierID = modifier.ModifierCounterID;
            activeModifiers.ModifierGuids.Enqueue(modifier);
            activeModifiersByID.Add(activeModifiers);
        }

        public DefaultIntModifier GetModifierForRemove(int modifierID)
        {
            for (int i = 0; i < activeModifiersByID.Count; i++)
            {
                if (activeModifiersByID[i].ModifierID != modifierID)
                    continue;

                return activeModifiersByID[i].ModifierGuids.Dequeue();
            }

            return null;
        }
    }

    [Serializable]
    public class ActiveModifiers
    {
        public int ModifierID;
        public Queue<DefaultIntModifier> ModifierGuids = new Queue<DefaultIntModifier>(32);
    }

    [Serializable]
    public class UnityIntModifier : BaseModifier<int>
    {
        [SerializeField]
        private int value;

        [SerializeField]
        private ModifierCalculationType calculationType;

        [SerializeField]
        private ModifierValueType modifierType;

        [SerializeField, ReadOnly]
        private string guid;

        [SerializeField, IdentifierDropDown(nameof(CounterIdentifierContainer))]
        private int modifierIdentifier;

        private Guid currentGuid;

        public override int ModifierCounterID { get => modifierIdentifier; set => modifierIdentifier = value; }

        public override int GetValue { get => value; set => this.value = value; }
        public override ModifierCalculationType GetCalculationType { get => calculationType; set => calculationType = value; }
        public override ModifierValueType GetModifierType { get => modifierType; set => modifierType = value; }

        public override Guid ModifierGuid
        {
            get
            {
                if (currentGuid != Guid.Empty)
                    return currentGuid;

                if (string.IsNullOrEmpty(guid))
                {
                    currentGuid = Guid.NewGuid();
                    guid = currentGuid.ToString();
                }
                else
                {
                    currentGuid = new Guid(guid);
                    return currentGuid;
                }

                return currentGuid;
            }
            set
            {
                currentGuid = value;
            }
        }

        public override void Modify(ref int currentMod)
        {
            currentMod = ModifiersCalculation.GetResult(currentMod, GetValue, GetCalculationType, GetModifierType);
        }
    }
}