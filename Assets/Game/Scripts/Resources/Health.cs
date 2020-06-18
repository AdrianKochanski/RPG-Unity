using System;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Resources {
    public class Health : MonoBehaviour, ISaveable
    {
        [Range(0,100)]
        [SerializeField] float levelUpRegeneration = 70;
        float health = -1f;
        float maxHealth;
        bool isDead = false;

        private void Start() {
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
            GetMaxHealthStat();
            if(health < 0) {
                health = maxHealth;
            }
        }

        private void RegenerateHealth() {
            float healthPercentage = GetHealthPercentage() + levelUpRegeneration;
            // print("health percent: " + GetHealthPercentage() + " ,health regen: " + levelUpRegeneration);
            // print("current: " + health + " ,max: " + maxHealth);
            // print("Health percentage: " + healthPercentage);
            GetMaxHealthStat();
            float newHealth = maxHealth * healthPercentage / 100;
            if(newHealth > maxHealth) {
                health = maxHealth;
            } else {
                health = newHealth;
            }
        }

        private void GetMaxHealthStat() {
            maxHealth = GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetMaxHealth() {
            return maxHealth;
        }

        public float GetHealth() {
            return health;
        }

        public bool IsDead(){
            return isDead;
        }

        public void TakeDamage(GameObject instigator, float damage) {
            print(gameObject.name + " took damage: " + damage);
            if(damage > health) {
                health = 0;
            }
            else health -= damage;
            if(health == 0) {
                Die();
                AwardExperience(instigator);
            }
        }

        private void AwardExperience(GameObject instigator) {
            Experience experience = instigator.GetComponent<Experience>();
            if(experience == null) return;
            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        public float GetHealthPercentage(){
            return 100 * health / maxHealth;
        }

        private void Die(){
            if(isDead) return;
            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancellCurrentAction();
        }

        public object CaptureState()
        {
            return health;
        }

        public void RestoreState(object state)
        {
            health = (float)state;
            if (health == 0)
            {
                Die();
            }
        }
    }
}
