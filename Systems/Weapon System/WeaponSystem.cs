

using System;
using System.Collections.Generic;

using UnityEngine;

using Unity.Jobs;


namespace SLE.Systems.Weapon
{
    using SLE.Systems.Weapon.Data;
    using SLE.Systems.Weapon.Jobs;

    public unsafe class WeaponSystem : SystemBase
    {
        static WeaponSystem _instance = null;
        public static WeaponSystem current => _instance;

        public WeaponSystem()
        {
            if (_instance)
                _instance.Dispose();

            _instance = this;

            activeWeapons = new HashSet<Weapon>(GameObject.FindObjectsOfType<Weapon>(false));
            locked = false;

            int length = activeWeapons.Count;

            Weapon.OnComponentCreate  += OnWeaponCreatedUpdateCache;
            Weapon.OnComponentDestroy += OnWeaponDestroyedUpdateCache;
            Weapon.OnComponentEnable  += OnWeaponEnableUpdateState;
            Weapon.OnComponentDisable += OnWeaponDisableUpdateState;
            Weapon.OnWeaponFire       += OnWeaponFiredUpdateState;
            Weapon.OnWeaponReload     += OnWeaponReloadedUpdateState;

            _cacheWeapons = new Weapon[length];
            activeWeapons.CopyTo(_cacheWeapons);

            _cacheWeaponData = new WeaponData[length];
            _cacheWeaponAmmo = new Ammo[length];

            int i;
            for (i = 0; i < length; i++)
            {
                Weapon weapon = _cacheWeapons[i];

                weapon._id = i;

                _cacheWeaponData[i] = new WeaponData(weapon);

                _cacheWeaponAmmo[i] = new Ammo(in weapon._ammoInfo);
                _cacheWeaponAmmo[i].infinity = weapon._ammo.infinity;
                _cacheWeaponAmmo[i].AddAmount(weapon._ammo.amount, Source.Ammo);
                _cacheWeaponAmmo[i].AddAmount(weapon._ammo.magazineAmmo, Source.Magazine);
            }

            locked = length == 0;
        }


        HashSet<Weapon> activeWeapons;
        bool            locked;
        bool            shouldRunUpdate;

        // --- Cache data. --- //

        Weapon[]     _cacheWeapons;
        WeaponData[] _cacheWeaponData;
        Ammo[]       _cacheWeaponAmmo;

        // --- Cache data. --- //

        private void OnWeaponCreatedUpdateCache(Weapon weapon)
        {
            locked = true;

            if (activeWeapons.Add(weapon))
            {
                int length = activeWeapons.Count;

                Array.Resize(ref _cacheWeapons, length);
                Array.Resize(ref _cacheWeaponData, length);
                Array.Resize(ref _cacheWeaponAmmo, length);

                activeWeapons.CopyTo(_cacheWeapons);

                int i;
                for (i = 0; i < length; i++)
                {
                    Weapon _weapon = _cacheWeapons[i];

                    _weapon._id = i;

                    _cacheWeaponData[i] = new WeaponData(_weapon);
                    _cacheWeaponAmmo[i] = new Ammo(in _weapon._ammoInfo);
                    _cacheWeaponAmmo[i].AddAmount(_weapon._ammo.amount, Source.Ammo);
                    _cacheWeaponAmmo[i].AddAmount(_weapon._ammo.magazineAmmo, Source.Magazine);
                }
            }

            locked = false;
        }
        private void OnWeaponDestroyedUpdateCache(Weapon weapon)
        {
            locked = true;

            if (activeWeapons.Remove(weapon))
            {
                int length = activeWeapons.Count;

                Array.Resize(ref _cacheWeapons, length);
                Array.Resize(ref _cacheWeaponData, length);
                Array.Resize(ref _cacheWeaponAmmo, length);

                activeWeapons.CopyTo(_cacheWeapons);

                int i;
                for (i = 0; i < length; i++)
                {
                    Weapon _weapon = _cacheWeapons[i];

                    _weapon._id = i;

                    _cacheWeaponData[i] = new WeaponData(_weapon);

                    _cacheWeaponAmmo[i] = new Ammo(in _weapon._ammoInfo);

                    ref Ammo ammo = ref _cacheWeaponAmmo[i];

                    ammo.infinity = _weapon._ammo.infinity;
                    ammo.AddAmount(_weapon._ammo.amount, Source.Ammo);
                    ammo.AddAmount(_weapon._ammo.magazineAmmo, Source.Magazine);
                }
            }

            locked = activeWeapons.Count == 0;
        }
        private void OnWeaponEnableUpdateState(Weapon weapon)
        {
            locked = true;

            int index = weapon._id;
            ref WeaponData data = ref _cacheWeaponData[index];

            data.state = WeaponState.Ready;

            locked = false;
        }
        private void OnWeaponDisableUpdateState(Weapon weapon)
        {
            locked = true;

            int index = weapon._id;
            ref WeaponData data = ref _cacheWeaponData[index];

            data.state = WeaponState.Inactive;

            locked = false;
        }
        private void OnWeaponFiredUpdateState(Weapon weapon)
        {
            locked = true;

            int index = weapon._id;
            ref WeaponData data = ref _cacheWeaponData[index];

            if (data.state == WeaponState.Ready)
            {
                data.state = WeaponState.Shooting;
                shouldRunUpdate = true;
            }

            locked = false;
        }
        private void OnWeaponReloadedUpdateState(Weapon weapon)
        {
            locked = true;

            int index = weapon._id;
            ref WeaponData data = ref _cacheWeaponData[index];

            if (data.state == WeaponState.Ready)
            {
                data.lastReloadTime = Time.time;
                data.state = WeaponState.Reloading;
                shouldRunUpdate = true;
            }

            locked = false;
        }

        protected override void Dispose(bool disposing)
        {
            _instance = null;
            activeWeapons = null;
            _cacheWeapons = null;
            _cacheWeaponData = null;
            _cacheWeaponAmmo = null;

            base.Dispose(disposing);
        }

        public override void OnStop()
        {
            Weapon.OnComponentCreate  -= OnWeaponCreatedUpdateCache;
            Weapon.OnComponentDestroy -= OnWeaponDestroyedUpdateCache;
            Weapon.OnWeaponFire       -= OnWeaponFiredUpdateState;
            Weapon.OnWeaponReload     -= OnWeaponReloadedUpdateState;
            Weapon.OnComponentEnable  -= OnWeaponEnableUpdateState;
            Weapon.OnComponentDisable -= OnWeaponDisableUpdateState;
        }

        public override JobHandle OnJobUpdate(float time, float deltaTime, ref JobHandle handle)
        {
            if (locked) return handle;
            if (!shouldRunUpdate) return handle;

            int length = _cacheWeapons.Length;

            int batchCount = GetBatchCount(length);

            fixed (WeaponData* weaponDataPtr = &_cacheWeaponData[0])
            {
                fixed (Ammo* weaponAmmoPtr = &_cacheWeaponAmmo[0])
                {
                    ProcessWeaponJob weaponJob = new ProcessWeaponJob
                    {
                        weaponData = weaponDataPtr,
                        weaponAmmo = weaponAmmoPtr,
                        time = time
                    };

                    JobHandle jobHandle = weaponJob.Schedule(length, batchCount);
                
                    return jobHandle;
                }
            }
        }
        public override unsafe void OnUpdate(float time, float deltaTime)
        {
            if (locked) return;
            if (!shouldRunUpdate) return;

            shouldRunUpdate = false;

            fixed (WeaponData* weaponDataPtr = &_cacheWeaponData[0])
            {
                fixed (Ammo* weaponAmmoPtr = &_cacheWeaponAmmo[0])
                {
                    int i;
                    int length = _cacheWeapons.Length;

                    for (i = 0; i < length; i++)
                    {
                        ref WeaponData data = ref _cacheWeaponData[i];

                        if (data.hasFired)
                            _cacheWeapons[i].SendMessage("OnFire");

                        if (data.state != WeaponState.Ready)
                            shouldRunUpdate = true;

                        _cacheWeapons[i]._ammo = weaponAmmoPtr[i];

                        data.hasFired = false;
                    }
                }
            }
        }
    }
}