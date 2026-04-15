using HECSFramework.Core;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Shelter, "ShelterControlSystemStateComponent")]
    public sealed class ShelterControlSystemStateComponent: BaseComponent
    {
        public bool IsPressed;

        public Vector2 From;
        public Vector2 CurrentCoord;

        public float DragLenght()
        {
            return (From - CurrentCoord).magnitude;
        }
    }
}
