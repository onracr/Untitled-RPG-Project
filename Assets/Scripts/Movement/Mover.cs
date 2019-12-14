using Core;
using Attributes;
using Saving;
using UnityEngine;
using UnityEngine.AI;

namespace Movement
{
   public class Mover : MonoBehaviour, IAction, ISaveable
   {
      [SerializeField] private float maxSpeed = 5f;
      [SerializeField] private float maxNavPathLength = 40f;

      // Cached Reference
      private NavMeshAgent _navMeshAgent;
      private Animator _animator;
      private ActionScheduler _actionScheduler;
      private Health _health;

      private void Awake()
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

      public bool CanMoveTo(Vector3 destination)
      {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(
                transform.position,
                destination,
                NavMesh.AllAreas,
                path
                );

            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > maxNavPathLength) return false;

            return true;
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

        private float GetPathLength(NavMeshPath path)
        {
            float total = 0f;
            if (path.corners.Length < 2) return total;
            for (int i = 0; i < path.corners.Length - 1; i++) {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return total;
        }

      public object CaptureState()
      {
         return new SerializableVector3(transform.position);
      }

      public void RestoreState(object state)
      {
         var position = (SerializableVector3) state;
         _navMeshAgent.enabled = false;
         transform.position = position.ToVector();
         _navMeshAgent.enabled = true;
         _actionScheduler.CancelCurrentAction();
      }
   }
}
