using UnityEngine;
using GameDevTV.Inventories;
using RPG.Stats;
using System.Collections.Generic;
using System;

namespace RPG.Inventories
{
    public class StatsEquipment : Equipment, IModifierProvider {

        public IEnumerable<float> GetAdditiveModifiersFor(Stat stat) {
            foreach(var slot in GetAllPopulatedSlots()) {
                var item = GetItemInSlot(slot) as IModifierProvider;
                if(item == null) continue;
                foreach(var modifier in item.GetAdditiveModifiersFor(stat)) {
                    yield return modifier;
                }
            }
        }

        public IEnumerable<float> GetPercentageModifiersFor(Stat stat) {
            foreach (var slot in GetAllPopulatedSlots()) {
                var item = GetItemInSlot(slot) as IModifierProvider;
                if (item == null) continue;
                foreach (var modifier in item.GetPercentageModifiersFor(stat)) {
                    yield return modifier;
                }
            }
        }
    }
}