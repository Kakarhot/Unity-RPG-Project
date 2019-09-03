using System;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        public Weapon weaponPrefab = null;
        [SerializeField] AnimatorOverrideController weaponOverride = null;
        [SerializeField] float weaponDamage = 10f;
        [SerializeField] float percentageDamage = 0;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";

        public float GetWeaponRange() {return weaponRange;}
        public float GetWeaponDamage() {return weaponDamage;}
        public float GetPercentageDamage() { return percentageDamage; }
    
        public Weapon SpawnWeapon(Transform leftHand, Transform rightHand, Animator animator)
        {
            DestroyOldWeapon(leftHand, rightHand);

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (weaponOverride)
            {
                animator.runtimeAnimatorController = weaponOverride;
            }
            else if (overrideController)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            if(weaponPrefab)
            {
                Transform handTransform = isRightHanded? rightHand : leftHand;
                Weapon weapon = Instantiate(weaponPrefab, handTransform);
                weapon.gameObject.name = weaponName;
                return weapon;
            }

            return null;
        }

        private void DestroyOldWeapon(Transform leftHand, Transform rightHand)
        {
            Transform oldWeapon = leftHand.Find(weaponName);
            if(!oldWeapon)
            {
                oldWeapon = rightHand.Find(weaponName);
            }
            if(oldWeapon == null) return;
            
            oldWeapon.name = "DESTROYING"; // ???
            Destroy(oldWeapon.gameObject);
        }

        public bool HasProjectile()
        {
            return projectile!=null;
        }

        public void SpawnProjectile(Transform target, GameObject instigator, Transform leftHand, Transform rightHand, float damage)
        {
            if(!projectile) return;
            Transform handTransform = isRightHanded ? rightHand : leftHand;
            Projectile projectileSpawned = Instantiate(projectile, handTransform.position, Quaternion.identity);
            projectileSpawned.SetTarget(target, instigator, damage);
        }
    }
}
