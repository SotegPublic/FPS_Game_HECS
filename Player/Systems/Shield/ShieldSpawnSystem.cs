using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;

namespace Systems
{
	[Serializable][Documentation(Doc.Player, Doc.Spawn, Doc.Shield, "this system spawn shield")]
    public sealed class ShieldSpawnSystem : BaseSystem, IInitAfterView
    {
        [Required] private ShieldContainerHolderComponent shieldContainerHolder;
        [Required] private ShieldSpawnTargetProviderComponent spawnTargetProvider;

        private Actor shieldActor;

        public async void InitAfterView()
        {
            var container = shieldContainerHolder.ShieldContainer;
            var spawnTargetTransform = spawnTargetProvider.Get.gameObject.transform;

            shieldActor = await container.GetActor(transform: spawnTargetTransform);
            shieldActor.transform.localPosition = Vector3.zero;
            shieldActor.Init();
        }

        public override void InitSystem()
        {
        }

        public void Reset()
        {
        }

        public override void BeforeDispose()
        {
            shieldActor?.HecsDestroy();
        }
    }
}