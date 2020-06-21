using System;
using GameDevTV.Utils;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes {
    public class Health : MonoBehaviour, ISaveable
    {
        [Range(0,100)]
        [SerializeField] float levelUpRegeneration = 70;
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UpdateBarEvent updateBar;
        [SerializeField] UnityEvent onDie;

        [Serializable]
        public class TakeDamageEvent : UnityEvent<float> {
        }

        [Serializable]
        public class UpdateBarEvent : UnityEvent<float>
        {
        }

        LazyValue<float> health;
        LazyValue<float> maxHealth;
        bool isDead = false;

        BaseStats baseStats;
        Animator animator;
        ActionScheduler scheduler;

        private void Awake() {
            baseStats = GetComponent<BaseStats>();
            animator = GetComponent<Animator>();
            scheduler = GetComponent<ActionScheduler>();
            health = new LazyValue<float>(GetMaxHealthStat);
            maxHealth = new LazyValue<float>(GetMaxHealthStat);
        }

        private void OnEnable() {
            baseStats.onLevelUp += RegenerateHealth;
        }

        private void OnDisable() {
            baseStats.onLevelUp -= RegenerateHealth;
        }

        private void Start() {
            health.ForceInit();
            maxHealth.ForceInit();
            updateBar.Invoke(GetFraction());
        }

        private void RegenerateHealth() {
            float healthPercentage = GetHealthPercentage() + levelUpRegeneration;
            // print("health percent: " + GetHealthPercentage() + " ,health regen: " + levelUpRegeneration);
            // print("current: " + health + " ,max: " + maxHealth);
            // print("Health percentage: " + healthPercentage);
            maxHealth.value = GetMaxHealthStat();
            float newHealth = maxHealth.value * healthPercentage / 100;
            if(newHealth > maxHealth.value) {
                health.value = maxHealth.value;
            } else {
                health.value = newHealth;
            }
        }

        private float GetMaxHealthStat() {
            return baseStats.GetStat(Stat.Health);
        }

        public float GetMaxHealth() {
            return maxHealth.value;
        }

        public float GetHealth() {
            return health.value;
        }

        public bool IsDead(){
            return isDead;
        }

        public void TakeDamage(GameObject instigator, float damage) {
            print(gameObject.name + " took damage: " + damage);
            if(damage > health.value) {
                health.value = 0;
            }
            else health.value -= damage;
            if(health.value == 0) {
                onDie.Invoke();
                Die();
                AwardExperience(instigator);
            } else {
                takeDamage.Invoke(damage);
            }
            updateBar.Invoke(GetFraction());
        }

        private void AwardExperience(GameObject instigator) {
            Experience experience = instigator.GetComponent<Experience>();
            if(experience == null) return;
            experience.GainExperience(baseStats.GetStat(Stat.ExperienceReward));
        }

        public void Heal(float amount) {
            health.value = Math.Min(maxHealth.value, health.value + amount);
        }

        public float GetHealthPercentage(){
            return 100 * GetFraction();
        }

        public float GetFraction() {
            return health.value / maxHealth.value;
        }

        private void Die(){
            if(isDead) return;
            isDead = true;
            animator.SetTrigger("die");
            scheduler.CancellCurrentAction();
        }

        public object CaptureState()
        {
            return health.value;
        }

        public void RestoreState(object state)
        {
            health.value = (float)state;
            if (health.value == 0)
            {
                Die();
            }
        }
    }
}
