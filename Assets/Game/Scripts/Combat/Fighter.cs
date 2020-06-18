using System.Collections.Generic;
using GameDevTV.Utils;
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
        Animator animator;
        Health healthComp;
        ActionScheduler scheduler;
        BaseStats baseStats;
        LazyValue<Weapon> currentWeapon;

        private void Awake() {
            mover = GetComponent<Mover>();
            animator = GetComponent<Animator>();
            healthComp = GetComponent<Health>();
            scheduler = GetComponent<ActionScheduler>();
            baseStats = GetComponent<BaseStats>();
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private Weapon SetupDefaultWeapon() {
            AttachWeapon(defaultWeapon);
            return defaultWeapon;
        }

        private void Start() {
            currentWeapon.ForceInit();
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
            currentWeapon.value = weapon;
            AttachWeapon(weapon);
        }

        private void AttachWeapon(Weapon weapon)
        {
            weapon.Spawn(leftHandTransform, rightHandTransform, animator);
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(target.transform.position, transform.position) < currentWeapon.value.getWeaponRange();
        }

        public void Cancel()
        {
            TriggerStopAttack();
            target = null;
            mover.Cancel();
        }

        private void TriggerStopAttack()
        {
            animator.ResetTrigger("attack");
            animator.SetTrigger("stopAttack");
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Attack(GameObject combatTarget){
            scheduler.StartAction(this);
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
            animator.ResetTrigger("stopAttack");
            animator.SetTrigger("attack");
        }

        public IEnumerable<float> GetAdditiveModifiersFor(Stat stat)
        {
            if(stat == Stat.Damage) {
                yield return currentWeapon.value.getWeaponDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiersFor(Stat stat)
        {
            if (stat == Stat.Damage) {
                yield return currentWeapon.value.GetPercentageDamageBonus();
            }
        }

        //Animation Event on Hit the object
        void Hit(){
            if(target == null) return ;
            float damage = baseStats.GetStat(Stat.Damage);
            if(currentWeapon.value.HasProjectile()){
                currentWeapon.value.LaunchProjectile(rightHandTransform, leftHandTransform, target, healthComp, damage);
            } else {
                target.TakeDamage(gameObject, damage);
            }
            
        }

        void Shoot(){
            Hit();
        }

        public object CaptureState()
        {
            return currentWeapon.value.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            Weapon weapon = UnityEngine.Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
    }
}