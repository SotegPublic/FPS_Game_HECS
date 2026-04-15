using BluePrints.Identifiers;
using HECSFramework.Core;
using Helpers;
using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Missions, Doc.MissionsGeneration, "MissionsDifficultyConfig")]
    public sealed class MissionsDifficultyConfigComponent : BaseComponent
    {
        [SerializeField] private MissionTypeDifficultyConfig[] typeDifficultyConfigs;
        [SerializeField] private GradeDifficultyConfig[] gradeDifficultyConfigs;
        [SerializeField] private StartZoneDifficultyConfig[] startZoneDifficultyConfigs;

        public int GetDifficultyValue(int gradeID, int typeID, int startZoneID)
        {
            var gradeValue = GetDifficultyValueByGrade(gradeID);
            var typeValue = GetDifficultyValueByType(typeID);
            var startZoneValue = GetDifficultyValueByZone(startZoneID);

            return typeValue + gradeValue + startZoneValue;
        }
        
        
        private int GetDifficultyValueByGrade(int gradeID)
        {
            for (int i = 0; i < gradeDifficultyConfigs.Length; i++)
            {
                if (gradeDifficultyConfigs[i].GradeID != gradeID)
                    continue;

                return gradeDifficultyConfigs[i].DifficultyValue;
            }

            return 0;
        }

        private int GetDifficultyValueByType(int typeID)
        {
            for(int i = 0; i < typeDifficultyConfigs.Length; i++)
            {
                if (typeID == typeDifficultyConfigs[i].MissionType)
                    return typeDifficultyConfigs[i].LevelModifier;
            }

            return 0;
        }

        private int GetDifficultyValueByZone(int zoneID)
        {
            for (int i = 0; i < startZoneDifficultyConfigs.Length; i++)
            {
                if (zoneID == startZoneDifficultyConfigs[i].StartZoneID)
                    return startZoneDifficultyConfigs[i].DifficultyValue;
            }

            return 0;
        }
    }

    [Serializable]
    public sealed class MissionTypeDifficultyConfig
    {
        [SerializeField][IdentifierDropDown(nameof(MissionTypeIdentifier))] private int missionType;
        [SerializeField] private int levelModifier;

        public int MissionType => missionType;
        public int LevelModifier => levelModifier;
    }

    [Serializable]
    public class GradeDifficultyConfig
    {
        [SerializeField][IdentifierDropDown(nameof(GradeIdentifier))] private int gradeID;
        [SerializeField] private int difficultyValue;

        public int GradeID => gradeID;
        public int DifficultyValue => difficultyValue;
    }

    [Serializable]
    public class StartZoneDifficultyConfig
    {
        [SerializeField][IdentifierDropDown(nameof(EnterZoneIdentifier))] private int startZoneID;
        [SerializeField] private int difficultyValue;

        public int StartZoneID => startZoneID;
        public int DifficultyValue => difficultyValue;
    }
}