using BluePrints.Identifiers;
using Components;
using Helpers;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(SpawnMissionTemplate), menuName = "GD/Configs/" + nameof(SpawnMissionTemplate), order = 1)]
public class SpawnMissionTemplate : ScriptableObject
{
    [SerializeField][IdentifierDropDown(nameof(GradeIdentifier))] private int gradeID;
    [SerializeField] private ZoneScenario[] spawns;

    public int GradeID => gradeID;
    public ZoneScenario[] Spawns => spawns;
}