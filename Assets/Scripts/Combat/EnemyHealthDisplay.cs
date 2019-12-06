using System;
using Combat;
using Resources;
using UnityEngine;
using UnityEngine.UI;

namespace Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        [SerializeField] private Text enemyHealthText = null;
        private Fighter _fighter;

        private void Awake()
        {
            _fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        private void Update()
        {
            if (_fighter.GetTarget() == null)
            {
                enemyHealthText.text = "N/A";
                return;
            }
            var health = _fighter.GetTarget();
            enemyHealthText.text = $"{health.GetHealthPoints():0}/{health.GetMaxHealthPoints():0}";
        }
    }
}