using Commands;
using Components;
using Cysharp.Threading.Tasks;
using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

namespace Systems
{
	[Serializable][Documentation(Doc.Shelter, "this system create rooms")]
    public sealed class CreateShelterRoomsSystem : BaseSystem, IReactGlobalCommand<CreateRoomCommand>
    {
        [Required] private ShelterFeatureStateComponent stateComponent;
        [Required] private ShelterRoomContainersHolderComponent roomsHolder;
        [Required] private NewRoomPanelStateComponent newRoomPanelHolder;

        public void CommandGlobalReact(CreateRoomCommand command)
        {
            if(roomsHolder.TryGetContainerByRoomIdentifier(command.RoomTypeID, out var container))
            {
                CreateRoomAsync(command, container).Forget();
                ChangeAddNewRoomPanelPosition();
            }
        }

        private void ChangeAddNewRoomPanelPosition()
        {
            var currentCell = newRoomPanelHolder.CurrentCellForPanel;

            if (currentCell.CellGridIndex.y + 1 < stateComponent.ShelterGrid.GetLength(1))
            {
                var targetCell = stateComponent.ShelterGrid[currentCell.CellGridIndex.x, currentCell.CellGridIndex.y + 1];
                newRoomPanelHolder.AddNewRoomComponent.gameObject.transform.position = targetCell.CellCenter;
                newRoomPanelHolder.CurrentCellForPanel = targetCell;
            }
            else
            {
                newRoomPanelHolder.AddNewRoomComponent.gameObject.SetActive(false);
            }
        }

        private async UniTask CreateRoomAsync(CreateRoomCommand command, EntityContainer container)
        {
            var targetCell = newRoomPanelHolder.CurrentCellForPanel;
            var parent = Owner.World.GetEntityBySingleComponent<RoomsParentTagComponent>().GetTransform();

            var roomActor = await container.GetActor(position: targetCell.CellCenter, transform: parent);
            roomActor.Init();

            targetCell.RoomEntity = roomActor.Entity;

            var shelterRoomsComponent = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<ShelterRoomsComponent>();
            shelterRoomsComponent.AddNewRoom(targetCell.CellGridIndex, command.RoomTypeID);

            roomActor.Entity.GetComponent<RoomTagComponent>().SetRoomGridIndex(targetCell.CellGridIndex);
        }

        public override void InitSystem()
        {
        }
    }
}

namespace Commands
{
    [Serializable][Documentation(Doc.Shelter, "Create room command")]
    public struct CreateRoomCommand : IGlobalCommand
    {
        public int RoomTypeID;
    }
}