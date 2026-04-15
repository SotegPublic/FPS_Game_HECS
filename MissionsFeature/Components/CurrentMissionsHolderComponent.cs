using HECSFramework.Core;
using System;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Missions, "here we hold current missions for missions list in shelter")]
    public sealed class CurrentMissionsHolderComponent : BaseComponent, IWorldSingleComponent
    {
        [Tooltip("Max count for missions list")]
        public int MissionsListLength;
        [Tooltip("Frequency for update mission list in seconds")]
        public long UpdateListFrequencyInSec;
        public long LastMissionsUpdateTime;
        public HECSList<ActiveMission> ActiveMissions;

        public override void Init()
        {
            base.Init();
            ActiveMissions = new HECSList<ActiveMission>(MissionsListLength);

            for(int i = 0; i < MissionsListLength; i++)
            {
                ActiveMissions.Add(new ActiveMission());
            }
        }

        public bool TryGetActiveMission(int missionID, out ActiveMission activeMission)
        {
            for (int i = 0; i < ActiveMissions.Count; i++)
            {
                if (ActiveMissions[i].MissionID == missionID)
                {
                    activeMission = ActiveMissions[i];
                    return true;
                }
            }

            activeMission = null;
            return false;
        }

        public int GetActiveMissionsCount()
        {
            var count = 0;

            for(int i = 0; i < ActiveMissions.Count; i++)
            {
                if (ActiveMissions[i].MissionID != 0)
                    count++;
            }

            return count;
        }

        public void ResetComplitedAndNotProgressedMissions()
        {
            for(int i = 0;i < ActiveMissions.Count; i++)
            {
                if (!ActiveMissions[i].IsInProgress)
                    ActiveMissions[i].Clear();
            }

            SortInProgressMissions();
        }

        private void SortInProgressMissions()
        {
            int insertPos = ActiveMissions.Count - 1;

            for (int i = ActiveMissions.Count - 1; i >= 0; i--)
            {
                if (!ActiveMissions[i].IsInProgress)
                    continue;

                if (i != insertPos)
                {
                    var temp = ActiveMissions[insertPos];
                    ActiveMissions[insertPos] = ActiveMissions[i];
                    ActiveMissions[i] = temp;
                }
                insertPos--;
            }
        }

        public void SortAfterAddedNewMissions()
        {
            var firstEmptySlot = -1;

            for(int i = 0; i < ActiveMissions.Count; i++)
            {
                if (ActiveMissions[i].MissionID != 0)
                    continue;

                firstEmptySlot = i;
                break;
            }

            if (firstEmptySlot == -1)
                return;

            for(int i = firstEmptySlot; i < ActiveMissions.Count; i++)
            {
                if (!ActiveMissions[i].IsInProgress)
                    continue;
                
                if(i != firstEmptySlot)
                {
                    var temp = ActiveMissions[firstEmptySlot];
                    ActiveMissions[firstEmptySlot] = ActiveMissions[i];
                    ActiveMissions[i] = temp;
                }
                firstEmptySlot++;
            }
        }
    }

    [Serializable]
    public class ActiveMission
    {
        public int MissionID;
        public long EndTimeSec;
        public bool IsCompleted;
        public bool IsFinished;

        public bool IsInProgress => EndTimeSec != 0 && !IsCompleted && !IsFinished;

        public float GetMissionRemainTime()
        {
            var currentTimeSec = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var remainingSec = EndTimeSec - currentTimeSec;

            return remainingSec;
        }

        public void Clear()
        {
            MissionID = 0;
            EndTimeSec = 0;
            IsCompleted = false;
            IsFinished = false;
        }
    }
}