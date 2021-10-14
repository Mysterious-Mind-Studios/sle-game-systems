using Unity.Mathematics;

namespace SLE.Systems.Weapon.Data
{
    public struct WeaponData
    {
        public WeaponData(in Weapon weapon)
        {
            range      = weapon._weaponInfo.range;
            fireRate   = weapon._weaponInfo.fireRate;
            reloadTime = weapon._weaponInfo.reloadTime;

            state = weapon.enabled ? WeaponState.Ready : WeaponState.Inactive;

            nextFireTime   = 0f;
            lastReloadTime = 0f;

            firePoint = weapon._firePoint ?
                            new float3x2(weapon._firePoint.position, weapon._firePoint.forward) :
                            new float3x2(weapon.transform.position, weapon.transform.forward);

            hasFired    = false;
        }
        public WeaponData(in WeaponData other)
        {
            range          = other.range;
            fireRate       = other.fireRate;
            reloadTime     = other.reloadTime;
            state          = other.state;
            nextFireTime   = other.nextFireTime;
            lastReloadTime = other.lastReloadTime;
            firePoint      = other.firePoint;
            hasFired       = other.hasFired;
        }

        public readonly float range;
        public readonly float fireRate;
        public readonly float reloadTime;

        public WeaponState state;
        public float nextFireTime;
        public float lastReloadTime;
        public bool  hasFired;

        /// <summary>
        /// c0 - The origin vector3 of the fire point.<br/>
        /// c1 - The direction vector3 of the fire point.
        /// </summary>
        public float3x2 firePoint;
    }
}