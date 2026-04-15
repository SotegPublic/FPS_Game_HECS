using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Holder, Doc.Shelter, "SurvivorsContainersHolderComponent")]
    public sealed class SurvivorsContainersHolderComponent : BaseContainerHolderComponent<SurvivorTagComponent>
    {
    }
}