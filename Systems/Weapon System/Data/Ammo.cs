
using System;

using Unity.Mathematics;


namespace SLE.Systems.Weapon.Data
{
    [Serializable]
    public struct Ammo
    {
        public Ammo(in AmmoInfo ammoData)
        {
            infinity = false;

            amount       = 0;
            magazineAmmo = 0;

            type             = ammoData.type;
            ammoCapacity     = ammoData.ammoCapacity;
            magazineCapacity = ammoData.magazineCapacity;
        }
        public Ammo(in Ammo other)
        {
            infinity = other.infinity;
            
            amount       = 0;
            magazineAmmo = 0;
            
            type             = other.type;
            ammoCapacity     = other.ammoCapacity;
            magazineCapacity = other.magazineCapacity;

            AddAmount(other.amount, Source.Ammo);
            AddAmount(other.magazineAmmo, Source.Magazine);
        }

        public bool infinity;

        public int amount;
        public int magazineAmmo;

        public readonly int type;
        public readonly int ammoCapacity;
        public readonly int magazineCapacity;

        public void AddAmount(int amount, Source source)
        {
            switch (source)
            {
                case Source.Ammo:
                    {
                        this.amount = math.min(ammoCapacity, this.amount + amount);
                        break;
                    }

                case Source.Magazine:
                    {
                        magazineAmmo = math.min(magazineCapacity, magazineAmmo + amount);
                        break;
                    }
            }
        }
        public void RemoveAmount(int amount, Source source)
        {
            switch (source)
            {
                case Source.Ammo:
                    {
                        this.amount = math.max(0, this.amount - amount);
                        break;
                    }

                case Source.Magazine:
                    {
                        magazineAmmo = math.max(0, magazineAmmo - amount);
                        break;
                    }
            }
        }
    }
}