using HECSFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Components
{
    [Obsolete]
    [Serializable][Documentation(Doc.Shelter, Doc.Holder, "here we hold all shelter rooms jobs")]
    public sealed class ShelterRoomsJobsHolderComponent : BaseComponent
    {
        private Dictionary<Guid, Queue<RoomServiceJob>> roomsJobs = new Dictionary<Guid, Queue<RoomServiceJob>>(16);

        public void AddJob(Guid roomID, RoomServiceJob job)
        {
            if(!roomsJobs.ContainsKey(roomID))
                roomsJobs.Add(roomID, new Queue<RoomServiceJob>(8));

            roomsJobs[roomID].Enqueue(job);
        }

        public Queue<RoomServiceJob> GetJobsQueue(Guid roomID)
        {
            if (!roomsJobs.ContainsKey(roomID))
                roomsJobs.Add(roomID, new Queue<RoomServiceJob>(8));

            return roomsJobs[roomID];
        }

        public int GetJobsQueueCount(Guid roomID)
        {
            return roomsJobs.ContainsKey(roomID) ? roomsJobs[roomID].Count : 0;
        }
    }

    [Obsolete]
    public class RoomServiceJob
    {
        public Entity Survivor;
    }
}