using HECSFramework.Core;
using System;

namespace Components
{
    [Serializable]
    [Documentation(Doc.UI, Doc.Shelter, "MissionsPanelStateComponent")]
    public sealed class MissionsPanelStateComponent : BaseComponent
    {
        public HECSList<MissionPanelMonoComponent> MissionPanels;

        public void Clear()
        {
            MissionPanels.Clear();
            MissionPanels = null;
        }
    }
}