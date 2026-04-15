using HECSFramework.Core;
using HECSFramework.Unity;
using HECSFramework.Unity.Helpers;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Missions, Doc.MissionsGeneration, "here we hold path templates")]
    public sealed class PathTemplatesHolderComponent : BaseComponent
    {
        [SerializeField] private PathMissionTemplate[] pathTemplates;

        private Dictionary<int, List<PathMissionTemplate>> templatesByGrades = new Dictionary<int, List<PathMissionTemplate>>();

        public override void Init()
        {
            base.Init();

            foreach (var pathTemplate in pathTemplates)
            {
                if(!templatesByGrades.ContainsKey(pathTemplate.EnterID))
                    templatesByGrades.Add(pathTemplate.EnterID, new List<PathMissionTemplate>());

                templatesByGrades[pathTemplate.EnterID].Add(pathTemplate);
            }
        }

        public bool TryGetRandomPathTemplate(int enterID, int missionTypeID, out PathMissionTemplate template)
        {
            if (templatesByGrades.ContainsKey(enterID))
            {
                var templates = templatesByGrades[enterID].Where(t => t.MissionTypeID == missionTypeID).ToArray();
                
                if(templates.Length > 0)
                {
                    var randomPathTemplate = templates.Length == 1 ? templates[0] : templates[UnityEngine.Random.Range(0, templates.Length)];

                    template = randomPathTemplate;
                    return true;
                }
            }

            template = null;
            return false;
        }

        [Button("Get Path Templates")]
        private void GetPathTemplates()
        {
            var templates = new SOProvider<PathMissionTemplate>().GetCollection().ToArray();
            pathTemplates = templates;
        }
    }
}