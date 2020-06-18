using System;
using UnityEngine;
using System.Collections.Generic;

namespace RPG.Stats {
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        public float GetStat(Stat stat, CharacterClass characterClass, int level) {
            float[] levelTable = GetLevels(stat, characterClass);
            if(levelTable == null || level <= 0) return 0;
            if(levelTable.Length < level) return levelTable[levelTable.Length - 1];
            return levelTable[level - 1];
        }

        public float[] GetLevels(Stat stat, CharacterClass characterClass) {
            BuildLookup();
            if (!lookupTable.ContainsKey(characterClass)) return null;
            var firstLevelDic = lookupTable[characterClass];
            if (!firstLevelDic.ContainsKey(stat)) return null;
            return firstLevelDic[stat];
        }

        private Dictionary<Stat, float[]> GetStatLookupOfCharacterClass(CharacterClass characterClass){
            return lookupTable[characterClass];
        }

        private void BuildLookup() {
            if(lookupTable != null) return;
            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();
            foreach(ProgressionCharacterClass progressionClass in characterClasses) {
                Dictionary<Stat, float[]> firstLevelLookup = new Dictionary<Stat, float[]>();
                lookupTable[progressionClass.characterClass] = firstLevelLookup;
                foreach (ProgressionStat progressionStat in progressionClass.stats) {
                    firstLevelLookup[progressionStat.stat] = progressionStat.levels;
                }
            }
        }

        [Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;
        }

        [Serializable]
        class ProgressionStat {
            public Stat stat;
            public float[] levels;
        }
    }

    
}