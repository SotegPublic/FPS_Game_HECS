using BluePrints.Identifiers;
using Components;
using Cysharp.Threading.Tasks;
using HECSFramework.Core;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Systems
{
	[Serializable][Documentation(Doc.Shelter, "here we create shelter grid")]
    public sealed class CreateShelterGridSystem : BaseGameStateSystem 
    {
        [Required] private ShelterFeatureStateComponent stateComponent;
        [Required] private ShelterConfigComponent shelterConfig;
        [Required] private NewRoomPanelStateComponent newRoomPanelStateComponent;

        protected override int State => GameStateIdentifierMap.CreateShelterGridState;

        public override void InitSystem()
        {
            stateComponent.ShelterGrid = new RoomCell[shelterConfig.RoomsHorizontalCount, shelterConfig.RoomsVerticalCount];
        }

        protected override void ProcessState(int from, int to)
        {
            GenerateGridAsync().Forget();
        }

        private async UniTask GenerateGridAsync()
        {
            var startX = 0f + shelterConfig.XOffset;
            var startY = 0f + shelterConfig.YOffset;

            var lastRoomIndex = Vector2Int.zero;
            var shelterRoomsComponent = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<ShelterRoomsComponent>();

            var roomsFilter = Owner.World.GetFilter(Filter.Get<RoomTagComponent>());

            for (int i = 0; i < shelterConfig.RoomsHorizontalCount; i++)
            {
                for (int j = 0; j < shelterConfig.RoomsVerticalCount; j++)
                {
                    var cell = new RoomCell
                    {
                        CellGridIndex = new Vector2Int(i, j),
                        CellCenter = new Vector3(
                            startX + (i * (shelterConfig.RoomWidth + shelterConfig.RoomPadding)),
                            startY + (-j * (shelterConfig.RoomHeight + shelterConfig.RoomPadding)))
                    };

                    stateComponent.ShelterGrid[i, j] = cell;
                }
            }

            foreach (var room in roomsFilter)
            {
                var roomGridIndex = room.GetComponent<RoomTagComponent>().RoomGridIndex;
                var cell = stateComponent.ShelterGrid[roomGridIndex.x, roomGridIndex.y];
                cell.RoomEntity = room;

                room.GetTransform().position = cell.CellCenter;

                if (lastRoomIndex.x < roomGridIndex.x)
                {
                    lastRoomIndex = roomGridIndex;
                }
                else
                {
                    if (lastRoomIndex.y < roomGridIndex.y)
                        lastRoomIndex = roomGridIndex;
                }
            }

            await SpawnNewRoomPanelAsync(lastRoomIndex);

            EndState();
        }

        private async UniTask SpawnNewRoomPanelAsync(Vector2Int lastRoomIndex)
        {
            var panelReference = newRoomPanelStateComponent.AddRoomButtonReference;
            var newRoomPanel = await Addressables.InstantiateAsync(panelReference, Vector3.zero, Quaternion.identity);
            var monoComponent = newRoomPanel.GetComponent<AddNewRoomMonoComponent>();
            newRoomPanelStateComponent.AddNewRoomComponent = monoComponent;

            var camera = Owner.World.GetSingleComponent<MainCameraComponent>().Camera;
            monoComponent.Canvas.worldCamera = camera;

            if (lastRoomIndex.y != stateComponent.ShelterGrid.GetLength(1) - 1)
            {
                var targetCell = stateComponent.ShelterGrid[lastRoomIndex.x, lastRoomIndex.y + 1];
                newRoomPanelStateComponent.CurrentCellForPanel = targetCell;
                monoComponent.gameObject.transform.position = targetCell.CellCenter;

                newRoomPanelStateComponent.IsPanelActive = true;
            }
            else
            {
                monoComponent.gameObject.SetActive(false);
            }
        }
    }

    public class RoomCell
    {
        public Vector2Int CellGridIndex;
        public Vector3 CellCenter;
        public Entity RoomEntity;
    }

}