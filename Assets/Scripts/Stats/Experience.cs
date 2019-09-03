using RPG.Saving;
using UnityEngine;
using System;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoints = 0;

        public event Action onExperienceGained;

        public float GetExperience() { return experiencePoints; }

        public void GainExperience(float experience)
        {
            this.experiencePoints += experience;
            onExperienceGained();
        }

        public object CaptureState()
        {
            return experiencePoints;
        }

        public void RestoreState(object state)
        {
            this.experiencePoints = (float)state;
        }
    }
}
