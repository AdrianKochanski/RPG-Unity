using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.DamageText
{
    public class DamageScriptSpawner : MonoBehaviour
    {
        [SerializeField] DamageText damageTexPrefab;

        public void Spawn(float damageAmount)
        {
            DamageText instance = Instantiate<DamageText>(damageTexPrefab, transform);
            // instance.GetComponentInChildren<Text>().text = damageAmount.ToString();
            instance.SetValue(damageAmount);
        }
    }
}
