using HECSFramework.Core;

[Documentation(DocFeature.ShootingZoneFeature, "main class for looter drone states")]
public abstract class LooterDroneState : BaseFSMState
{
    public readonly static int FollowState = IndexGenerator.GenerateIndex("FollowState");
    public readonly static int GoToCollectState = IndexGenerator.GenerateIndex("GoToCollectState");
    public readonly static int CollectState = IndexGenerator.GenerateIndex("CollectState");
    public readonly static int AwaitLootState = IndexGenerator.GenerateIndex("AwaitLootState");
    public readonly static int GoToLootState = IndexGenerator.GenerateIndex("GoToLootState");
    public readonly static int GetLootState = IndexGenerator.GenerateIndex("GetLootState");
    public readonly static int RotateToStartPositionState = IndexGenerator.GenerateIndex("RotateToStartPositionState");
    public readonly static int ReturnState = IndexGenerator.GenerateIndex("ReturnState");
    public readonly static int RotateOnStartPositionState = IndexGenerator.GenerateIndex("RotateOnStartPositionState");

    public readonly static int TryFindAnotherLootState = IndexGenerator.GenerateIndex("TryFindAnotherLootState");
    public readonly static int RotateToLootState = IndexGenerator.GenerateIndex("RotateToLootState");

    public LooterDroneState(StateMachine stateMachine, int nextDefaultState) : base(stateMachine, nextDefaultState)
    {
    }
}
