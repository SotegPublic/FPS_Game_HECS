using System;
using System.Collections.Generic;
using HECSFramework.Core;

namespace Components
{
    [Serializable][Documentation(Doc.Shelter, Doc.Holder, "here we hold Survivors before they arrived to rooms")]
    public sealed class ArrivedSurvivorsHolderComponent : BaseComponent
    {
        public Dictionary<Entity, HECSList<ArrivedSurvivor>> Survivors = new Dictionary<Entity, HECSList<ArrivedSurvivor>>(16);

        public override void Init()
        {
        }

        public int GetTotalSurvivorsCount()
        {
            var count = 0;
            foreach(var room in Survivors)
            {
                for(int i = 0; i < room.Value.Count; i++)
                {
                    if (room.Value[i].IsNewSurvivor)
                        count++;
                }
            }

            return count;
        }

        public void AddSurvivor(Entity room, ArrivedSurvivor survivor)
        {
            if (!Survivors.ContainsKey(room))
                Survivors.Add(room, new HECSList<ArrivedSurvivor>(8));

            Survivors[room].Add(survivor);
        }

        public int GetSurvivorsCount(Entity room)
        {
            return Survivors.ContainsKey(room) ? Survivors[room].Count : 0;
        }

        public bool IsHaveSurvivors()
        {
            if (Survivors.Count == 0)
                return false;

            foreach(var room in Survivors)
            {
                if (room.Value.Count > 0)
                    return true;
            }

            return false;
        }
    }


    public class ArrivedSurvivor
    {
        public Entity Entity;
        public int CurrentWayPointIndex;
        public bool IsInBunker;
        public bool IsWalking;
        public bool IsNewSurvivor;
    }
}