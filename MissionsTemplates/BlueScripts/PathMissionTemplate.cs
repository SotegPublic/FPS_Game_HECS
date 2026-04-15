using BluePrints.Identifiers;
using Helpers;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(PathMissionTemplate), menuName = "GD/Configs/" + nameof(PathMissionTemplate), order = 1)]
public class PathMissionTemplate : ScriptableObject
{
    [SerializeField][IdentifierDropDown(nameof(EnterZoneIdentifier))] private int enterID;
    [SerializeField][IdentifierDropDown(nameof(MissionTypeIdentifier))] private int missionTypeID;
    [SerializeField][IdentifierDropDown(nameof(ShootingZoneIdentifier))] private int[] path;

    public int EnterID => enterID;
    public int MissionTypeID => missionTypeID;
    public int[] Path => path;
}
