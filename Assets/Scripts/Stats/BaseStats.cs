using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameDevTV.Utils;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1,99)][SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpEffect;
        [SerializeField] bool bshouldUseModifiers = false;

        public event Action onLevelUp;

        LazyValue<int> currentLevel;
        Experience experience;

        private void Awake() 
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start() 
        {
            currentLevel.ForceInit();
        }

        // Called after Awake()
        private void OnEnable() 
        {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        // For good code practice
        private void OnDisable() 
        {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat));
        }

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, CalculateLevel());
        }

        private float GetAdditiveModifier(Stat stat)
        {
            if(!bshouldUseModifiers) return 0;

            float total = 0;
            foreach(IModifierProvider modifierProvider in GetComponents<IModifierProvider>())
            {
                foreach(float modifier in modifierProvider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (!bshouldUseModifiers) return 0;

            float total = 0;
            foreach (IModifierProvider modifierProvider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in modifierProvider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        public int GetLevel()
        {
            return currentLevel.value;
        }

        public int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();
            if(experience == null) return startingLevel;

            float currentXP = experience.GetExperience();
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            for(int level = 1; level < penultimateLevel + 1; level++)
            {
                float levelXP = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                if(levelXP > currentXP)
                {
                    return level;
                }
            }
            return penultimateLevel + 1;
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value)
            {
                currentLevel.value += 1;
                onLevelUp();
                GameObject vfx = Instantiate(levelUpEffect, transform);
            }
        }
    }
}


