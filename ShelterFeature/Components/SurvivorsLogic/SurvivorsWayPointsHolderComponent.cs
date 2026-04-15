using HECSFramework.Core;
using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Shelter, Doc.Tag, "SurvivorsWayPointsHolderComponent")]
    public sealed class SurvivorsWayPointsHolderComponent : BaseComponent, IWorldSingleComponent
    {
        public HECSList<Transform> Waypoints = new HECSList<Transform>(4);

        public override void AfterInit()
        {
            foreach(Transform child in Owner.GetTransform())
            {
                Waypoints.Add(child);
            }
        }
    }
}