using Commands;
using Components;
using Cysharp.Threading.Tasks;
using HECSFramework.Core;
using System;
using UnityEngine;

namespace Systems
{
    [Serializable][Documentation(Doc.Shelter, "this system process survivors move in shelter mode")]
    public sealed class MoveSurvivorsSystem : BaseSystem, IUpdatable
    {
        [Required] public ArrivedSurvivorsHolderComponent ArrivedSurvivorsHolderComponent;

        private Entity spawnPoint;

        public override void InitSystem()
        {
            GetSpawnPointAsync().Forget();
        }

        private async UniTask GetSpawnPointAsync()
        {
            await new WaitSapwnPoint(Owner.World).RunJob();
            spawnPoint = Owner.World.GetEntityBySingleComponent<SurvivorsSpawnPointTagComponent>();
        }

        public void UpdateLocal()
        {
            MoveSurvivors();
        }

        private void MoveSurvivors()
        {
            if (!Owner.ContainsMask<ShelterSceneActiveTagComponent>())
                return;
            if (spawnPoint == null || !spawnPoint.IsAliveAndNotDead())
                return;
            if (!ArrivedSurvivorsHolderComponent.IsHaveSurvivors())
                return;

            var wayPoints = spawnPoint.GetComponent<SurvivorsWayPointsHolderComponent>().Waypoints;

            foreach(var arrivedSurvivors in ArrivedSurvivorsHolderComponent.Survivors)
            {
                var room = arrivedSurvivors.Key;

                for(int i = arrivedSurvivors.Value.Count - 1; i >= 0; i--)
                {
                    var arrivedSurvivor = arrivedSurvivors.Value[i];

                    if (!arrivedSurvivor.Entity.ContainsMask<ViewReadyTagComponent>())
                        continue;

                    var currentWayPoint = arrivedSurvivor.CurrentWayPointIndex;

                    if (!arrivedSurvivor.IsInBunker)
                    {
                        if (!arrivedSurvivor.IsWalking)
                        {
                            arrivedSurvivor.Entity.Command(new TriggerAnimationCommand { Index = AnimParametersMap.Walking });
                            arrivedSurvivor.IsWalking = true;
                        }

                            var speed = arrivedSurvivor.Entity.GetComponent<MoveSpeedComponent>().WalkingSpeed * Time.deltaTime;
                        arrivedSurvivor.Entity.GetTransform().position = Vector3.MoveTowards(
                            arrivedSurvivor.Entity.GetPosition(),
                            wayPoints[currentWayPoint + 1].position,
                            speed);

                        if ((arrivedSurvivor.Entity.GetTransform().position - wayPoints[currentWayPoint + 1].position).sqrMagnitude <= 0.01f)
                        {
                            arrivedSurvivor.CurrentWayPointIndex++;
                            arrivedSurvivor.IsInBunker = arrivedSurvivor.CurrentWayPointIndex == wayPoints.Count - 1 ? true : false;

                            if (arrivedSurvivor.IsInBunker)
                                arrivedSurvivor.Entity.Command(new TriggerAnimationCommand { Index = AnimParametersMap.Idle });
                        }
                    }
                }
            }
        }
    }
}
