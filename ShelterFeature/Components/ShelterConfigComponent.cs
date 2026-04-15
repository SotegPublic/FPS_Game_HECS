using System;
using HECSFramework.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Components
{
    [Serializable]
    [Documentation(Doc.Shelter, Doc.Config, "here we set shelter parameters")]
    public sealed class ShelterConfigComponent : BaseComponent
    {
        [SerializeField] private float xOffset = 2f;
        [SerializeField] private float yOffset = 0f;
        [SerializeField] private float roomWidth = 4.6f;
        [SerializeField] private float roomHeight = 1.7f;
        [SerializeField] private float roomPadding = 0.05f;
        [SerializeField] private int roomsVerticalCount = 3;
        [SerializeField] private int roomsHorizontalCount = 3;


        public float XOffset => xOffset;
        public float YOffset => yOffset;
        public float RoomWidth => roomWidth;
        public float RoomHeight => roomHeight;
        public float RoomPadding => roomPadding;
        public int RoomsVerticalCount => roomsVerticalCount;
        public int RoomsHorizontalCount => roomsHorizontalCount;

        #region Editor
#if UNITY_EDITOR
        public static event Action OpenEditor;

        [Button("Configurate rooms matrix")]
        private void OpenRoomsConfiguratorWindow()
        {
            OpenEditor?.Invoke();
        }
#endif
        #endregion
    }
}