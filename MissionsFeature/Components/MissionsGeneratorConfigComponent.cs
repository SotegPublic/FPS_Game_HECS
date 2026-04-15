using HECSFramework.Core;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Missions, Doc.MissionsGeneration, "missions generator config")]
    public sealed class MissionsGeneratorConfigComponent : BaseComponent
    {
        [Header("Monsters")]
        [SerializeField]
        private Vector2Int monstersCount = new Vector2Int(10, 15);

        [Header("Waves")]
        [SerializeField]
        private int baseWavesCount = 3;

        [SerializeField]
        private int wavesRandom = 3;

        [SerializeField]
        private float waitMaxDelay = 1;

        public Vector2Int MonstersCount => monstersCount;
        public int BaseWavesCount => baseWavesCount;
        public int WavesRandom => wavesRandom;
        public float WaitMaxDelay => waitMaxDelay;
    }
}