using System;
using HECSFramework.Core;
using Components;

namespace Systems
{
	[Serializable][Documentation(Doc.Loot, Doc.Visual, "this system add local visual lock when spawn loot")]
    public sealed class LootVisualLocalLockSystem : BaseSystem
    {
        public override void InitSystem()
        {
            var gloabalLootEntity = Owner.World.GetEntityBySingleComponent<GlobalLootFeatureTagComponent>();

            gloabalLootEntity.GetOrAddComponent<VisualLocalLockComponent>().AddLock();
        }

        public override void BeforeDispose()
        {
            if (!Owner.World.IsAlive)
                return;

            base.BeforeDispose();
            
            var gloabalLootEntity = Owner.World.GetEntityBySingleComponent<GlobalLootFeatureTagComponent>();

            gloabalLootEntity.GetOrAddComponent<VisualLocalLockComponent>().Remove();
        }
    }
}