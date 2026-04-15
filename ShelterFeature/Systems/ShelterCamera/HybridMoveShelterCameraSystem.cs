using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;

namespace Systems
{
	[Serializable][Documentation(Doc.Shelter, Doc.Camera, "hybrid move shelter camera system")]
    public sealed class HybridMoveShelterCameraSystem : BaseSystem, IGlobalStart, IUpdatable, IReactGlobalCommand<EndGameStateCommand>,
        IReactCommand<InputStartedCommand>, IReactCommand<InputPerformedCommand>, IReactCommand<InputEndedCommand>
    {
        [Required] private UnityTransformComponent transformComponent;
        [Required] private ShelterCameraTargetComponent cameraTarget;
        [Required] private ShelterCameraConfigComponent config;
        [Required] private HybridShelterCameraStateComponent systemState;

        private ShelterFeatureStateComponent shelterStateComponent;
        private ShelterConfigComponent shelterConfig;
        private MainCameraComponent cameraComponent;

        public void GlobalStart()
        {
            var shelterFeature = Owner.World.GetEntityBySingleComponent<ShelterFeatureTagComponent>();
            shelterStateComponent = shelterFeature.GetComponent<ShelterFeatureStateComponent>();
            shelterConfig = shelterFeature.GetComponent<ShelterConfigComponent>();
        }

        public override void InitSystem()
        {
            cameraComponent = Owner.World.GetSingleComponent<MainCameraComponent>();
        }

        public void CommandGlobalReact(EndGameStateCommand command)
        {
            if (command.GameState == GameStateIdentifierMap.CreateShelterGridState)
            {
                cameraTarget.TargetCell = shelterStateComponent.ShelterGrid[0, 0];
                transformComponent.Transform.position = new Vector3(
                    cameraTarget.TargetCell.CellCenter.x + config.XOffset,
                    cameraTarget.TargetCell.CellCenter.y,
                    transformComponent.Transform.position.z);

                var maxVerticalIndex = shelterStateComponent.ShelterGrid.GetLength(1) - 1;
                var verticalEdgeX = shelterStateComponent.ShelterGrid[0, maxVerticalIndex].CellCenter.y;
                var verticalEdgeY = shelterStateComponent.ShelterGrid[0, 0].CellCenter.y;
                systemState.VerticalEdge = new Vector2(verticalEdgeX, verticalEdgeY);
            }
        }

        public void CommandReact(InputStartedCommand command)
        {
            if (!IsCanProcessInputCommand(command.Index))
                return;

            systemState.IsInputActive = true;
            systemState.StartInputPosition = command.Context.ReadValue<Vector2>();
            systemState.CameraStartPosition = transformComponent.Transform.position;
        }

        public void CommandReact(InputPerformedCommand command)
        {
            if (!IsCanProcessInputCommand(command.Index))
                return;

            MoveCamera(command);

            systemState.CurrentInputPosition = command.Context.ReadValue<Vector2>();
            var dirXDelta = systemState.StartInputPosition.x - systemState.CurrentInputPosition.x;

            if (Mathf.Abs(dirXDelta) < config.HybridSwipeThreshold)
                return;

            var dir = dirXDelta > 0 ? DirectionTypes.Right : DirectionTypes.Left;

            if (systemState.CurrentDirection != dir)
                SetNewOffset(dir);
        }

        public void CommandReact(InputEndedCommand command)
        {
            if (!IsCanProcessInputCommand(command.Index))
                return;

            systemState.IsInputActive = false;

            var dirXDelta = systemState.StartInputPosition.x - systemState.CurrentInputPosition.x;
            if (Mathf.Abs(dirXDelta) > config.HybridSwipeThreshold)
            {
                ChangeRoomsColumn();
                systemState.CurrentDirection = DirectionTypes.None;
                systemState.NewOffsetX = 0;
            }
            else
            {
                UpdateTargetRoom();
            }
        }

        #region VerticalMove
        private void MoveCamera(InputPerformedCommand command)
        {
            systemState.CurrentInputPosition = command.Context.ReadValue<Vector2>();

            var startToSpace = cameraComponent.Camera.ScreenToWorldPoint(new Vector3(systemState.StartInputPosition.x, systemState.StartInputPosition.y, cameraComponent.Camera.nearClipPlane));
            var currentToSpace = cameraComponent.Camera.ScreenToWorldPoint(new Vector3(systemState.CurrentInputPosition.x, systemState.CurrentInputPosition.y, cameraComponent.Camera.nearClipPlane));

            var inputDir = currentToSpace - startToSpace;
            var newPos = transformComponent.Transform.localPosition;
            var delta = inputDir * config.FreeMoveCameraSpeed;

            newPos.y = Mathf.Clamp(systemState.CameraStartPosition.y - delta.y, systemState.VerticalEdge.x, systemState.VerticalEdge.y);

            transformComponent.Transform.localPosition = newPos;
        }
        #endregion

        #region HorizontalMove
        private void SetNewOffset(DirectionTypes dir)
        {
            systemState.CurrentDirection = dir;

            switch (systemState.CurrentDirection)
            {
                case DirectionTypes.Right:
                case DirectionTypes.Left:
                    var horisontalModifier = systemState.CurrentDirection == DirectionTypes.Left ? -1 : 1;
                    systemState.NewOffsetX = cameraTarget.TargetCell.CellCenter.x + config.XOffset + config.CameraOffset * horisontalModifier;
                    break;
                case DirectionTypes.Down:
                case DirectionTypes.Up:
                case DirectionTypes.None:
                default:
                    break;
            }
        }

        private void ChangeRoomsColumn()
        {
            switch (systemState.CurrentDirection)
            {
                case DirectionTypes.Right:
                case DirectionTypes.Left:
                    var horisontalMove = systemState.CurrentDirection == DirectionTypes.Right ? 1 : -1;
                    var newHorisontalGridIndex = cameraTarget.TargetCell.CellGridIndex.x + horisontalMove;

                    if (newHorisontalGridIndex >= shelterStateComponent.ShelterGrid.GetLength(0) || newHorisontalGridIndex < 0)
                    {
                        transformComponent.Transform.position = new Vector3(
                            cameraTarget.TargetCell.CellCenter.x + config.XOffset,
                            transformComponent.Transform.position.y,
                            transformComponent.Transform.position.z);
                        UpdateTargetRoom();
                    }
                    else
                    {
                        var newX = shelterStateComponent.ShelterGrid[newHorisontalGridIndex, cameraTarget.TargetCell.CellGridIndex.y].CellCenter.x + config.XOffset;
                        systemState.NewCameraPosition = new Vector3(newX, transformComponent.Transform.position.y, transformComponent.Transform.position.z);
                        systemState.IsChangeColumnInProgress = true;
                    }
                    break;
                case DirectionTypes.Down:
                case DirectionTypes.Up:
                case DirectionTypes.None:
                default:
                    break;
            }
        }

        public void UpdateLocal()
        {
            if (config.MoveType != ShelterCameraMoveType.Hybrid)
                return;

            if (systemState.IsChangeColumnInProgress)
            {
                MoveCameraToNewColumn();
                return;
            }

            if (systemState.IsInputActive)
            {
                MoveCameraToOffset();
            }
        }

        private void MoveCameraToOffset()
        {
            if (systemState.CurrentDirection == DirectionTypes.None)
                return;

            var newPosition = new Vector3(systemState.NewOffsetX, transformComponent.Transform.position.y, transformComponent.Transform.position.z);
            transformComponent.Transform.position = Vector3.SmoothDamp(transformComponent.Transform.position, newPosition, ref systemState.Velocity, config.MoveByRoomsCameraSpeed);
        }

        private void MoveCameraToNewColumn()
        {
            transformComponent.Transform.position = Vector3.SmoothDamp(transformComponent.Transform.position, systemState.NewCameraPosition, ref systemState.Velocity, config.MoveByRoomsCameraSpeed);

            if ((systemState.NewCameraPosition - transformComponent.Transform.position).sqrMagnitude < config.SqrChangeRoomThreshold)
            {
                transformComponent.Transform.position = systemState.NewCameraPosition;
                systemState.IsChangeColumnInProgress = false;
                UpdateTargetRoom();
            }
        }
        #endregion

        private void UpdateTargetRoom()
        {
            Vector3 cameraPos = transformComponent.Transform.position;

            var cellWidth = shelterConfig.RoomWidth + shelterConfig.RoomPadding;
            var cellHeight = shelterConfig.RoomHeight + shelterConfig.RoomPadding;
            var roomCenterX = cellWidth / 2f;
            var roomCenterY = -cellHeight / 2f;

            var roomX = Mathf.FloorToInt((cameraPos.x + roomCenterX) / cellWidth);
            var roomY = Mathf.FloorToInt(-(cameraPos.y + roomCenterY) / cellHeight);

            cameraTarget.TargetCell = shelterStateComponent.ShelterGrid[roomX, roomY];
        }

        private bool IsCanProcessInputCommand(int inputIndex)
        {
            if (config.MoveType != ShelterCameraMoveType.Hybrid)
                return false;
            if (inputIndex != InputIdentifierMap.Touch)
                return false;
            if (systemState.IsChangeColumnInProgress)
                return false;

            return true;
        }
    }
}