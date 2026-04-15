using HECSFramework.Core;
using HECSFramework.Unity;
using UnityEngine;

namespace Components.MonoBehaviourComponents
{
    public abstract class ResourcesNodeMonoComponent : MonoBehaviour, IHaveActor
    {
        public Actor Actor { get; set; }

        public bool IsAlive => Actor.Entity.IsAliveAndNotDead();

        public abstract void KillNode();
    }
}


