using System;
using Commands;
using Components;
using Components.MonoBehaviourComponents;
using HECSFramework.Core;
using HECSFramework.Unity;
using UnityEngine;

namespace Systems
{
    [Serializable][Documentation(Doc.UI, Doc.Loot, Doc.Inventory, "LootAndInventoryUISystem")]
    public sealed class LootAndInventoryUISystem : BaseSystem, IGlobalStart, IHaveActor
    {
        [Required] private UIAccessProviderComponent uiAccess;
        [Required] private UIAccessPrfbProviderComponent uiPrefabAccess;
        [Required] private InventoriesTilesHolder tilesHolder;

        private InventoriesHolderComponent playerInventories;

        public Actor Actor { get; set; }

        public void GlobalStart()
        {
            uiAccess.Get.GetButton(UIAccessIdentifierMap.Button).onClick.AddListener(Close);

            playerInventories = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>().GetComponent<InventoriesHolderComponent>();

            CreateInventory(playerInventories.GetInventoryByID(InventoryTypeIdentifierMap.LooterDroneInventory), uiAccess.Get.GetUIAccess(UIAccessIdentifierMap.LootView).GetRectTransform(UIAccessIdentifierMap.Root), UIAccessIdentifierMap.ItemTile);
            CreateInventory(playerInventories.GetInventoryByID(InventoryTypeIdentifierMap.PlayerStorageInventory), uiAccess.Get.GetUIAccess(UIAccessIdentifierMap.StorageInventoryView).GetRectTransform(UIAccessIdentifierMap.Root), UIAccessIdentifierMap.ItemTile);
            CreateInventory(playerInventories.GetInventoryByID(InventoryTypeIdentifierMap.PlayerWeaponInventoryLeft), uiAccess.Get.GetUIAccess(UIAccessIdentifierMap.WeaponInventoryView).GetRectTransform(UIAccessIdentifierMap.WeaponColumns_1), UIAccessIdentifierMap.ItemTile);
            CreateInventory(playerInventories.GetInventoryByID(InventoryTypeIdentifierMap.PlayerWeaponInventoryRight), uiAccess.Get.GetUIAccess(UIAccessIdentifierMap.WeaponInventoryView).GetRectTransform(UIAccessIdentifierMap.WeaponColumns_2), UIAccessIdentifierMap.ItemTile);
            CreateInventory(playerInventories.GetInventoryByID(InventoryTypeIdentifierMap.PlayerArtefactInventory), uiAccess.Get.GetUIAccess(UIAccessIdentifierMap.ArtefactsInventoryView).GetRectTransform(UIAccessIdentifierMap.Root), UIAccessIdentifierMap.ItemTile);
            CreateInventory(playerInventories.GetInventoryByID(InventoryTypeIdentifierMap.PlayerUtilityInventory), uiAccess.Get.GetUIAccess(UIAccessIdentifierMap.UtilitiesInventoryView).GetRectTransform(UIAccessIdentifierMap.Root), UIAccessIdentifierMap.ItemTile);

            Owner.Command(new UpdateFitLayoutCommand());
        }

        private void CreateInventory(IInventoryComponent inventoryComponent, Transform root, int prefabIndex)
        {
            using var inventoryItems = inventoryComponent.GetItems();

            var tilePref = uiPrefabAccess.Get.GetPrefab(prefabIndex);
            var itemInfoSource = new BaseItemInfoSource(Owner.World);

            for (int i = 0; i < inventoryItems.Count; i++) 
            {
                var lootTile = MonoBehaviour.Instantiate(tilePref, root).GetComponent<ItemTileMonoComponent>();
                lootTile.Init(itemInfoSource);
                lootTile.DrawTile(inventoryItems.Items[i].Key, inventoryComponent.InventoryID);
                tilesHolder.AddTile(inventoryComponent.InventoryID, lootTile);
            }
        }

        public override void InitSystem()
        {
        }

        private void Close()
        {
            Owner.World.Command(new LootWindowClosedCommand());
            Owner.Command(new HideUICommand());
        }

        public override void BeforeDispose()
        {
            base.BeforeDispose();
            
            uiAccess.Get.GetButton(UIAccessIdentifierMap.Button).onClick.RemoveListener(Close);
        }
    }
}