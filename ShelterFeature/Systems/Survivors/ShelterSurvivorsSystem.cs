using Commands;
using Components;
using Cysharp.Threading.Tasks;
using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Systems
{
    [Serializable]
    [Documentation(Doc.Shelter, "this create and remove survivals and their modifiers")]
    public sealed class ShelterSurvivorsSystem : BaseSystem, ILateUpdatable, IReactGlobalCommand<ShelterDataWasLoadedCommand>, IReactGlobalCommand<CleanShelterGlobalCommand>,
        IReactGlobalCommand<SendSurvivorsOnMissionCommand>, IReactGlobalCommand<SurvivorArrivedInRoomCommand>, IReactGlobalCommand<ReturnSurvivorsFromMissionCommand>,
        IReactGlobalCommand<CreateSurvivorsCommand>
    {
        [Required] public ShelterSurvivorsSystemStateComponent StateComponent;
        [Required] public SurvivorsContainersHolderComponent SurvivorsContainers;
        [Required] public ArrivedSurvivorsHolderComponent ArrivedSurvivorsHolderComponent;
        [Required] public ShelterSurvivorModifiersHolderComponent SurvivorModifiersHolder;
        [Required] public CooldownComponent CooldownComponent;

        private Entity spawnPoint;
        private EntitiesFilter roomsFilter;
        private ReservedSurvivorsCounterComponent reservedSurvivorsCounter;

        public override void InitSystem()
        {
            roomsFilter = Owner.World.GetFilter(Filter.Get<RoomTagComponent>());
            reservedSurvivorsCounter = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<ReservedSurvivorsCounterComponent>();
        }

        public void UpdateLateLocal()
        {
            CreateSurvivor();
        }

        public void CommandGlobalReact(ShelterDataWasLoadedCommand command)
        {
            foreach (var room in roomsFilter)
            {
                StateComponent.RoomsForSurvivors.Add(room);
            }

            var container = SurvivorsContainers.GetFirstOrDefault();
            var survivorsCount = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<SurvivorsStorageCounterComponent>().Value;
            var reservedCount = reservedSurvivorsCounter.Value;

            var spawnSurvivorsNeeded = survivorsCount - reservedCount;

            for (int i = 0; i < spawnSurvivorsNeeded; i++)
            {
                var room = StateComponent.RoomsForSurvivors[Random.Range(0, StateComponent.RoomsForSurvivors.Count)];
                SpawnSurvivorInRoomAsync(room, container).Forget();
                room.GetComponent<SurvivorsCounterComponent>().ChangeValue(1);

                if (room.GetComponent<SurvivorsCounterComponent>().Value == room.GetComponent<SurvivorsCounterComponent>().CalculatedMaxValue)
                    StateComponent.RoomsForSurvivors.RemoveSwap(room);
            }

            for (int i = 0; i < survivorsCount; i++)
            {
                AddSurvivorModifiers();
            }

            StateComponent.RoomsForSurvivors.ClearFast();
        }
        public void CommandGlobalReact(CleanShelterGlobalCommand command)
        {
            foreach (var room in ArrivedSurvivorsHolderComponent.Survivors)
            {
                var targetPosition = room.Key.GetComponent<RoomMonocomponentProviderComponent>().Get.FloorEdge.position;


                for (int i = 0; i < room.Value.Count; i++)
                {
                    var arrivedSurvivor = room.Value[i];

                    arrivedSurvivor.Entity.Command(new TriggerAnimationCommand { Index = AnimParametersMap.Idle });
                    var survivorTransform = arrivedSurvivor.Entity.GetTransform();

                    survivorTransform.position = targetPosition;
                    survivorTransform.parent = room.Key.GetTransform();

                    room.Value[i].Entity.Command(new ForceStartAICommand());
                    StateComponent.ActiveSurvivors.Add(arrivedSurvivor.Entity);
                    UpdateCounters(room.Key, arrivedSurvivor.IsNewSurvivor);
                }

                room.Value.ClearFast();
            }

            spawnPoint = null;
        }
        public void CommandGlobalReact(SendSurvivorsOnMissionCommand command)
        {
            for (int i = 0; i < command.SurvivorsCount; i++)
            {
                var survivor = StateComponent.ActiveSurvivors[Random.Range(0, StateComponent.ActiveSurvivors.Count)];
                var room = survivor.GetComponent<SurvivorRoomHolderComponent>().Room;

                room.GetComponent<SurvivorsCounterComponent>().ChangeValue(-1);

                survivor.Command(new ForceStopAICommand());
                survivor.Command(new TriggerAnimationCommand { Index = AnimParametersMap.Idle });

                Owner.World.Command(new DestroyEntityWorldCommand { Entity = survivor });
                StateComponent.ActiveSurvivors.RemoveSwap(survivor);
                reservedSurvivorsCounter.ChangeValue(1);
            }
        }
        public void CommandGlobalReact(SurvivorArrivedInRoomCommand command)
        {
            var arrivedSurvivors = ArrivedSurvivorsHolderComponent.Survivors[command.Room];
            arrivedSurvivors.RemoveSwap(command.Survivor);
            command.Survivor.Entity.Command(new ForceStartAICommand());
            StateComponent.ActiveSurvivors.Add(command.Survivor.Entity);
            UpdateCounters(command.Room, command.Survivor.IsNewSurvivor);
        }

        public void CommandGlobalReact(ReturnSurvivorsFromMissionCommand command)
        {
            for (int i = 0; i < command.ReturnCount; i++)
            {
                StateComponent.ArrivedSurvivorQueue.Enqueue(new ArrivedSurvivor
                {
                    IsNewSurvivor = false,
                });
            }

            var dif = command.SendCount - command.ReturnCount;
            for (int i = 0; i < dif; i++)
            {
                RemoveSurvivorModifiers();
            }

            var playerCountersHolder = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<CountersHolderComponent>();

            if (playerCountersHolder.TryGetIntCounter(CounterIdentifierContainerMap.Survivors, out var survivorsCounter))
                survivorsCounter.ChangeValue(-dif);

            if (playerCountersHolder.TryGetIntCounter(CounterIdentifierContainerMap.ReservedSurvivors, out var reservedCounter))
                reservedCounter.ChangeValue(-dif);
        }

        public void CommandGlobalReact(CreateSurvivorsCommand command)
        {
            for (int i = 0; i < command.Count; i++)
            {
                if (!IsWeHavePlace())
                    continue;

                StateComponent.ArrivedSurvivorQueue.Enqueue(new ArrivedSurvivor
                {
                    IsNewSurvivor = true,
                });
            }
        }

        #region Create
        private void CreateSurvivor()
        {
            if (Owner.ContainsMask<OnCooldownTagComponent>())
                return;
            if (StateComponent.ArrivedSurvivorQueue.Count == 0)
                return;
            if (!IsWeHavePlace())
                return;

            var arrivedSurvivor = StateComponent.ArrivedSurvivorQueue.Dequeue();

            foreach (var room in roomsFilter)
            {
                var survivorsCounter = room.GetComponent<SurvivorsCounterComponent>();
                var maxInRoomSurvivorsCount = survivorsCounter.CalculatedMaxValue;
                var currentInRoomSurvivorsCount = survivorsCounter.Value;
                var arriveddSurvivorsCount = ArrivedSurvivorsHolderComponent.GetSurvivorsCount(room);

                var isWeHavePlaceInRoom = currentInRoomSurvivorsCount + arriveddSurvivorsCount < maxInRoomSurvivorsCount;

                if (!isWeHavePlaceInRoom)
                    continue;

                StateComponent.RoomsForSurvivors.Add(room);
            }

            if (StateComponent.RoomsForSurvivors.Count == 0)
                return;

            var roomForSurvivor = StateComponent.RoomsForSurvivors[Random.Range(0, StateComponent.RoomsForSurvivors.Count)];
            SpawnSurvivorAsync(roomForSurvivor, arrivedSurvivor).Forget();

            StateComponent.RoomsForSurvivors.ClearFast();
        }

        private async UniTask SpawnSurvivorAsync(Entity roomForSurvivor, ArrivedSurvivor arrivedSurvivor)
        {
            var container = SurvivorsContainers.GetFirstOrDefault();

            if (Owner.ContainsMask<ShelterSceneActiveTagComponent>())
            {
                await SpawnSurvivorOutsideAsync(roomForSurvivor, container, arrivedSurvivor);
                var tag = Owner.GetOrAddComponent<OnCooldownTagComponent>();
                tag.Cooldown = CooldownComponent.Value;
            }
            else
            {
                await SpawnSurvivorInRoomAsync(roomForSurvivor, container);
                UpdateCounters(roomForSurvivor, arrivedSurvivor.IsNewSurvivor);
            }
        }

        private async UniTask SpawnSurvivorOutsideAsync(Entity roomForSurvivor, EntityContainer container, ArrivedSurvivor arrivedSurvivor)
        {
            if (spawnPoint == null)
            {
                await new WaitSapwnPoint(Owner.World).RunJob();
                spawnPoint = Owner.World.GetEntityBySingleComponent<SurvivorsSpawnPointTagComponent>();
            }

            var survivor = await CreateSurvivor(container, spawnPoint.GetPosition(), roomForSurvivor);
            arrivedSurvivor.Entity = survivor.Entity;

            ArrivedSurvivorsHolderComponent.AddSurvivor(roomForSurvivor, arrivedSurvivor);
        }

        private async UniTask SpawnSurvivorInRoomAsync(Entity roomForSurvivor, EntityContainer container)
        {
            var spawnPoint = roomForSurvivor.GetComponent<RoomMonocomponentProviderComponent>().Get.FloorEdge;
            var survivor = await CreateSurvivor(container, spawnPoint.position, roomForSurvivor, parrentTransform: roomForSurvivor.GetTransform());

            await new WaitFor<ViewReadyTagComponent>(survivor.Entity).RunJob();

            survivor.Command(new ForceStartAICommand());
            StateComponent.ActiveSurvivors.Add(survivor.Entity);
        }

        private static async UniTask<Actor> CreateSurvivor(EntityContainer container, Vector3 spawnPoint, Entity roomForSurvivor, Transform parrentTransform = null)
        {
            var survivorRotation = Quaternion.Euler(0, -90, 0);
            var survivor = await container.GetActor(position: spawnPoint, rotation: survivorRotation, transform: parrentTransform);

            survivor.Init();
            survivor.Entity.GetOrAddComponent<SurvivorRoomHolderComponent>().Room = roomForSurvivor;

            return survivor;
        }
        #endregion

        #region Counters
        private void UpdateCounters(Entity room, bool isNewSurvivor)
        {
            if (isNewSurvivor)
                UpdateCountersForNewSurvivor(room);
            else
            {
                room.GetComponent<SurvivorsCounterComponent>().ChangeValue(1);
                var playerCountersHolder = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<CountersHolderComponent>();
                if (playerCountersHolder.TryGetIntCounter(CounterIdentifierContainerMap.ReservedSurvivors, out var reservedCounter))
                    reservedCounter.ChangeValue(-1);
            }
        }

        private void UpdateCountersForNewSurvivor(Entity room)
        {
            room.GetComponent<SurvivorsCounterComponent>().ChangeValue(1);
            AddSurvivorModifiers();

            var playerCountersHolder = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<CountersHolderComponent>();
            if (playerCountersHolder.TryGetIntCounter(CounterIdentifierContainerMap.Survivors, out var counter))
                counter.ChangeValue(1);
        }

        private void AddSurvivorModifiers()
        {
            for (int i = 0; i < SurvivorModifiersHolder.Modifiers.Length; i++)
            {
                var protoModifier = SurvivorModifiersHolder.Modifiers[i];
                var modifier = new DefaultIntModifier
                {
                    GetValue = protoModifier.GetValue,
                    GetCalculationType = protoModifier.GetCalculationType,
                    GetModifierType = protoModifier.GetModifierType,
                    ModifierCounterID = protoModifier.ModifierCounterID,
                    ModifierGuid = Guid.NewGuid(),
                    ModifierType = protoModifier.ModifierType
                };

                Owner.Command(new AddCounterModifierCommand<int> { Id = modifier.ModifierCounterID, IsUnique = false, Modifier = modifier, Owner = Owner.GUID });
                SurvivorModifiersHolder.AddActiveModifier(modifier);
            }
        }

        private void RemoveSurvivorModifiers()
        {
            for (int i = 0; i < SurvivorModifiersHolder.Modifiers.Length; i++)
            {
                var protoModifier = SurvivorModifiersHolder.Modifiers[i];
                var modifier = SurvivorModifiersHolder.GetModifierForRemove(protoModifier.ModifierCounterID);
                Owner.Command(new RemoveCounterModifierCommand<int> { Id = modifier.ModifierCounterID, Modifier = modifier, Owner = Owner.GUID });
            }
        }
        #endregion

        private bool IsWeHavePlace()
        {
            var survivorsCounter = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<SurvivorsCounterComponent>();
            var survivorsMax = survivorsCounter.CalculatedMaxValue;
            var currentSurvivorsCount = survivorsCounter.Value;
            var arrivedCount = ArrivedSurvivorsHolderComponent.GetTotalSurvivorsCount();

            if (currentSurvivorsCount + arrivedCount >= survivorsMax)
                return false;

            return true;
        }
    }


    [Documentation(Doc.Shelter, Doc.Job, "this job wait until survivors spawn point will init")]
    public readonly struct WaitSapwnPoint : IHecsJob
    {
        public readonly World World;

        public WaitSapwnPoint(World world)
        {
            World = world;
        }

        public void Run() { }

        public bool IsComplete()
        {
            return World.TryGetEntityBySingleComponent<SurvivorsSpawnPointTagComponent>(out var spawnPoint);
        }
    }
}

namespace Commands
{
    [Serializable][Documentation(Doc.Shelter, "this create survivals and jobs for rooms services")]
    public struct SurvivorArrivedInRoomCommand : IGlobalCommand
    {
        public Entity Room;
        public ArrivedSurvivor Survivor;
    }

    [Serializable]
    [Documentation(Doc.Shelter, "we send this command when we need send survivors on mission")]
    public struct SendSurvivorsOnMissionCommand : IGlobalCommand
    {
        public int SurvivorsCount;
    }

    [Serializable]
    [Documentation(Doc.Shelter, "we send this command when we need return survivors from mission")]
    public struct ReturnSurvivorsFromMissionCommand : IGlobalCommand
    {
        public int SendCount;
        public int ReturnCount;
    }

    [Serializable]
    [Documentation(Doc.Shelter, "we send this command when we need create new survivors")]
    public struct CreateSurvivorsCommand : IGlobalCommand
    {
        public int Count;
    }
}