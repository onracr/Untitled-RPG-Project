using System;
using UnityEngine;
using UnityEngine.UI;

namespace Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        [SerializeField] private Text levelText;

        private BaseStats _levelToDisplay;

        private void Awake()
        {
            _levelToDisplay = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        private void Update()
        {
            levelText.text = $"{_levelToDisplay.CalculateLevel()}";
        }
    }
}