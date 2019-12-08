using GameDevTV.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
        [SerializeField] private int startingLevel = 1;
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private Progression progression = null;
        [SerializeField] private GameObject levelUpParticleEffect = null;
        [SerializeField] private bool shouldUseModifiers = false;

        public event Action OnLevelUp;

        private Experience experience;
        private LazyValue<int> _currentLevel;

        private void Awake()
        {
            experience = GetComponent<Experience>();
            _currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            _currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if (experience != null)
            {
                experience.OnExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (experience != null)
            {
                experience.OnExperienceGained -= UpdateLevel;
            }
        }

        private void UpdateLevel()
        {
            var newLevel = CalculateLevel();
            if (_currentLevel.value < newLevel)
            {
                _currentLevel.value = newLevel;
                LevelUpEffect();
                OnLevelUp?.Invoke();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat)/100);
        }

        public int GetLevel()
        {
            return _currentLevel.value;
        }

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        private float GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;

            float total = 0;
            foreach (var provider in GetComponents<IModifierProvider>())
            {
                foreach (var modifier in provider.GetAdditiveModifiers(stat))
                    
                {
                    total += modifier;
                }
            }

            return total;
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;

            float total = 0;
            foreach (var provider in GetComponents<IModifierProvider>())
            {
                foreach (var modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        public int CalculateLevel()
        {
            var experience = GetComponent<Experience>();

            if (experience == null) return startingLevel;
            
            float currentXp = experience.GetExpPoints();
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);

            for (var level = 1; level <= penultimateLevel; level++)
            {
                float xpToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                if (xpToLevelUp > currentXp)
                    return level;
            }

            return penultimateLevel + 1;

        }
    }
}
