using System;
using HECSFramework.Unity;
using HECSFramework.Core;
using UnityEngine;
using Components;
using Commands;
using TMPro;

namespace Systems
{
	[Serializable][Documentation(Doc.Shelter, Doc.Camera, "this system move camera by roomCells")]
    public sealed class MoveByRoomsShelterCameraSystem : BaseSystem, IGlobalStart, IUpdatable, IReactGlobalCommand<EndGameStateCommand>,
        IReactCommand<InputStartedCommand>, IReactCommand<InputPerformedCommand>, IReactCommand<InputEndedCommand>
    {
        [Required] private UnityTransformComponent transformComponent;
        [Required] private ShelterCameraTargetComponent cameraTarget;
        [Required] private ShelterCameraConfigComponent config;
        [Required] private MoveByRoomsShelterCameraStateComponent systemState;

        private ShelterFeatureStateComponent shelterStateComponent;

        public void CommandReact(InputPerformedCommand command)
        {
            if (config.MoveType != ShelterCameraMoveType.MoveByRooms)
                return;

            if (command.Index != InputIdentifierMap.Touch)
                return;

            systemState.CurrentInputPosition = command.Context.ReadValue<Vector2>();

            var dirDelta = systemState.StartInputPosition - systemState.CurrentInputPosition;

            if (dirDelta.sqrMagnitude < config.SqrSwipeThreshold)
                return;

            var dir = DirectionTypes.None;

            if (Mathf.Abs(dirDelta.x) > Mathf.Abs(dirDelta.y))
            {
                dir = dirDelta.x > 0 ? DirectionTypes.Right : DirectionTypes.Left;
            }
            else
            {
                dir = dirDelta.y > 0 ? DirectionTypes.Up : DirectionTypes.Down;
            }

            if (systemState.CurrentDirection != dir)
                SetNewOffset(dir);
        }

        private void SetNewOffset(DirectionTypes dir)
        {
            systemState.CurrentDirection = dir;

            switch (systemState.CurrentDirection)
            {
                case DirectionTypes.Down:
                case DirectionTypes.Up:
                    var verticalModifier = systemState.CurrentDirection == DirectionTypes.Down ? -1 : 1;
                    systemState.NewOffsetPosition = new Vector3(
                        cameraTarget.TargetCell.CellCenter.x + config.XOffset,
                        cameraTarget.TargetCell.CellCenter.y + config.CameraOffset * verticalModifier,
                        transformComponent.Transform.position.z);
                    break;
                case DirectionTypes.Right:
                case DirectionTypes.Left:
                    var horisontalModifier = systemState.CurrentDirection == DirectionTypes.Left ? -1 : 1;
                    systemState.NewOffsetPosition = new Vector3(
                        cameraTarget.TargetCell.CellCenter.x + config.XOffset + config.CameraOffset * horisontalModifier,
                        cameraTarget.TargetCell.CellCenter.y, transformComponent.Transform.position.z);
                    break;
                case DirectionTypes.None:
                default:
                    break;
            }
        }


        public void CommandReact(InputEndedCommand command)
        {
            if (config.MoveType != ShelterCameraMoveType.MoveByRooms)
                return;

            if (command.Index != InputIdentifierMap.Touch)
                return;

            systemState.IsInputActive = false;

            SetNewCameraTarget();

            systemState.CurrentDirection = DirectionTypes.None;
            systemState.NewOffsetPosition = Vector3.zero;
        }

        private void SetNewCameraTarget()
        {
            switch (systemState.CurrentDirection)
            {
                case DirectionTypes.Down:
                case DirectionTypes.Up:
                    var verticalMove = systemState.CurrentDirection == DirectionTypes.Down ? 1 : -1;
                    var newVerticalGridIndex = cameraTarget.TargetCell.CellGridIndex.y + verticalMove;

                    if (newVerticalGridIndex >= shelterStateComponent.ShelterGrid.GetLength(1) || newVerticalGridIndex < 0)
                    {
                        transformComponent.Transform.position = new Vector3(
                            cameraTarget.TargetCell.CellCenter.x + config.XOffset,
                            cameraTarget.TargetCell.CellCenter.y,
                            transformComponent.Transform.position.z);
                    }
                    else
                    {
                        cameraTarget.TargetCell = shelterStateComponent.ShelterGrid[cameraTarget.TargetCell.CellGridIndex.x, newVerticalGridIndex];
                        systemState.NewCameraPosition = new Vector3(
                            cameraTarget.TargetCell.CellCenter.x + config.XOffset,
                            cameraTarget.TargetCell.CellCenter.y,
                            transformComponent.Transform.position.z);
                        systemState.IsChangeRoomInProgress = true;
                    }
                    break;
                case DirectionTypes.Right:
                case DirectionTypes.Left:
                    var horisontalMove = systemState.CurrentDirection == DirectionTypes.Right ? 1 : -1;
                    var newHorisontalGridIndex = cameraTarget.TargetCell.CellGridIndex.x + horisontalMove;

                    if (newHorisontalGridIndex >= shelterStateComponent.ShelterGrid.GetLength(0) || newHorisontalGridIndex < 0)
                    {
                        transformComponent.Transform.position = new Vector3(
                            cameraTarget.TargetCell.CellCenter.x + config.XOffset,
                            cameraTarget.TargetCell.CellCenter.y,
                            transformComponent.Transform.position.z);
                    }
                    else
                    {
                        cameraTarget.TargetCell = shelterStateComponent.ShelterGrid[newHorisontalGridIndex, cameraTarget.TargetCell.CellGridIndex.y];
                        systemState.NewCameraPosition = new Vector3(
                            cameraTarget.TargetCell.CellCenter.x + config.XOffset,
                            cameraTarget.TargetCell.CellCenter.y,
                            transformComponent.Transform.position.z);
                        systemState.IsChangeRoomInProgress = true;
                    }
                    break;
                case DirectionTypes.None:
                default:
                    break;
            }
        }

        public void CommandReact(InputStartedCommand command)
        {
            if (config.MoveType != ShelterCameraMoveType.MoveByRooms)
                return;

            if (command.Index != InputIdentifierMap.Touch)
                return;

            systemState.IsInputActive = true;
            systemState.StartInputPosition = command.Context.ReadValue<Vector2>();
        }

        public void GlobalStart()
        {
            shelterStateComponent = Owner.World.GetEntityBySingleComponent<ShelterFeatureTagComponent>().GetComponent<ShelterFeatureStateComponent>();
        }

        public override void InitSystem()
        {
        }

        public void CommandGlobalReact(EndGameStateCommand command)
        {
            if(command.GameState == GameStateIdentifierMap.CreateShelterGridState)
            {
                

                cameraTarget.TargetCell = shelterStateComponent.ShelterGrid[0, 0];
                transformComponent.Transform.position = new Vector3(
                    cameraTarget.TargetCell.CellCenter.x + config.XOffset,
                    cameraTarget.TargetCell.CellCenter.y,
                    transformComponent.Transform.position.z);
            }
        }

        public void UpdateLocal()
        {
            if (config.MoveType != ShelterCameraMoveType.MoveByRooms)
                return;

            if (systemState.IsChangeRoomInProgress)
            {
                MoveCameraToNewRoom();
                return;
            }

            if (systemState.IsInputActive)
            {
                MoveCameraToOffset();
            }
        }

        private void MoveCameraToNewRoom()
        {
            transformComponent.Transform.position = Vector3.SmoothDamp(transformComponent.Transform.position, systemState.NewCameraPosition, ref systemState.Velocity, config.MoveByRoomsCameraSpeed);

            if((systemState.NewCameraPosition - transformComponent.Transform.position).sqrMagnitude < config.SqrChangeRoomThreshold)
            {
                transformComponent.Transform.position = systemState.NewCameraPosition;
                systemState.IsChangeRoomInProgress = false;
            }
        }

        private void MoveCameraToOffset()
        {
            if (systemState.CurrentDirection == DirectionTypes.None)
                return;

            transformComponent.Transform.position = Vector3.SmoothDamp(transformComponent.Transform.position, systemState.NewOffsetPosition, ref systemState.Velocity, config.MoveByRoomsCameraSpeed);
        }
    }
}