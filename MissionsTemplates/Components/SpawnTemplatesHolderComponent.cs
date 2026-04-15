using HECSFramework.Core;
using HECSFramework.Unity.Helpers;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Missions, Doc.MissionsGeneration, "here we hold spawn templates")]
    public sealed class SpawnTemplatesHolderComponent : BaseComponent
    {
        [SerializeField] private SpawnMissionTemplate[] spawnTemplates;

        private Dictionary<int, List<SpawnMissionTemplate>> templatesByGrades = new Dictionary<int, List<SpawnMissionTemplate>>();

        public override void Init()
        {
            base.Init();

            foreach (var pathTemplate in spawnTemplates)
            {
                if (!templatesByGrades.ContainsKey(pathTemplate.GradeID))
                    templatesByGrades.Add(pathTemplate.GradeID, new List<SpawnMissionTemplate>());

                templatesByGrades[pathTemplate.GradeID].Add(pathTemplate);
            }
        }

        public bool TryGetRandomSpawnsTemplate(int gradeID, int stepsCount, out SpawnMissionTemplate template)
        {
            if (templatesByGrades.ContainsKey(gradeID))
            {
                var templates = templatesByGrades[gradeID].Where(t => t.Spawns.Length >= stepsCount).ToArray();
                
                if(templates.Length == 0)
                {
                    template = null;
                    return false;
                }

                template = templates.Length == 1 ? templates[0] : templates[UnityEngine.Random.Range(0, templates.Length)];
                return true;
            }

            template = null;
            return false;
        }

        [Button("Get Spawn Templates")]
        private void GetSpawnTemplates()
        {
            var templates = new SOProvider<SpawnMissionTemplate>().GetCollection().ToArray();
            spawnTemplates = templates;
        }
    }
}