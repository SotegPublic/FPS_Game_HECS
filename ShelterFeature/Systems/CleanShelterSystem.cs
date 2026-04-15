using Commands;
using Components;
using Cysharp.Threading.Tasks;
using HECSFramework.Core;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Systems
{
    [Serializable]
    [Documentation(Doc.Shelter, "here we exit from shelter")]
    public sealed class CleanShelterSystem : BaseGameStateSystem
    {
        [Required] private ShelterFeatureStateComponent stateComponent;

        protected override int State => GameStateIdentifierMap.CleanShelterState;

        private EntitiesFilter roomsFilter;

        public override void InitSystem()
        {
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

namespace Commands
{
    [Serializable]
    [Documentation(Doc.Shelter, "command for clear shelter")]
    public struct CleanShelterGlobalCommand : IGlobalCommand { }
}