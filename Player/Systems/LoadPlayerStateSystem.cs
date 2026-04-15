using System;
using Commands;
using Components;
using HECSFramework.Core;

namespace Systems
{
    [Serializable]
    [Documentation(Doc.Player, Doc.Load, Doc.Save, "System for loading and saving player")]
    public sealed class LoadPlayerStateSystem : BaseGameStateSystem
    {
        protected override int State { get; } = GameStateIdentifierMap.LoadPlayerState;

        public override void InitSystem() { }

        protected override void ProcessState(int from, int to)
        {
            var player = Owner.World.GetEntityBySingleComponent<PlayerTagComponent>();
            var openedEnters = player.GetComponent<OpenedEntersComponent>();
            openedEnters.AddEnterZoneID(EnterZoneIdentifierMap.FirstEntrance);

            //firstScenario
            Owner.World.Request<AddItemRequestResult, AddItemRequestCommand>(new AddItemRequestCommand
            {
                ItemID = EntityContainersMap._Beretta_Grade0,
                Count = 1,
                InventoryID = InventoryTypeIdentifierMap.PlayerWeaponInventoryLeft
            });

            Owner.World.Request<AddItemRequestResult, AddItemRequestCommand>(new AddItemRequestCommand
            {
                ItemID = EntityContainersMap._MiniGun,
                Count = 1,
                InventoryID = InventoryTypeIdentifierMap.PlayerWeaponInventoryRight
            });

            //secondScenario
            //Owner.World.Request<AddItemRequestResult, AddItemRequestCommand>(new AddItemRequestCommand
            //{
            //    ItemID = EntityContainersMap._Beretta_Item,
            //    Count = 1,
            //    InventoryID = InventoryTypeIdentifierMap.PlayerWeaponInventoryLeft
            //});

            //Owner.World.Request<AddItemRequestResult, AddItemRequestCommand>(new AddItemRequestCommand
            //{
            //    ItemID = EntityContainersMap._Magnum_Item,
            //    Count = 1,
            //    InventoryID = InventoryTypeIdentifierMap.PlayerWeaponInventoryRight
            //});

            //Owner.World.Request<AddItemRequestResult, AddItemRequestCommand>(new AddItemRequestCommand
            //{
            //    ItemID = EntityContainersMap._FireArtefact_Item,
            //    Count = 1,
            //    InventoryID = InventoryTypeIdentifierMap.PlayerArtefactInventory
            //});

            //thirdScenario
            //Owner.World.Request<AddItemRequestResult, AddItemRequestCommand>(new AddItemRequestCommand
            //{
            //    ItemID = EntityContainersMap._Beretta_Item,
            //    Count = 1,
            //    InventoryID = InventoryTypeIdentifierMap.PlayerWeaponInventoryLeft
            //});

            //Owner.World.Request<AddItemRequestResult, AddItemRequestCommand>(new AddItemRequestCommand
            //{
            //    ItemID = EntityContainersMap._Magnum_Item,
            //    Count = 1,
            //    InventoryID = InventoryTypeIdentifierMap.PlayerWeaponInventoryRight
            //});

            //Owner.World.Request<AddItemRequestResult, AddItemRequestCommand>(new AddItemRequestCommand
            //{
            //    ItemID = EntityContainersMap._AK47_Item,
            //    Count = 1,
            //    InventoryID = InventoryTypeIdentifierMap.PlayerWeaponInventoryRight
            //});

            //Owner.World.Request<AddItemRequestResult, AddItemRequestCommand>(new AddItemRequestCommand
            //{
            //    ItemID = EntityContainersMap._FireArtefact_Item,
            //    Count = 1,
            //    InventoryID = InventoryTypeIdentifierMap.PlayerArtefactInventory
            //});

            //fourth scenario
            //Owner.World.Request<AddItemRequestResult, AddItemRequestCommand>(new AddItemRequestCommand
            //{
            //    ItemID = EntityContainersMap._Beretta_Item,
            //    Count = 1,
            //    InventoryID = InventoryTypeIdentifierMap.PlayerWeaponInventoryLeft
            //});

            //Owner.World.Request<AddItemRequestResult, AddItemRequestCommand>(new AddItemRequestCommand
            //{
            //    ItemID = EntityContainersMap._Magnum_Item,
            //    Count = 1,
            //    InventoryID = InventoryTypeIdentifierMap.PlayerWeaponInventoryRight
            //});

            //Owner.World.Request<AddItemRequestResult, AddItemRequestCommand>(new AddItemRequestCommand
            //{
            //    ItemID = EntityContainersMap._AK47_Item,
            //    Count = 1,
            //    InventoryID = InventoryTypeIdentifierMap.PlayerWeaponInventoryRight
            //});

            //Owner.World.Request<AddItemRequestResult, AddItemRequestCommand>(new AddItemRequestCommand
            //{
            //    ItemID = EntityContainersMap._P90_Item,
            //    Count = 1,
            //    InventoryID = InventoryTypeIdentifierMap.PlayerWeaponInventoryLeft
            //});

            //Owner.World.Request<AddItemRequestResult, AddItemRequestCommand>(new AddItemRequestCommand
            //{
            //    ItemID = EntityContainersMap._FireArtefact_Item,
            //    Count = 1,
            //    InventoryID = InventoryTypeIdentifierMap.PlayerArtefactInventory
            //});


            // EntityManager.ResolversMap.InitCustomJSONResolvers();
            //
            // if (!LoadPlayer())
            // {
            //     //to get default player state on first launch
            //     EntityManager.Command(new PlayerHasNoSavesCommand());
            // }
            //
            // EntityManager.Default.Command(new PlayerLoadedCommand());
            EndState();
        }

        // private void SavePlayer()
        // {
        //     var container = new JSONEntityContainer();
        //     container.SerializeEntitySavebleOnly(Owner);
        //     var json = JsonConvert.SerializeObject(container);
        //     SaveManager.SaveJson(SavePathProvider.PlayerSavePath, json);
        // }
        //
        // private bool LoadPlayer()
        // {
        //     if (!SaveManager.TryLoadJson(SavePathProvider.PlayerSavePath, out var json))
        //     {
        //         HECSDebug.Log("There is no any saves, apply defaults");
        //         return false;
        //     }
        //     
        //     var container = JsonConvert.DeserializeObject<JSONEntityContainer>(json);
        //     container.DeserializeToEntity(Owner);
        //     return true;
        // }

        // public void CommandGlobalReact(SaveCommand command)
        // {
        //     SavePlayer();
        // }
    }
}