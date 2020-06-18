using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {

        Experience experience;
        Text displaytext;

        private void Awake()
        {
            displaytext = GetComponent<Text>();
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {
            displaytext.text = String.Format("{0:0}", experience.GetExperience());
        }
    }
}