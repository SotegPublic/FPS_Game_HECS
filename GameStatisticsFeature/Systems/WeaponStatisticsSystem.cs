using System;
using HECSFramework.Core;
using Components;
using Commands;
using UnityEngine;

namespace Systems
{
	[Serializable][Documentation(Doc.Statistics, "this system collect weapon statistics")]
    public sealed class WeaponStatisticsSystem : BaseSystem, IReactGlobalCommand<UpdateWeaponDamageStatisticsCommand>,
        IReactGlobalCommand<CleanRaidGlobalCommand>, IRequestProvider<BestWeaponRequestResult>
    {
        [Required] private WeaponStatisticsHolderComponent weaponStatisticsHolder;

        public void CommandGlobalReact(UpdateWeaponDamageStatisticsCommand command)
        {
            if (!weaponStatisticsHolder.IsContainWeapon(command.WeaponGuid))
            {
                var weaponInfo = Owner.World.Request<WeaponInfoRequestResult, AbilityInfoRequestCommand>(
                    new AbilityInfoRequestCommand { AbilityGuid = command.WeaponGuid });

                weaponStatisticsHolder.AddWeapon(command.WeaponGuid, weaponInfo);
            }
            
            weaponStatisticsHolder.UpdateDamage(command.WeaponGuid, command.Damage);
        }

        public void CommandGlobalReact(CleanRaidGlobalCommand command)
        {
            weaponStatisticsHolder.ResetStatistics();
        }

        public override void InitSystem()
        {
        }

        public BestWeaponRequestResult Request()
        {
            (string name, Sprite sprite, float damage) bestWeaponTuple = default;

            foreach (var weaponStatistics in weaponStatisticsHolder.DamageByWeapon)
            {
                if (weaponStatistics.Value.WeaponDamage > bestWeaponTuple.damage)
                    bestWeaponTuple = (weaponStatistics.Value.WeaponName, weaponStatistics.Value.WeaponSprite, weaponStatistics.Value.WeaponDamage);
            }

            return new BestWeaponRequestResult 
            { 
                Name = bestWeaponTuple.name,
                WeaponDamage = bestWeaponTuple.damage,
                WeaponSprite = bestWeaponTuple.sprite 
            };
        }
    }
}

public struct BestWeaponRequestResult
{
    public string Name;
    public float WeaponDamage;
    public Sprite WeaponSprite;
}

namespace Commands
{
    [Serializable]
    [Documentation(Doc.Statistics, "UpdateWeaponDamageStatisticsCommand")]
    public struct UpdateWeaponDamageStatisticsCommand : IGlobalCommand
    {
        public Guid WeaponGuid;
        public float Damage;
    }
}