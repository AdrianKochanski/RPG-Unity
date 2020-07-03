//using GameDevTV.Inventories;
using UnityEngine;
using UnityEngine.AI;
using System;
using GameDevTV.Inventories;
using System.Collections.Generic;

namespace RPG.Inventories
{
    [CreateAssetMenu(fileName = "DropLibrary", menuName = "RPG/Inventory/Drop Library", order = 0)]
    public class DropLibrary : ScriptableObject {
        [SerializeField] 
        Dropconfig[] potentialDrops;
        [Range(0,100)]
        [SerializeField] float[] dropChancePercentage;
        [SerializeField] int[] minDrops;
        [SerializeField] int[] maxDrops;

        [Serializable]
        class Dropconfig {
            public InventoryItem item;
            [Range(0, 100)]
            public float[] relativeChance;
            public int[] minNumber;
            public int[] maxNumber;
            public int GetRandomNumber(int level) {
                if(!item.IsStackable()) {
                    return 1;
                }
                int max = GetByLevel(maxNumber, level);
                int min = GetByLevel(minNumber, level);
                return UnityEngine.Random.Range(min, max +1);
            }
        }

        public struct Dropped {
            public InventoryItem item;
            public int number;
        }

        public IEnumerable<Dropped> GetRandomDrops(int level) {
            if(!ShouldRandomDrop(level)) {
                yield break;
            }
            for (int i = 0; i < GetRandomNumberOfDrops(level); i++)
            {
                yield return GetRandomDrop(level);
            }
        }

        bool ShouldRandomDrop(int level) {
            return UnityEngine.Random.Range(0, 100) < 
                GetByLevel(dropChancePercentage, level);
        }

        int GetRandomNumberOfDrops(int level) {
            return UnityEngine.Random.Range(
                GetByLevel(minDrops, level), 
                GetByLevel(maxDrops, level) + 1
            );
        }

        Dropped GetRandomDrop(int level) {
            Dropconfig itemSelected = SelectRandomDrop(level);
            //if(!itemSelected) return null;
            Dropped drop = new Dropped();
            drop.item = itemSelected.item;
            drop.number = itemSelected.GetRandomNumber(level);
            return drop;
        }

        Dropconfig SelectRandomDrop(int level) {
            float totalChance = GetTotalChance(level);
            float randomRoll = UnityEngine.Random.Range(0, totalChance);
            float chanceTotal = 0f;
            foreach (var drop in potentialDrops) {
                chanceTotal += GetByLevel(drop.relativeChance, level);
                if(chanceTotal > randomRoll) {
                    return drop;
                }
            }
            return null;
        }

        float GetTotalChance(int level) {
            float total = 0f;
            foreach (var drop in potentialDrops) {
                total += GetByLevel(drop.relativeChance, level);
            }
            return total;
        }

        static T GetByLevel<T>(T[] values, int level) {
            if (values.Length == 0) return default;
            if (level > values.Length) return values[values.Length - 1];
            if (level <= 0) return default;
            return values[level - 1];
        }
    }
}