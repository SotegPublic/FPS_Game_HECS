using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Visual, Doc.Loot, "this config for rewards visual elements")]
    public sealed class LootVisualConfigComponent : BaseComponent
    {
        [SerializeField] private float spawnDistance = 0.5f;
        [SerializeField] private float arcAngle = 90f;

        public float SpawnDistance => spawnDistance;
        public float ArcAngle => arcAngle;
    }
}