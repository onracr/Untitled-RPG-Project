using System;
using System.Runtime.Remoting.Messaging;
using Core;
using Saving;
using UnityEngine;
using UnityEngine.AI;

namespace Movement
{
   public class Mover : MonoBehaviour, IAction, ISaveable
   {
      [SerializeField] private float maxSpeed = 5f;

      // Cached Reference
      private NavMeshAgent _navMeshAgent;
      private Animator _animator;
      private ActionScheduler _actionScheduler;
      private Health _health;

      private void Start()
      {
         _navMeshAgent = GetComponent<NavMeshAgent>();
         _animator = GetComponent<Animator>();
         _actionScheduler = GetComponent<ActionScheduler>();
         _health = GetComponent<Health>();
      }

      private void Update()
      {
         _navMeshAgent.enabled = !_health.IsDead();
         UpdateAnimator();
      }

      public void StartMoveAction(Vector3 destination, float speedFraction)
      {
         _actionScheduler.StartAction(this);
         MoveTo(destination, speedFraction);
      }

      public void MoveTo(Vector3 destination, float speedFraction)
      {
         _navMeshAgent.destination = destination;
         _navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
         _navMeshAgent.isStopped = false;
      }

      public void Cancel()
      {
         _navMeshAgent.isStopped = true;
      }

      private void UpdateAnimator()
      {
         var velocity = _navMeshAgent.velocity;
         var localVelocity = transform.InverseTransformDirection(velocity);
         var speed = localVelocity.z;
         _animator.SetFloat("forwardSpeed", speed);
      }

      public object CaptureState()
      {
         return new SerializableVector3(transform.position);
      }

      public void RestoreState(object state)
      {
         var position = (SerializableVector3) state;
         GetComponent<NavMeshAgent>().enabled = false;
         transform.position = position.ToVector();
         GetComponent<NavMeshAgent>().enabled = true;
         GetComponent<ActionScheduler>().CancelCurrentAction();
      }
   }
}
