using Core;
using UnityEngine;
using Movement;
using Attributes;
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
        [SerializeField] private WeaponConfig defaultWeapon = null;
        [SerializeField] private string defaultWeaponName = "Unarmed";
        
        private float _timeSinceLastAttack = Mathf.Infinity; // to start attacking at the start of the game
        
        private Health _target;
        private Mover _mover;
        private ActionScheduler _actionScheduler;
        private Animator _animator;
        WeaponConfig _currentWeaponConfig;
        private LazyValue<Weapon> _currentWeapon;

        private void Awake()
        {
            _currentWeaponConfig = defaultWeapon;
            _currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            _animator = GetComponent<Animator>();
            _mover = GetComponent<Mover>();
            _actionScheduler = GetComponent<ActionScheduler>();
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
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
            
            if (!IsInRange(_target.transform))
            {
                _mover.MoveTo(_target.transform.position, 1f);
            }
            else
            {
                _mover.Cancel();
                AttackBehaviour();
            }
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            var animator = GetComponent<Animator>();
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            _currentWeaponConfig = weapon;
            _currentWeapon.value = AttachWeapon(weapon);
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


        private bool IsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) <= _currentWeaponConfig.GetRange();
        }

        public void AttackTo(GameObject target)
        {
            _actionScheduler.StartAction(this);
            _target = target.GetComponent<Health>();
        }

        public bool CanAttack(GameObject target)
        {
            if (target == null) return false;
            if (!_mover.CanMoveTo(target.transform.position) && !IsInRange(target.transform)) 
            {
                return false;
            }

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
                yield return _currentWeaponConfig.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _currentWeaponConfig.GetPercentageBonus();
            }
        }

        // Animation Event
        public void Hit()
        {
            if (_target == null) return;

            if (_currentWeapon.value != null)
            {
                _currentWeapon.value.OnHit();
            }
            
            var damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            if (_currentWeaponConfig.HasProjectile())
            {
                _currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, _target, gameObject, damage);
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
            return _currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string) state;
            WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }
    }
}