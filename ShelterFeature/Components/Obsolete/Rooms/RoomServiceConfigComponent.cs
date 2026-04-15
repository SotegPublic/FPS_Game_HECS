using HECSFramework.Core;
using HECSFramework.Unity;
using Helpers;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Components
{
    [Obsolete]
    [Serializable][Documentation(Doc.Shelter, Doc.Config, "here we set parameters for room service")]
    public sealed class RoomServiceConfigComponent : BaseComponent
    {
        [SerializeField] private ResourceForServiceModel[] resourcesForService;
        [SerializeField] private float serviceDuration;
        [SerializeField] private int serviceProfit;
        [SerializeField] private int maxAwaitingServiceJobs;

        public ResourceForServiceModel[] ResourcesForService => resourcesForService;
        public float ServiceDuration => serviceDuration;
        public int MaxAwaitingServiceJobs => maxAwaitingServiceJobs;
    }

    [Obsolete][Serializable]
    public class ResourceForServiceModel
    {
        [IdentifierDropDown(nameof(CounterIdentifierContainer))] public int ResourceID;
        public int ServiceCost;
        public int ResourceCap;
    }
}