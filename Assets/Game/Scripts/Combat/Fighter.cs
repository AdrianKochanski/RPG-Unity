using System.Collections.Generic;
using RPG.Core;
using RPG.Movement;
using RPG.Resources;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Weapon defaultWeapon = null;
        [SerializeField] string defaultWeaponName = "Unarmed";

        Health target;
        float timeSinceLastAttacks = Mathf.Infinity;
        Mover mover;
        Weapon currentWeapon = null;

        private void Start() {
            mover = GetComponent<Mover>();
            if(currentWeapon == null){
                EquipWeapon(defaultWeapon);
            }
        }

        private void Update()
        {
            timeSinceLastAttacks += Time.deltaTime;
            if(target ==null) return;
            if(target.IsDead()) return;
            if (target != null && !GetIsInRange())
            {
                mover.MoveTo(target.transform.position, 1f);
            }
            else
            {
                mover.Cancel();
                AttackBehaviour();
            }
        }

        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon = weapon;
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(leftHandTransform, rightHandTransform, animator);
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(target.transform.position, transform.position) < currentWeapon.getWeaponRange();
        }

        public void Cancel()
        {
            TriggerStopAttack();
            target = null;
            mover.Cancel();
        }

        private void TriggerStopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Attack(GameObject combatTarget){
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public Health GetTarget() {
            return target;
        }

        private void AttackBehaviour()
        {
            if(timeSinceLastAttacks > timeBetweenAttacks)
            {
                // this will trigger the Hit() event
                transform.LookAt(target.transform);
                TriggerStartAttack();
                timeSinceLastAttacks = 0f;
            }
        }

        private void TriggerStartAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        public IEnumerable<float> GetAdditiveModifiersFor(Stat stat)
        {
            if(stat == Stat.Damage) {
                yield return currentWeapon.getWeaponDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiersFor(Stat stat)
        {
            if (stat == Stat.Damage) {
                yield return currentWeapon.GetPercentageDamageBonus();
            }
        }

        //Animation Event on Hit the object
        void Hit(){
            if(target == null) return ;
            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            if(currentWeapon.HasProjectile()){
                currentWeapon.LaunchProjectile(rightHandTransform, leftHandTransform, target, GetComponent<Health>(), damage);
            } else {
                target.TakeDamage(gameObject, damage);
            }
            
        }

        void Shoot(){
            Hit();
        }

        public object CaptureState()
        {
            return currentWeapon.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            Weapon weapon = UnityEngine.Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
    }
}