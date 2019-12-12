using System;
using Core;
using GameDevTV.Utils;
using Saving;
using Stats;
using UnityEngine;
using UnityEngine.Events;

namespace Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private float regenerationPercentage = 70f;
        [SerializeField] private TakeDamageEvent takeDamage;
        
        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        { }

        // Cached Reference
        private Animator _animator;
        private ActionScheduler _actionScheduler;
        private LazyValue<float> _health;
        
        private bool _isDead = false;
        
        private void Awake()
        {
            _health = new LazyValue<float>(GetInitialHealth);

            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Start()
        {
            _health.ForceInit();
        }

        private void OnEnable()
        {
            GetComponent<BaseStats>().OnLevelUp += RegenerateHealth;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().OnLevelUp -= RegenerateHealth;
        }

         public void TakeDamage(GameObject instigator, float amount)
        {
            print(gameObject.name + " took damage:" + amount);
            
            _health.value = Mathf.Max(_health.value - amount, 0);

            if (_health.value == 0)
            {
                Die();
                AwardExperience(instigator);
            }
            else
            {
                takeDamage?.Invoke(amount);
            }
        }

         public float GetHealthPoints()
         {
             return _health.value;
         }

         public float GetMaxHealthPoints()
         {
             return GetComponent<BaseStats>().GetStat(Stat.Health);
         }
         

        public float GetPercentage()
        {
            return 100 * GetFraction();
        }

        public float GetFraction()
        {
            return _health.value / GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Die()
        {
            if (_isDead) return;
            if (_health.value <= 0)
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
            _health.value = Mathf.Max(_health.value, regenHealthPoints);
        }

        public bool IsDead()
        {
            return _isDead;
        }

        public object CaptureState()
        {
            return _health.value;
        }

        public void RestoreState(object state)
        {
            _health.value = (float)state;

            if (_health.value <= 0)
                Die();
        }
    }
}