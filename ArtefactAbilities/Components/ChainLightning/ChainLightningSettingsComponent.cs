using System;
using HECSFramework.Core;

namespace Components
{
    [Serializable][Documentation(Doc.Abilities, Doc.ShooterFeature, Doc.Artefact, "ChainLightningSettingsComponent")]
    public sealed class ChainLightningSettingsComponent : BaseComponent
    {
        public int Bounces = 3;
        public float Radius;
        public float ShowTime;
    }
}