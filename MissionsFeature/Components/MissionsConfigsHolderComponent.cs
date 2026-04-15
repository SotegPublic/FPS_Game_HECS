using HECSFramework.Core;
using HECSFramework.Unity.Helpers;
using Helpers;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Missions, Doc.MissionsGeneration, "MissionsByEnterConfig")]
    public sealed class MissionsConfigsHolderComponent : BaseComponent
    {
        [SerializeField] private MissionConfig[] missions;
        [SerializeField][IdentifierDropDown(nameof(MissionIdentifier))] private int[] scenarioMissionsOrder;

        private Dictionary<int, List<MissionConfig>> configsByEnter = new Dictionary<int, List<MissionConfig>>(8);

        public int[] ScenarioMissionsOrder => scenarioMissionsOrder;
        public MissionConfig[] Missions => missions;

        public override void Init()
        {
            base.Init();

            foreach(var config in missions)
            {
                if (config.IsScenarioMission)
                    continue;

                if(!configsByEnter.ContainsKey(config.StartZone))
                    configsByEnter.Add(config.StartZone, new List<MissionConfig>(16));

                configsByEnter[config.StartZone].Add(config);
            }

        }

        public int GetScenarioMissionID(int currentScenarioIndex) => scenarioMissionsOrder[currentScenarioIndex];

        public MissionConfig GetMissionByID(int missionID)
        {
            for (int i = 0; i < missions.Length; i++)
            {
                if (missions[i].MissionId == missionID)
                    return missions[i];
            }

            return null;
        }

        public bool TryGetMissionsByEnter(int enterID, out List<MissionConfig> missions)
        {
            if (configsByEnter.ContainsKey(enterID))
            {
                missions = configsByEnter[enterID];
                return true;
            }

            missions = null;
            return false;
        }

        [Button("Get mission configs")]
        public void GetMissions()
        {
            var scenarioMissions = new SOProvider<MissionConfig>().GetCollection().ToArray();

            missions = scenarioMissions;
        }
    }
}