using System;
using Saving;
using UnityEngine;

namespace Core
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private float health = 100f;

        // Cached Reference
        private Animator _animator;
        private ActionScheduler _actionScheduler;
        
        private bool _isDead = false;
        
        private void Start()
        {
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();
        }

        public void TakeDamage(float amount)
        {
            if (health <= 0) return;
            
            health -= amount;
            Die();            
            // health = Mathf.Max(health - amount, 0);  does the same thing
        }

        private void Die()
        {
            if (_isDead) return;
            if (health == 0)
            {
                GetComponent<Animator>().SetTrigger("die");
                GetComponent<ActionScheduler>().CancelCurrentAction();
                _isDead = true;
            }
        }
        

        public bool IsDead()
        {
            return _isDead;
        }

        public object CaptureState()
        {
            return health;
        }

        public void RestoreState(object state)
        {
            health = (float) state;
            
            if (health <= 0)
                Die();
        }
    }
}