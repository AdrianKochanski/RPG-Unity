using RPG.Saving;
using UnityEngine;

namespace RPG.Stats
{    
    public class Experience : MonoBehaviour, ISaveable {
        [SerializeField] float experiencePoints = 0;

        public delegate void ExperienceGainedDelegate();
        public event ExperienceGainedDelegate onExperienceGained;
        // public event Action onExperienceGained;

        public object CaptureState()
        {
            return experiencePoints;
        }

        public void GainExperience(float experience) {
            experiencePoints += experience;
            onExperienceGained();
        }

        public float GetExperience() {
            return experiencePoints;
        }

        public void RestoreState(object state)
        {
            // print("restoring experience");
            experiencePoints = (float) state;
        }
    }
}