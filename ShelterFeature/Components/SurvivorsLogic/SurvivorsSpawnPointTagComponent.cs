using HECSFramework.Core;
using HECSFramework.Unity;
using System;

namespace Components
{
    [Serializable][Documentation(Doc.Shelter, Doc.Tag, "SurvivorsSpawnPointComponent")]
    public sealed class SurvivorsSpawnPointTagComponent : BaseComponent, IWorldSingleComponent
    {
    }
}