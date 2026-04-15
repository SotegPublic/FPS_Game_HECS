using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.GameState, Doc.Shelter, "here we load shelter scene")]
    public sealed class LoadShelterSceneSystem : BaseGameStateSystem 
    {
        [Required] private ShelterFeatureStateComponent stateComponent;
        [Required] private ShelterSceneHolderComponent sceneHolder;

        protected override int State => GameStateIdentifierMap.LoadShelterSceneState;

        public override void InitSystem()
        {
        }

        protected override void ProcessState(int from, int to)
        {
            LoadSceneAsync().Forget();
        }

        private async UniTask LoadSceneAsync()
        {
            stateComponent.ShelterScene = await Addressables.LoadSceneAsync(sceneHolder.SceneReference, LoadSceneMode.Additive).Task;
            SceneManager.SetActiveScene(stateComponent.ShelterScene.Scene);

            EndState();
        }

        protected override void OnExitState()
        {
            base.OnExitState();
            Owner.World.Command(new FocusCameraByIdCommand { CameraId = CameraIdentifierMap.ShelterCamera });
        }
    }
}