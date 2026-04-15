using HECSFramework.Core;
using HECSFramework.Unity;
using Helpers;
using System;
using UnityEngine;
using System.Linq;

namespace Components
{
    [Serializable][Documentation(Doc.Shelter, Doc.Counters, "this system send commands when shelter counters updates")]
    public sealed class ShelterUICountersFilter : BaseComponent
    {
        [SerializeField][IdentifierDropDown(nameof(CounterIdentifierContainer))] private int[] counters;

        public bool ContainCounter(int counter) => counters.Contains(counter);
    }
}