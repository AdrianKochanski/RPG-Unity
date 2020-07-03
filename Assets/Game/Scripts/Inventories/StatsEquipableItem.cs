using GameDevTV.Inventories;
using RPG.Stats;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = ("RPG/Inventory/Equipable Item"))]
    public class StatsEquipableItem : EquipableItem, IModifierProvider
    {
        [SerializeField]
        Modifier[] additiveModifiers;
        [SerializeField]
        Modifier[] percentageModifiers;

        [Serializable] 
        struct Modifier {
            public Stat stat;
            public float value;
        }

        public IEnumerable<float> GetAdditiveModifiersFor(Stat stat) {
            foreach(var modifier in  additiveModifiers) {
                if(modifier.stat == stat) {
                    yield return modifier.value;
                }
            }
        }

        public IEnumerable<float> GetPercentageModifiersFor(Stat stat) {
            foreach (var modifier in percentageModifiers) {
                if (modifier.stat == stat) {
                    yield return modifier.value;
                }
            }
        }
    }
}