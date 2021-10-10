using UnityEngine;

namespace SLE.Systems.Weapon.Data
{
    [CreateAssetMenu(menuName = "Weapon System/Ammo data")]
    public sealed class AmmoInfo : ScriptableObject
    {
        public int  type;
        public int  ammoCapacity;
        public int  magazineCapacity;
    }
}