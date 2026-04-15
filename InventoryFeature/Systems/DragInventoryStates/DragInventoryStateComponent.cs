using Components.MonoBehaviourComponents;
using HECSFramework.Core;
using Systems;
using UnityEngine;

namespace Components
{
    [Documentation(Doc.UI, Doc.Inventory, Doc.DragUI, "we use it in " + nameof(LootAndInventoryUIDragSystem) + "and process in local fsm")]
    public class DragInventoryStateComponent : BaseComponent
    {
        public bool IsPressed;
        public bool IsPressedChecked;

        public float PressedTime;
        public Vector2 From;
        public Vector2 CurrentCoord;

        public int FromInventory;
        public int SlotIndex;
        public ItemTileMonoComponent CurrentTile;

        public ItemTileMonoComponent DragView;

        public float DragLenght()
        {
            return (From - CurrentCoord).magnitude;
        }
    }
}