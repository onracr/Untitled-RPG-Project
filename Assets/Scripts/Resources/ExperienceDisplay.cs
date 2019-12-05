using System;
using UnityEngine;
using UnityEngine.UI;

namespace Resources
{
    public class ExperienceDisplay : MonoBehaviour
    {
        [SerializeField] private Text xpText;

        private Experience _experience;

        private void Awake()
        {
            _experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {
            xpText.text = $"{_experience.GetExpPoints():0}";
        }
    }
}