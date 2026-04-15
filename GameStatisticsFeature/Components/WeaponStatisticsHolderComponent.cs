using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    [Serializable][Documentation(Doc.Statistics, "here we hold damage statistics by each weapon")]
    public sealed class WeaponStatisticsHolderComponent : BaseComponent
    {
        private Dictionary<Guid, WeaponStatistics> damageByWeapon = new Dictionary<Guid, WeaponStatistics>(12);

        public ReadOnlyDictionary<Guid, WeaponStatistics> DamageByWeapon;

        public override void Init()
        {
            DamageByWeapon = new ReadOnlyDictionary<Guid, WeaponStatistics>(damageByWeapon);
        }

        public bool IsContainWeapon(Guid weaponId)
        {
            return damageByWeapon.ContainsKey(weaponId);
        }

        public void AddWeapon(Guid weaponId, WeaponInfoRequestResult weaponInfo)
        {
            damageByWeapon.Add(weaponId, new WeaponStatistics(weaponInfo.WeaponName, weaponInfo.WeaponSprite));
        }

        public void UpdateDamage(Guid guid, float damage)
        {
            damageByWeapon[guid].WeaponDamage += damage;
        }

        public void ResetStatistics()
        {
            damageByWeapon.Clear();
        }
    }

    public class WeaponStatistics
    {
        public string WeaponName;
        public Sprite WeaponSprite;
        public float WeaponDamage;

        public WeaponStatistics(string weaponName, Sprite weaponSprite)
        {
            WeaponName = weaponName;
            WeaponSprite = weaponSprite;
        }
    }
}