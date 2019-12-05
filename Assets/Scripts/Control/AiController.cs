using System;
using System.Collections;
using Combat;
using Core;
using Movement;
using Resources;
using UnityEngine;

namespace Control
{
    public class AiController : MonoBehaviour
    {
        [SerializeField] private PatrolPath patrolPath = null;
        [SerializeField] private float chaseDistance = 5f;
        [SerializeField] private float timeSinceLastSawPlayer = Mathf.Infinity;
        [SerializeField] private float timeSinceArrivedAtWayPoint = Mathf.Infinity;
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
        private int _currentWayPointIndex = 0;
        private Vector3 _guardingPosition;

        private void Start()
        {
            _guardingPosition = transform.position;
            _player = GameObject.FindWithTag("Player");
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
            _mover = GetComponent<Mover>();
            _actionScheduler = GetComponent<ActionScheduler>();
        }

        private void Update()
        {
            if (_health.IsDead()) return;
            
            ChasingBehaviour();
            UpdateTimers();
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWayPoint += Time.deltaTime;
        }

        private void ChasingBehaviour()
        {
            if (DistanceToPlayer() < chaseDistance && _fighter.CanAttack(_player))
            {
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < SuspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            _fighter.AttackTo(_player);
            timeSinceLastSawPlayer = 0;
            print(timeSinceLastSawPlayer);
        }

        private void SuspicionBehaviour()
        {
            _actionScheduler.CancelCurrentAction();
        }

        private void PatrolBehaviour()
        {
            var nextPosition = _guardingPosition;
            if (patrolPath != null)
            {
                if (IsAtWayPoint())
                { 
                    timeSinceArrivedAtWayPoint = 0;
                    CycleWayPoint();
                }
                nextPosition = GetCurrentWayPoint();
            }

            if (timeSinceArrivedAtWayPoint > wayPointDwellTime)
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
