using UnityEngine;

namespace SLE.Systems.Weapon.Data
{
    [CreateAssetMenu(menuName = "Weapon System/Weapon data")]
    public class WeaponInfo : ScriptableObject
    {
        [Header("Properties")]
        [Tooltip("Weapon's custom name")]
        public new string name;

        [Space]
        public float damage;
        public float fireRate;
        public float reloadTime;
        public float spread;
        public float range;

        [Header("Sound Effects References")]
        [Space]
        public AudioClip fireSound;
        [Space(2f)]
        public AudioClip reloadSound;
        [Space(2f)]
        public AudioClip noAmmoSound;
        [Space(10f)]
        public AudioClip[] extraSounds;

        [Header("Visual Effects References")]
        [Space]
        public GameObject MuzzleFlash;
        [Space(2f)]
        public GameObject BulletHit;


        [Header("References")]
        [Space]
        public Transform weaponPrefab;

        [Header("For weapons that fires projectile only.")]
        [Space]
        public Transform projectile = null;
    }
}