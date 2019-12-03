using Core;
using UnityEngine;
using Movement;

namespace Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] private float timeBetweenAttacks = 1f;
        [SerializeField] private Transform rightHandTransform = null;
        [SerializeField] private Transform leftHandTransform = null;
        [SerializeField] private Weapon defaultWeapon;
        
        private float _timeSinceLastAttack = Mathf.Infinity; // to start attacking at the start of the game
        
        private Health _target;
        private Mover _mover;
        private ActionScheduler _actionScheduler;
        private Animator _animator;
        private Weapon _currentWeapon = null;

        private void Start()
        {
            _mover = GetComponent<Mover>();
            _actionScheduler = GetComponent<ActionScheduler>();
            _animator = GetComponent<Animator>();

            EquipWeapon(defaultWeapon);
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
            _currentWeapon = weapon;
            weapon.Spawn(rightHandTransform, leftHandTransform, _animator);
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
            return Vector3.Distance(transform.position, _target.transform.position) <= _currentWeapon.GetRange();
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

        // Animation Event
        public void Hit()
        {
            if (_target == null) return;
            _target.TakeDamage(_currentWeapon.GetDamage());   
        }
        
    }
}