using HECSFramework.Core;
using System;

namespace Components
{
    [Serializable][Documentation(Doc.Buff, "ShieldVampiricTagComponent")]
    public sealed class ShieldVampiricTagComponent : BaseComponent
    {
        private int stacksCount;

        public void AddStack()
        {
            stacksCount++;
        }

        public void RemoveBuff()
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