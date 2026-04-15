using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Shelter, Doc.Tag, "we set this tag when shelter scene active")]
    public sealed class ShelterSceneActiveTagComponent : BaseComponent
    {
    }
}