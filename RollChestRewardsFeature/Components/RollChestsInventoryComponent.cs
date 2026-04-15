using HECSFramework.Core;
using HECSFramework.Unity;
using Helpers;
using System;
using System.Collections.Generic;

namespace Components
{
    [Serializable][Documentation(Doc.Inventory, Doc.Loot, "here we hold roll chests")]
    public sealed class RollChestsInventoryComponent : BaseComponent
    {
        private Dictionary<int, Queue<HECSList<int>>> chests = new Dictionary<int, Queue<HECSList<int>>>(16);

        public void AddChest(Entity entity)
        {
            var chestID = entity.GetContainerIndex();

            if (!chests.ContainsKey(chestID))
                chests.Add(chestID, new Queue<HECSList<int>>(16));

            var rewardsHolder = entity.GetComponent<LootRewardsHolderComponent>();
            chests[chestID].Enqueue(rewardsHolder.Rewards);
        }

        public HECSPooledArray<RollChestInfo> GetRollChestsCounts()
        {
            var array = HECSPooledArray<RollChestInfo>.GetArray(chests.Count);

            foreach(var chest in chests)
            {
                array.Add(new RollChestInfo
                {
                    ChestID = chest.Key,
                    Count = chest.Value.Count
                });
            }

            return array;
        }

        public bool TryGetChestRewards(int chestTypeID, out HECSList<int> rewards)
        {
            if (chests.ContainsKey(chestTypeID))
            {
                rewards = chests[chestTypeID].Dequeue();

                if (chests[chestTypeID].Count == 0)
                    chests.Remove(chestTypeID);

                return true;
            }

            rewards = null;
            return false;
        }

        public void ClearInventory()
        {
            foreach(var chestRewards in chests.Values)
            {
                for(int i = 0; i < chestRewards.Count; i++)
                {
                    chestRewards.Dequeue().Clear();
                }
            }

            chests.Clear();
        }
    }

    public struct RollChestInfo
    {
        public int ChestID;
        public int Count;
    }
}