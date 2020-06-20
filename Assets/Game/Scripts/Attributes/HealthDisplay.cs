using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour {
        
        Health health;
        Text displaytext;

        private void Awake() {
            displaytext = GetComponent<Text>();
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        private void Update() {
            displaytext.text = String.Format("{0:0}/{1:0}", health.GetHealth(), health.GetMaxHealth());
        }
    }
}