
using System;

using UnityEngine;


namespace SLE.Systems.Weapon
{
    using SLE.Events;
    using SLE.Systems.Weapon.Data;

    public abstract class Weapon : SLEComponent<Weapon>
    {
        internal static Action<Weapon> OnSucessfullFire = wpn => wpn.OnFire();

        internal static event OnObjectChange<Weapon> OnWeaponFire;
        internal static event OnObjectChange<Weapon> OnWeaponReload;

#if UNDER_DEVELOPMENT
        [SerializeField]
        internal WeaponInfo _weaponInfo;

        [SerializeField]
        internal AmmoInfo _ammoInfo;

        [SerializeField]
        internal Ammo _ammo;

        [Space]
        [SerializeField]
        internal Transform _firePoint;
#else
        internal WeaponInfo _weaponInfo;
        internal AmmoInfo   _ammoInfo;
        internal Ammo       _ammo;
        internal Transform  _firePoint;
#endif

        public float damage        => _weaponInfo.damage;
        public float fireRate      => _weaponInfo.fireRate;
        public float reloadTime    => _weaponInfo.reloadTime;
        public float spread        => _weaponInfo.spread;
        public float range         => _weaponInfo.range;
        public Ammo  ammo          => _ammo;
        public Transform firePoint => _firePoint;
        public Transform projectilePrefab => _weaponInfo.projectile;

        protected abstract void OnFire();

        public void Fire()
        {
            OnWeaponFire(this);
        }
        public void Reload()
        {
            OnWeaponReload(this);
        }
        public override string ToString()
        {
            return _weaponInfo.name;
        }
    }
}