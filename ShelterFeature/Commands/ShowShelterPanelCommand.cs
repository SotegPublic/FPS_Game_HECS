using HECSFramework.Core;
using System;

namespace Commands
{
    [Serializable]
    [Documentation(Doc.UI, "this command use for switch between shelter UI panels")]
    public struct ShowShelterPanelCommand : IGlobalCommand
    {
        public int PanelID;
    }
}