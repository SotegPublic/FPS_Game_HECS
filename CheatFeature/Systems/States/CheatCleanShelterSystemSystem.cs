using Commands;
using Components;
using Cysharp.Threading.Tasks;
using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Systems
{
	[Serializable][Documentation(Doc.Cheats, Doc.Shelter, "this system clean shelter when we use cheat start")]
    public sealed class CheatCleanShelterSystemSystem : BaseGameStateSystem
    {
        [Required] public CheatStartParametersComponent StartParametersComponent;

        protected override int State => GameStateIdentifierMap.CheatUnloadShelterSceneState;

        private EntitiesFilter roomsFilter;
        private ShelterFeatureStateComponent stateComponent;
        public override void InitSystem()
        {
            stateComponent = Owner.World.GetEntityBySingleComponent<ShelterFeatureTagComponent>().GetComponent<ShelterFeatureStateComponent>();
            roomsFilter = Owner.World.GetFilter(Filter.Get<RoomTagComponent>());
        }

        protected override void ProcessState(int from, int to)
        {
            CleanAsync().Forget();
        }

        public async UniTask CleanAsync()
        {
            Owner.World.Command(new CleanShelterGlobalCommand());

            await UniTask.DelayFrame(1);

            MoveRooms();
            await UnloadScene();

            if(StartParametersComponent.IsOpenShelter)
                Owner.World.Command(new ForceGameStateTransitionGlobalCommand { GameState = GameStateIdentifierMap.LoadShelterSceneState });
            else
                EndState();
        }

        private async UniTask UnloadScene()
        {
            await Addressables.UnloadSceneAsync(stateComponent.ShelterScene).Task;
        }

        private void MoveRooms()
        {
            foreach (var room in roomsFilter)
            {
                var roomTransform = room.GetTransform();
                roomTransform.localPosition = Vector3.zero;
            }
        }
    }
}