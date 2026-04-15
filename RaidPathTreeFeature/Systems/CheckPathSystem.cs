using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;
using Helpers;
using System.Linq;
using System.Collections.Generic;

namespace Systems
{
	[Serializable][Documentation(Doc.Raid, Doc.Path, "this system checks the availability of the path")]
    public sealed class CheckPathSystem : BaseSystem, IRequestProvider<CheckPathResult, CheckPathCommand>
    {
        [Required] private RaidPathTreeHolderComponent treeHolder;
        [Required] private GlobalLairsCooldownsHolderComponent cooldownsHolder;

        public override void InitSystem()
        {
        }

        public CheckPathResult Request(CheckPathCommand command)
        {
            var bonuses = new HECSList<BonusByStep>(32);
            var foundAvailablePath = CheckPath(command.StartZoneID, command.NextZoneID, command.PlayerRemainingSteps - 1, command.PlayerRemainingSteps, bonuses);

            bonuses.Sort((a, b) => a.Step.CompareTo(b.Step));
            var sortedBonuses = bonuses.Select(x => x.BonusID).ToList();

            return new CheckPathResult { IsPathAvailable = foundAvailablePath, PathBonuses = sortedBonuses };
        }

        public bool CheckPath(int startNodeID, int targetNodeID, int step, int remainigSteps, HECSList<BonusByStep> bonuses)
        {
            GetZoneBonuses(targetNodeID, bonuses, remainigSteps - step);

            var node = treeHolder.PathTree[targetNodeID];

            if (node.IsLair)
            {
                if(cooldownsHolder.ContainsZoneID(targetNodeID))
                    return false;
            }

            var availablePaths = node.AvailablePathIDs;

            if (availablePaths.Length <= 1)
                return true;

            var foundAvailablePath = false;

            for (int i = 0; i < availablePaths.Length; i++)
            {
                var nextNodeID = availablePaths[i];

                if (nextNodeID == startNodeID)
                    continue;

                var nextStepResult = CheckPath(targetNodeID, nextNodeID, step - 1, remainigSteps, bonuses);

                if (nextStepResult || step <= 0)
                {
                    foundAvailablePath = true;
                }
            }

            return foundAvailablePath;
        }

        private void GetZoneBonuses(int targetNodeID, HECSList<BonusByStep> bonuses, int step)
        {
            var node = treeHolder.PathTree[targetNodeID];

            foreach (var bonus in node.Bonuses)
            {
                bonuses.Add(new BonusByStep {  BonusID = bonus, Step = step });
            }
        }
    }

    public struct CheckPathResult
    {
        public bool IsPathAvailable;
        public List<int> PathBonuses;
    }

    public class BonusByStep
    {
        public int Step;
        public int BonusID;
    }
}

namespace Commands
{
    [Serializable][Documentation(Doc.Raid, Doc.Path, "CheckPathCommand")]
    public struct CheckPathCommand : ICommand
    {
        public int StartZoneID;
        public int NextZoneID;
        public int PlayerRemainingSteps;
    }
}