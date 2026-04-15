using HECSFramework.Core;
using System;
using System.Collections.Generic;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Shelter, "ShelterSurvivorsSystemStateComponent")]
    public sealed class ShelterSurvivorsSystemStateComponent : BaseComponent
    {
        public HECSList<Entity> RoomsForSurvivors = new HECSList<Entity>(16);
        public HECSList<Entity> ActiveSurvivors = new HECSList<Entity>(32);
        public Queue<ArrivedSurvivor> ArrivedSurvivorQueue = new Queue<ArrivedSurvivor>(8);
    }
}