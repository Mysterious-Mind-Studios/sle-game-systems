
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;


namespace SLE.Systems.Weapon.Jobs
{
    using SLE.Systems.Weapon.Data;

    [BurstCompile]
    unsafe struct ProcessWeaponJob : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction]
        public WeaponData* weaponData;
        [NativeDisableUnsafePtrRestriction]
        public Ammo*       weaponAmmo;

        [ReadOnly]
        public float time;

        public void Execute(int index)
        {
            ref WeaponData weapon = ref weaponData[index];
            ref Ammo       ammo   = ref weaponAmmo[index];

            switch(weapon.state)
            {
                case WeaponState.Shooting:
                    {
                        if(ammo.infinity)
                        {
                            if (time >= weapon.nextFireTime)
                            {
                                weapon.nextFireTime = time + 1.0f / weapon.fireRate;
                                weapon.hasFired = true;
                            }
                            else
                                weapon.hasFired = false;
                        }

                        if (ammo.amount > 0)
                        {
                            if (time >= weapon.nextFireTime)
                            {
                                weapon.nextFireTime = time + 1.0f / weapon.fireRate;

                                ammo.RemoveAmount(1, Source.Ammo);
                                
                                weapon.hasFired = true;
                            }
                            else
                                weapon.hasFired = false;
                        }

                        weapon.state = WeaponState.Ready;
                    }
                    break;

                case WeaponState.Reloading:
                    {
                        if (ammo.magazineAmmo > 0)
                        {
                            float elapsedTime = time - weapon.lastReloadTime;

                            if (elapsedTime < weapon.reloadTime)
                                return;

                            weapon.lastReloadTime = time;

                            int reloadAmount = ammo.ammoCapacity - ammo.amount;
                            ammo.AddAmount(reloadAmount, Source.Ammo);
                            ammo.RemoveAmount(reloadAmount, Source.Magazine);
                        }

                        weapon.state = WeaponState.Ready;
                    }
                    break;

                default:
                    return;
            }
        }
    }
}