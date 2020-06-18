using System;
using GameDevTV.Utils;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour {
        [Range(1,25)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpParticleEffect = null;
        [SerializeField] Boolean shouldUseModifiers = false;

        public event Action onLevelUp;

        LazyValue<int> currentLevel;

        Experience experience;
        
        private void Awake() {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void OnEnable() {
            if (experience != null) {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable() {
            if (experience != null) {
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        private void Start() {
            currentLevel.ForceInit();
        }

        private void UpdateLevel() {
            int newLevel = CalculateLevel();
            if(newLevel > currentLevel.value) {
                currentLevel.value = newLevel;
                LevelUpEffect();
                onLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }

        public float GetStat(Stat stat) {
            return (GetBaseStat(stat) + GetAdditiveModifiersFor(stat)) 
            * ( 1 + GetPercentageModifier(stat) / 100);
        }

        private float GetBaseStat(Stat stat) {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        public int GetLevel() {
            return currentLevel.value;
        }

        public float GetAdditiveModifiersFor(Stat stat) {
            if (!shouldUseModifiers) return 0f;
            float total = 0f;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>()) {
                foreach (float modifier in provider.GetAdditiveModifiersFor(stat)) {
                    total += modifier;
                }
            }
            return total;
        }

        private float GetPercentageModifier(Stat stat) {
            if (!shouldUseModifiers) return 0f;
            float total = 0f;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>()) {
                foreach (float modifier in provider.GetPercentageModifiersFor(stat)) {
                    total += modifier;
                }
            }
            return total;
        }

        private int CalculateLevel() {
            if(experience == null) return startingLevel;
            float currentXP = experience.GetExperience();
            float[] levels = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            if(levels == null) return startingLevel;
            int maxLevel = levels.Length + 1;
            float sumExperience = 0;
            for(int level = 1; level < maxLevel; level ++) {
                float XPToNextLevel =
                progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                sumExperience += XPToNextLevel;
                if(sumExperience > currentXP) {
                    if(level < startingLevel) {
                        if(startingLevel <= maxLevel) return startingLevel;
                        else return maxLevel;
                    }
                    else return level;
                }
            }
            return maxLevel;
        }
    }
}