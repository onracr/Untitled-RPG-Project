using System;
using Core;
using Saving;
using Stats;
using UnityEngine;

namespace Resources
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private float regenerationPercentage = 70f;
        
        private float _health = -1f;

        // Cached Reference
        private Animator _animator;
        private ActionScheduler _actionScheduler;
        
        private bool _isDead = false;
        
        private void Start()
        {
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();

            GetComponent<BaseStats>().OnLevelUp += RegenerateHealth;
            
            if (_health < 0)
                _health = GetComponent<BaseStats>().GetStat(Stat.Health);
        }

         public void TakeDamage(GameObject instigator, float amount)
        {
            print(gameObject.name + " took damage:" + amount);
            
            _health = Mathf.Max(_health - amount, 0);

            if (_health == 0)
            {
                Die();
                AwardExperience(instigator);
            }
        }

         public float GetHealthPoints()
         {
             return _health;
         }

         public float GetMaxHealthPoints()
         {
             return GetComponent<BaseStats>().GetStat(Stat.Health);
         }
         

        public float GetPercentage()
        {
            return 100 * (_health / GetComponent<BaseStats>().GetStat(Stat.Health));
        }

        private void Die()
        {
            if (_isDead) return;
            if (_health <= 0)
            {
                GetComponent<Animator>().SetTrigger("die");
                GetComponent<ActionScheduler>().CancelCurrentAction();
                _isDead = true;
            }
        }

        private void AwardExperience(GameObject instigator)
        {
            var experience = instigator.GetComponent<Experience>();
            if (experience == null) return;

            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }
        
        private void RegenerateHealth()
        {
            var regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (regenerationPercentage / 100);
            _health = Mathf.Max(_health, regenHealthPoints);
        }

        public bool IsDead()
        {
            return _isDead;
        }

        public object CaptureState()
        {
            return _health;
        }

        public void RestoreState(object state)
        {
            _health = (float) state;
            
            if (_health <= 0)
                Die();
        }
    }
}