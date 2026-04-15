using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Shelter, "RoomMonocomponentProviderComponent")]
    public sealed class RoomMonocomponentProviderComponent : BaseComponent, IInitAfterView
    {
        private RoomMonocomponent monocomponent;

        public RoomMonocomponent Get => monocomponent;

        public void InitAfterView()
        {
            var view = Owner.GetComponent<ViewReadyTagComponent>().View;
            monocomponent = view.GetComponent<RoomMonocomponent>();
        }

        public void Reset()
        {
        }
    }
}