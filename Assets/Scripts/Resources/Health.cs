using System;
using Core;
using Saving;
using Stats;
using UnityEngine;

namespace Resources
{
    public class Health : MonoBehaviour, ISaveable
    {
        private float _health = -1f;

        // Cached Reference
        private Animator _animator;
        private ActionScheduler _actionScheduler;
        
        private bool _isDead = false;
        
        private void Start()
        {
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();

            if (_health < 0)
                _health = GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public void TakeDamage(GameObject instigator, float amount)
        {
            _health = Mathf.Max(_health - amount, 0);
            //health -= amount;

            if (_health == 0)
            {
                Die();
                AwardExperience(instigator);
            }
        }

        public float GetPercentage()
        {
            print(_health);
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