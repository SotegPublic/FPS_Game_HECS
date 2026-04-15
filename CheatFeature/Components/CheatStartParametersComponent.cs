using HECSFramework.Core;
using HECSFramework.Unity;
using Helpers;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Cheats, "here we hold parameters from cheat start window")]
    public sealed class CheatStartParametersComponent : BaseComponent
    {
        public EntityContainer[] Weapons;
        public EntityContainer[] Artefacts;
        public int MissionID;
        public int StartZoneIndex;
        public int TargetZoneIndex;

        public bool IsScenarioOn;
        public bool IsOpenShelter;

        public Entity StartShootingPoint;

        public override void BeforeDispose()
        {
            Weapons = null;
            Artefacts = null;
            MissionID = 0;
            StartZoneIndex = 0;
            TargetZoneIndex = 0;

            IsScenarioOn = false;

            StartShootingPoint = null;
        }
    }
}