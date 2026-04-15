using HECSFramework.Core;
using System;
using System.Collections.Generic;

namespace Components
{
    [Serializable][Documentation(Doc.Raid, "here we hold all raid path nodes")]
    public sealed class RaidPathTreeHolderComponent : BaseComponent
    {
        private Dictionary<int, PathNode> pathTree = new Dictionary<int, PathNode>(64);

        public ReadOnlyDictionary<int, PathNode> PathTree;

        public override void Init()
        {
            PathTree = new ReadOnlyDictionary<int, PathNode>(pathTree);
        }

        public void AddNode(PathNode node)
        {
            pathTree.Add(node.ZoneID, node);
        }

        public void ClearPathTree()
        {
            if(pathTree.Count == 0) 
                return;
            
            pathTree.Clear();
        }
    }

    public struct PathNode
    {
        public int ZoneID;
        public bool IsLair;
        public HECSList<int> Bonuses;
        public int[] AvailablePathIDs;
    }
}