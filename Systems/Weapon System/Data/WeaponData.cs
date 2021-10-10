using Unity.Mathematics;

namespace SLE.Systems.Weapon.Data
{
    public struct WeaponData
    {
        public WeaponData(in Weapon weapon)
        {
            range      = weapon.weaponInfo.range;
            fireRate   = weapon.weaponInfo.fireRate;
            reloadTime = weapon.weaponInfo.reloadTime;

            state = weapon.enabled ? WeaponState.Ready : WeaponState.Inactive;

            nextFireTime   = 0f;
            lastReloadTime = 0f;

            firePoint = weapon.firePoint ?
                            new float3x2(weapon.firePoint.position, weapon.firePoint.forward) :
                            new float3x2(weapon.transform.position, weapon.transform.forward);

            targetLayer = weapon.targetLayer;
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
            targetLayer    = other.targetLayer;
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
        public int      targetLayer;
    }
}