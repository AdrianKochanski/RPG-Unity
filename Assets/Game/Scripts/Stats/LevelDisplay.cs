using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour {

        BaseStats baseStats;
        Text displaytext;

        private void Awake()
        {
            displaytext = GetComponent<Text>();
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        private void Update()
        {
            displaytext.text = String.Format("{0:0}", baseStats.GetLevel());
        }
    }
}