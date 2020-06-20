using System;
using RPG.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes {
    public class EnemyHealthDisplay : MonoBehaviour {
        
        Text displaytext;
        Fighter player;
        Health enemy;

        private void Awake() {
            player = GameObject.FindWithTag("Player").GetComponent<Fighter>();
            displaytext = GetComponent<Text>();
        }

        private void Update() {
            enemy = player.GetTarget();
            if(enemy == null) {
                displaytext.text = "x/x";
                return;
            }
            if (enemy.IsDead()) {
                displaytext.text = "Target died";
                return;
            }
            displaytext.text = String.Format("{0:0}/{1:0}", enemy.GetHealth(), enemy.GetMaxHealth());
        }
    }
}