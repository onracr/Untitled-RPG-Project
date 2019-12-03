using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Create New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] private GameObject equippedPrefab = null;
        [SerializeField] private AnimatorOverrideController animatorOverride = null;
        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private float damageAmount = 5f;
        [SerializeField] private bool isRightHanded = true;

        public float GetRange()
        {
            return weaponRange;
        }

        public float GetDamage()
        {
            return damageAmount;
        }

        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            if (equippedPrefab != null)
            {
                Transform handTransform;
                if (isRightHanded)
                    handTransform = rightHand;
                else
                    handTransform = leftHand;
                
                Instantiate(equippedPrefab, handTransform);
            }

            if (animatorOverride != null)
                animator.runtimeAnimatorController = animatorOverride;
        }
    }

}
