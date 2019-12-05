using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Resources
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private Text healthText;
        private Health _health;

        private void Awake()
        {
            _health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        private void Update()
        {
            healthText.text = $"{_health.GetPercentage():0}%";
            //healthText.text = String.Format("{0:0}%", _health.GetPercentage());
            //healthText.text = _health.GetPercentage().ToString("F2") + "%";
        }
        
    }
}