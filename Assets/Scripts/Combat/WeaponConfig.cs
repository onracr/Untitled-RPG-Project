using Core;
using Attributes;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Create New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] private Weapon equippedPrefab = null;
        [SerializeField] private AnimatorOverrideController animatorOverride = null;
        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private float damageAmount = 5f;
        [SerializeField] private float percentageBonus = 0f;
        [SerializeField] private bool isRightHanded = true;
        [SerializeField] private Projectile projectile = null;

        private const string WeaponName = "Weapon";
        
        public float GetRange()
        {
            return weaponRange;
        }

        public float GetDamage()
        {
            return damageAmount;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(
                projectile, 
                GetTransform(rightHand, leftHand).position,
                Quaternion.identity
                );
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);
            
            Weapon weapon = null;
            if (equippedPrefab != null)
            {
                var handTransform = GetTransform(rightHand, leftHand);
                weapon = Instantiate(equippedPrefab, handTransform);
                weapon.gameObject.name = WeaponName;
            }
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(WeaponName);

            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(WeaponName);
            }

            if (oldWeapon == null) return;

            oldWeapon.name = "Destroying";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (isRightHanded)
                handTransform = rightHand;
            else
                handTransform = leftHand;
            return handTransform;
        }
    }

}
