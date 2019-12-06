using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1,99)]
        [SerializeField] private int startingLevel = 1;
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private Progression progression = null;

        private int _currentLevel = 0;

        private void Start()
        {
            _currentLevel = CalculateLevel();
        }

        private void Update()
        {
            var newLevel = CalculateLevel();
            if (_currentLevel < newLevel)
            {
                _currentLevel = newLevel;
                print("Levelled up!");
            }
        }

        public float GetStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, CalculateLevel());
        }

        public int GetLevel()
        {
            return _currentLevel;
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
