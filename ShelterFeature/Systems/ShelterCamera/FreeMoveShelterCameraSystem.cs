using Commands;
using Components;
using HECSFramework.Core;
using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace Systems
{
	[Serializable][Documentation(Doc.Shelter, Doc.Camera, "this system move camera")]
    public sealed class FreeMoveShelterCameraSystem : BaseSystem, IGlobalStart, IReactGlobalCommand<EndGameStateCommand>,
        IReactGlobalCommand<StartMoveShelterCameraCommand>, IReactGlobalCommand<MoveShelterCameraCommand>, IReactGlobalCommand<StopMoveShelterCameraCommand>
    {
        [Required] private UnityTransformComponent transformComponent;
        [Required] private ShelterCameraTargetComponent cameraTarget;
        [Required] private ShelterCameraConfigComponent config;
        [Required] private FreeShelterCameraStateComponent systemState;

        private ShelterFeatureStateComponent shelterStateComponent;
        private ShelterConfigComponent shelterConfig;
        private MainCameraComponent cameraComponent;

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

                var maxHorizontalIndex = shelterStateComponent.ShelterGrid.GetLength(0) - 1;
                var horizontalEdgeX = shelterStateComponent.ShelterGrid[0, 0].CellCenter.x + config.XOffset;
                var horizontalEdgeY = shelterStateComponent.ShelterGrid[maxHorizontalIndex, 0].CellCenter.x + config.XOffset;
                systemState.HorizontalEdge = new Vector2(horizontalEdgeX, horizontalEdgeY);
                
                
                var maxVerticalIndex = shelterStateComponent.ShelterGrid.GetLength(1) - 1;
                var verticalEdgeX = shelterStateComponent.ShelterGrid[0, maxVerticalIndex].CellCenter.y;
                var verticalEdgeY = shelterStateComponent.ShelterGrid[0, 0].CellCenter.y;
                systemState.VerticalEdge = new Vector2(verticalEdgeX, verticalEdgeY);
            }
        }

        public void GlobalStart()
        {
            var shelterFeature = Owner.World.GetEntityBySingleComponent<ShelterFeatureTagComponent>();
            shelterStateComponent = shelterFeature.GetComponent<ShelterFeatureStateComponent>();
            shelterConfig = shelterFeature.GetComponent<ShelterConfigComponent>();
        }

        public void CommandGlobalReact(StartMoveShelterCameraCommand command)
        {
            if (config.MoveType != ShelterCameraMoveType.FreeMove)
                return;

            systemState.StartInputPosition = command.StartPosition;
            systemState.CameraStartPosition = transformComponent.Transform.position;
        }

        public void CommandGlobalReact(MoveShelterCameraCommand command)
        {
            if (config.MoveType != ShelterCameraMoveType.FreeMove)
                return;

            MoveCamera(command);
        }

        public void CommandGlobalReact(StopMoveShelterCameraCommand command)
        {
            if (config.MoveType != ShelterCameraMoveType.FreeMove)
                return;

            UpdateTargetRoom();
        }

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

        private void MoveCamera(MoveShelterCameraCommand command)
        {
            var currentPosition = command.CurrentPointerPosition;

            var startToSpace = cameraComponent.Camera.ScreenToWorldPoint(new Vector3(systemState.StartInputPosition.x, systemState.StartInputPosition.y, cameraComponent.Camera.nearClipPlane));
            var currentToSpace = cameraComponent.Camera.ScreenToWorldPoint(new Vector3(currentPosition.x, currentPosition.y, cameraComponent.Camera.nearClipPlane));

            var inputDir = currentToSpace - startToSpace;
            var newPos = transformComponent.Transform.localPosition;
            var delta = inputDir * config.FreeMoveCameraSpeed;

            newPos.x = Mathf.Clamp(systemState.CameraStartPosition.x - delta.x, systemState.HorizontalEdge.x, systemState.HorizontalEdge.y);
            newPos.y = Mathf.Clamp(systemState.CameraStartPosition.y - delta.y, systemState.VerticalEdge.x, systemState.VerticalEdge.y);

            transformComponent.Transform.localPosition = newPos;
        }
    }
}