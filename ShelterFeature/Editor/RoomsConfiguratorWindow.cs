using BluePrints.Configs;
using BluePrints.Identifiers;
using Components;
using HECSFramework.Unity;
using HECSFramework.Unity.Helpers;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class RoomsConfiguratorWindow : OdinEditorWindow
{
    [HorizontalGroup("Rooms table group", Width = 400, PaddingLeft = 20)]
    [TableMatrix(DrawElementMethod = nameof(DrawElement), HorizontalTitle = "Rooms table", SquareCells = true)]
    [ShowInInspector] private static RoomIdentifier[,] roomsMatrix;

    private static ShelterRoomsIconsConfig iconConfigs;
    private static EntityContainer shelterFeature;
    private static EntityContainer playerFeature;
    private static ShelterConfigComponent configComponent;

    static RoomsConfiguratorWindow()
    {
        ShelterConfigComponent.OpenEditor += OpenConfiguratorWindow;
    }

    public static void OpenConfiguratorWindow()
    {
        var shelterFeatureContainer = new SOProvider<EntityContainer>()
            .GetCollection().Where(x => x.IsHaveComponent<ShelterFeatureTagComponent>()).FirstOrDefault();

        var playerFeatureContainer = new SOProvider<EntityContainer>()
            .GetCollection().Where(x => x.IsHaveComponent<PlayerTagComponent>()).FirstOrDefault();

        iconConfigs = new SOProvider<ShelterRoomsIconsConfig>().GetCollection().FirstOrDefault();

        GetWindow<RoomsConfiguratorWindow>().Show();
        shelterFeature = shelterFeatureContainer;
        playerFeature = playerFeatureContainer;

        configComponent = shelterFeature.GetComponent<ShelterConfigComponent>();
        var roomsArray = playerFeature.GetComponent<ShelterRoomsComponent>().RoomsArray;

        roomsMatrix = new RoomIdentifier[configComponent.RoomsHorizontalCount, configComponent.RoomsVerticalCount];

        if (roomsArray != null || roomsArray.Length != 0)
        {
            for (int i = 0; i < roomsArray.Length; i++)
            {
                if(roomsArray[i].RoomMatrixIndex.x < roomsMatrix.GetLength(0) && roomsArray[i].RoomMatrixIndex.y < roomsMatrix.GetLength(1))
                {
                    var identifier = new SOProvider<RoomIdentifier>()
                        .GetCollection().Where(x => x.Id == roomsArray[i].RoomID).FirstOrDefault();
                    roomsMatrix[roomsArray[i].RoomMatrixIndex.x, roomsArray[i].RoomMatrixIndex.y] = identifier;
                }
            }
        }
    }

    [Button("Save and Close", ButtonSizes.Large)]
    private void SaveAndClose()
    {
        var shelterRoomsComponent = playerFeature.GetComponent<ShelterRoomsComponent>();
        shelterRoomsComponent.UpdateRoomsArray(roomsMatrix);
        EditorUtility.SetDirty(shelterFeature);
        EditorUtility.SetDirty(playerFeature);
        AssetDatabase.SaveAssets();
        this.Close();
    }

    private RoomIdentifier DrawElement(Rect rect, RoomIdentifier value)
    {
        var icon = value is null ? null : iconConfigs.GetIcon(value);

        return (RoomIdentifier)SirenixEditorFields.UnityPreviewObjectField(
            rect: rect,
            value: value,
            texture: icon,
            type: typeof(RoomIdentifier)
        );
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        ShelterConfigComponent.OpenEditor -= OpenConfiguratorWindow;
    }
}
