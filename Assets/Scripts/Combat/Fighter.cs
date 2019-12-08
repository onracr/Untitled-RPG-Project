using Core;
using UnityEngine;
using Movement;
using Resources;
using Saving;
using Stats;
using System.Collections.Generic;
using GameDevTV.Utils;
using System;

namespace Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        [SerializeField] private float timeBetweenAttacks = 1f;
        [SerializeField] private Transform rightHandTransform = null;
        [SerializeField] private Transform leftHandTransform = null;
        [SerializeField] private Weapon defaultWeapon = null;
        [SerializeField] private string defaultWeaponName = "Unarmed";
        
        private float _timeSinceLastAttack = Mathf.Infinity; // to start attacking at the start of the game
        
        private Health _target;
        private Mover _mover;
        private ActionScheduler _actionScheduler;
        private Animator _animator;
        private LazyValue<Weapon> _currentWeapon;

        private void Awake()
        {
            _currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);

            _animator = GetComponent<Animator>();
            _mover = GetComponent<Mover>();
            _actionScheduler = GetComponent<ActionScheduler>();
        }

        private Weapon SetupDefaultWeapon()
        {
            AttachWeapon(defaultWeapon);
            return defaultWeapon;
        }

        private void Start()
        {
            _currentWeapon.ForceInit();
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;

            if (_target == null) return;
            if (_target.IsDead()) return;
            
            if (!IsInRange())
            {
                _mover.MoveTo(_target.transform.position, 1f);
            }
            else
            {
                _mover.Cancel();
                AttackBehaviour();
            }
        }
        public void EquipWeapon(Weapon weapon)
        {
            _currentWeapon.value = weapon;
            AttachWeapon(weapon);
        }

        private void AttachWeapon(Weapon weapon)
        {
            var animator = GetComponent<Animator>();
            weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public Health GetTarget()
        {
            return _target;
        }

        public void AttackBehaviour()
        {
            transform.LookAt(_target.transform);
            
            if (_timeSinceLastAttack > timeBetweenAttacks)
            {
                // This will trigger Hit() event
                _animator.ResetTrigger("stopAttack");
                _animator.SetTrigger("attack");
                _timeSinceLastAttack = 0f;
            }
        }


        private bool IsInRange()
        {
            return Vector3.Distance(transform.position, _target.transform.position) <= _currentWeapon.value.GetRange();
        }

        public void AttackTo(GameObject target)
        {
            _actionScheduler.StartAction(this);
            _target = target.GetComponent<Health>();
        }

        public bool CanAttack(GameObject target)
        {
            if (target == null) return false;
            Health targetToTest = target.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Cancel()
        {
            if (_target != null)
            {
                _animator.ResetTrigger("attack");
                _animator.SetTrigger("stopAttack");
            }
            _target = null;
            _mover.Cancel();
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _currentWeapon.value.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _currentWeapon.value.GetPercentageBonus();
            }
        }

        // Animation Event
        public void Hit()
        {
            if (_target == null) return;

            var damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            if (_currentWeapon.value.HasProjectile())
            {
                _currentWeapon.value.LaunchProjectile(rightHandTransform, leftHandTransform, _target, gameObject, damage);
            }
            else
            {
                _target.TakeDamage(gameObject, damage);   
            }
        }

        private void Shoot()
        {
            Hit();
        }

        public object CaptureState()
        {
            return _currentWeapon.value.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string) state;
            Weapon weapon = UnityEngine.Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
    }
}