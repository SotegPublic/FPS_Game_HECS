using System;
using BluePrints.Identifiers;
using Helpers;
using UnityEngine;

namespace BluePrints.Configs
{
    [CreateAssetMenu(fileName = nameof(ShelterRoomsIconsConfig), menuName = "BluePrints/Configs/" + nameof(ShelterRoomsIconsConfig))]
    public class ShelterRoomsIconsConfig : ScriptableObject
    {
        [SerializeField] private RoomIconConfig[] configs;

        public Texture2D GetIcon(int roomID)
        {
            for(int i = 0; i < configs.Length; i++)
            {
                if(roomID == configs[i].RoomID)
                    return configs[i].Icon;
            }

            return null;
        }
    }

    [Serializable]
    public class RoomIconConfig
    {
        [SerializeField] private Texture2D icon;
        [SerializeField][IdentifierDropDown(nameof(RoomIdentifier))] int roomID;

        public Texture2D Icon => icon;
        public int RoomID => roomID;
    }
}