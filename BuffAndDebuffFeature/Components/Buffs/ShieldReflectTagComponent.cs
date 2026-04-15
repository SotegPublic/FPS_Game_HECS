using HECSFramework.Core;
using System;

namespace Components
{
    [Serializable][Documentation(Doc.Shield, Doc.Buff, "ShieldReflectTagComponent")]
    public sealed class ShieldReflectTagComponent : BaseComponent
    {
        private int stacksCount;

        public void AddStack()
        {
            stacksCount++;
        }

        public void RemoveStack()
        {
            stacksCount--;

            if (stacksCount <= 0)
                Owner.RemoveComponent(this);
        }

        public override void BeforeDispose()
        {
            stacksCount = 0;
        }
    }
}