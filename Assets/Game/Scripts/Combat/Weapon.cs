using RPG.Resources;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] GameObject equippedPrefab = null;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] float percentageDamageBonus = 0f;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "weapon";

        public void Spawn(Transform leftHand, Transform rightHand, Animator animator){
            DestroyOldWeapon(leftHand, rightHand);
            if(equippedPrefab != null)
            {
                GameObject weapon = Instantiate(equippedPrefab, GetTransform(leftHand, rightHand));
                weapon.name = weaponName;
            }
            var overriderController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null){
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if (overriderController != null) {  
                animator.runtimeAnimatorController = overriderController.runtimeAnimatorController;
            }
        }

        private void DestroyOldWeapon(Transform leftHand, Transform rightHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if(oldWeapon == null){
                oldWeapon = leftHand.Find(weaponName);
            }
            if(oldWeapon == null) return;
            oldWeapon.name = "DESTROYED";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetTransform(Transform leftHand, Transform rightHand)
        {
            Transform handTransform;
            if (isRightHanded) handTransform = rightHand;
            else handTransform = leftHand;
            return handTransform;
        }

        public bool HasProjectile(){
            return projectile != null;
        }

        public void LaunchProjectile(
            Transform rightHand, 
            Transform leftHand, 
            Health target,
            Health owner,
            float calculatedDamage)
        {
            Projectile projectileInstance = 
            Instantiate(
                projectile, 
                GetTransform(leftHand, rightHand).position, 
                Quaternion.identity
            );
            projectileInstance.SetTarget(target, calculatedDamage, owner);
        }

        public float getWeaponRange(){
            return weaponRange;
        }

        public float getWeaponDamage(){
            return weaponDamage;
        }

        public float GetPercentageDamageBonus() {
            return percentageDamageBonus;
        }
    }
}