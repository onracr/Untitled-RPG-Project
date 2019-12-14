using Combat;
using Core;
using GameDevTV.Utils;
using Movement;
using Attributes;
using UnityEngine;
using System;

namespace Control
{
    public class AiController : MonoBehaviour
    {
        [SerializeField] private PatrolPath patrolPath = null;
        [SerializeField] private float chaseDistance = 5f;
        [SerializeField] private float shoutDistance = 5f;
        [SerializeField] private float wayPointTolerance = 1f;
        [SerializeField] private float wayPointDwellTime = 2f;
        [Range(0,1)]
        [SerializeField] private float patrolSpeedFraction = 0.2f;
        
        // Cached Reference
        private GameObject _player;
        private Fighter _fighter;
        private Health _health;
        private Mover _mover;
        private ActionScheduler _actionScheduler;

        private const float SuspicionTime = 3f;
        private const float AggroCooldownTime = 2f;
        private int _currentWayPointIndex = 0;
        private float _timeSinceArrivedAtWayPoint = Mathf.Infinity;
        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private float _timeSinceAggrevated = Mathf.Infinity;
        private LazyValue<Vector3> _guardingPosition;

        private void Awake()
        {
            _player = GameObject.FindWithTag("Player");
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
            _mover = GetComponent<Mover>();
            _actionScheduler = GetComponent<ActionScheduler>();

            _guardingPosition = new LazyValue<Vector3>(GetGuardingPosition);
        }

        private Vector3 GetGuardingPosition()
        {
            return transform.position;
        }

        private void Start()
        {
            _guardingPosition.ForceInit();
        } 

        private void Update()
        {
            if (_health.IsDead()) return;
            
            ChasingBehaviour();
            UpdateTimers();
        }

        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceArrivedAtWayPoint += Time.deltaTime;
            _timeSinceAggrevated += Time.deltaTime;
        }

        private void ChasingBehaviour()
        {
            if (IsAggrevated() && _fighter.CanAttack(_player)) {
                AttackBehaviour();
            }
            else if (_timeSinceLastSawPlayer < SuspicionTime) {
                SuspicionBehaviour();
            }
            else {
                PatrolBehaviour();
            }
        }

        private bool IsAggrevated()
        {
            return (DistanceToPlayer() < chaseDistance || _timeSinceAggrevated < AggroCooldownTime);
        }

        public void Aggrevate()
        {
            _timeSinceAggrevated = 0;
        }

        private void AttackBehaviour()
        {
            _fighter.AttackTo(_player);
            _timeSinceLastSawPlayer = 0;

            AggrevateNearByEnemies();
        }

        private void AggrevateNearByEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0f);

            foreach (var hit in hits)
            {
                var aiController = hit.transform.GetComponent<AiController>();
                if (aiController == null) continue;
                
                aiController.Aggrevate();
            }
        }

        private void SuspicionBehaviour()
        {
            _actionScheduler.CancelCurrentAction();
        }

        private void PatrolBehaviour()
        {
            var nextPosition = _guardingPosition.value;
            if (patrolPath != null)
            {
                if (IsAtWayPoint())
                { 
                    _timeSinceArrivedAtWayPoint = 0;
                    CycleWayPoint();
                }
                nextPosition = GetCurrentWayPoint();
            }

            if (_timeSinceArrivedAtWayPoint > wayPointDwellTime)
            {
                _mover.StartMoveAction(nextPosition, patrolSpeedFraction); // starting movement action, cancels fighting action
            }
        }

        private Vector3 GetCurrentWayPoint()
        {
            return patrolPath.GetWayPoint(_currentWayPointIndex);
        }

        private void CycleWayPoint()
        {
            _currentWayPointIndex = patrolPath.GetNextIndex(_currentWayPointIndex);
        }

        private bool IsAtWayPoint()
        {
            var distanceToWayPoint = Vector3.Distance(transform.position, GetCurrentWayPoint());
            return distanceToWayPoint < wayPointTolerance;
        }

        private float DistanceToPlayer()
        {
            return Vector3.Distance(_player.transform.position, transform.position);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, chaseDistance);  
        }
    }

}
