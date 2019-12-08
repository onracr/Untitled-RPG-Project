using Combat;
using Core;
using Movement;
using Resources;
using UnityEngine;

namespace Control
{
    public class PlayerController : MonoBehaviour 
    {
        // References
        [SerializeField] private new Camera camera = null;
        
        // Cached Reference
        private Mover _mover;
        private Fighter _fighter;
        private Health _health;
        

        private void Awake() 
        {
            _mover = GetComponent<Mover>();
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
        }

        private void Update()
        {
            if (_health.IsDead()) return;
            if (CombatInteraction()) return;
            if (MovementInteraction()) return;
        }

        private bool CombatInteraction()
        {
            var hits = Physics.RaycastAll(GetMouseRay());

            foreach (var hit in hits)
            {
                var target = hit.transform.GetComponent<CombatTarget>();
                if (target == null) continue;

                if (!_fighter.CanAttack(target.gameObject)) continue;

                if (Input.GetMouseButtonDown(0))
                {
                    _fighter.AttackTo(target.gameObject);
                }
                return true;
            }
            return false;
        }

        private bool MovementInteraction()
        {
            if (Physics.Raycast(GetMouseRay(), out var hit)) {
                if (Input.GetMouseButton(0)) {
                    _mover.StartMoveAction(hit.point, 1f);
                }
                return true;
            }
            return false;
        }

        private Ray GetMouseRay()
        {
            return camera.ScreenPointToRay(Input.mousePosition);
        }
    }
}