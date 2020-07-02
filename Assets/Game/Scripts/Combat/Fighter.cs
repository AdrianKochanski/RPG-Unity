using System.Collections.Generic;
using GameDevTV.Utils;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using GameDevTV.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;
        [SerializeField] string defaultWeaponName = "Unarmed";

        Health target;
        float timeSinceLastAttacks = Mathf.Infinity;
        private float chasingSpeed = 1f;
        Mover mover;
        Animator animator;
        Health healthComp;
        ActionScheduler scheduler;
        BaseStats baseStats;
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;

        private void Awake() {
            mover = GetComponent<Mover>();
            animator = GetComponent<Animator>();
            healthComp = GetComponent<Health>();
            scheduler = GetComponent<ActionScheduler>();
            baseStats = GetComponent<BaseStats>();
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private Weapon SetupDefaultWeapon() {
            return AttachWeapon(defaultWeapon);
        }

        private void Start() {
            currentWeapon.ForceInit();
        }

        private void Update() {
            timeSinceLastAttacks += Time.deltaTime;
            if(target == null) return;
            if(target.IsDead()) return;
            if (!GetIsInRange(target.transform))
            {
                mover.MoveTo(target.transform.position, chasingSpeed);
            }
            else
            {
                mover.Cancel();
                AttackBehaviour();
            }
        }

        public void EquipWeapon(WeaponConfig weapon) {
            currentWeaponConfig = weapon;
            currentWeapon.value =  AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon) {
            return weapon.Spawn(leftHandTransform, rightHandTransform, animator);
        }

        private bool GetIsInRange(Transform targetTransform) {
            return Vector3.Distance(transform.position, targetTransform.position) < currentWeaponConfig.getWeaponRange();
        }

        public void Cancel() {
            TriggerStopAttack();
            target = null;
            mover.Cancel();
        }

        private void TriggerStopAttack() {
            animator.ResetTrigger("attack");
            animator.SetTrigger("stopAttack");
        }

        public bool CanAttack(GameObject combatTarget) {
            if(combatTarget == null) return false;
            if(!mover.CanMoveTo(combatTarget.transform.position) && !GetIsInRange(combatTarget.transform)) return false;
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

        private void AttackBehaviour() {
            if(timeSinceLastAttacks > timeBetweenAttacks)
            {
                // this will trigger the Hit() event
                transform.LookAt(target.transform);
                TriggerStartAttack();
                timeSinceLastAttacks = 0f;
            }
        }

        private void TriggerStartAttack() {
            animator.ResetTrigger("stopAttack");
            animator.SetTrigger("attack");
        }

        public IEnumerable<float> GetAdditiveModifiersFor(Stat stat) {
            if(stat == Stat.Damage) {
                yield return currentWeaponConfig.getWeaponDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiersFor(Stat stat) {
            if (stat == Stat.Damage) {
                yield return currentWeaponConfig.GetPercentageDamageBonus();
            }
        }

        //Animation Event on Hit the object
        void Hit(){
            if(target == null) return ;
            float damage = baseStats.GetStat(Stat.Damage);
            if(currentWeapon.value != null) {
                currentWeapon.value.OnHit();
            }
            if(currentWeaponConfig.HasProjectile()){
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, healthComp, damage);
            } else {
                target.TakeDamage(gameObject, damage);
            }
        }

        void Shoot() {
            Hit();
        }

        public object CaptureState() {
            return currentWeaponConfig.name;
        }

        public void SetChasignSpped(float speed) {
            chasingSpeed = speed;
        }

        public void RestoreState(object state) {
            string weaponName = (string)state;
            WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }
    }
}