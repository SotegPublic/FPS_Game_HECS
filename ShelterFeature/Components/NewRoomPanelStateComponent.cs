using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using Systems;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Components
{
    [Serializable][Documentation(Doc.Shelter, "NewRoomPanelStateComponent")]
    public sealed class NewRoomPanelStateComponent : BaseComponent
    {
        [SerializeField] private AssetReference addRoomButtonReference;

        [HideInInspector] public bool IsPanelActive;
        [HideInInspector] public AddNewRoomMonoComponent AddNewRoomComponent;
        [HideInInspector] public RoomCell CurrentCellForPanel;

        public AssetReference AddRoomButtonReference => addRoomButtonReference;
    }
}