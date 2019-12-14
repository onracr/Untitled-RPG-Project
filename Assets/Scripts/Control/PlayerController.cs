using System;
using Combat;
using Movement;
using Attributes;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace Control
{
    public class PlayerController : MonoBehaviour 
    {
        // References, Config variables
        [SerializeField] private new Camera camera = null;
        [SerializeField] private CursorMapping[] cursorMappings = null;
        [SerializeField] private float maxNavMeshProjectionDistance = 1f;
        [SerializeField] private float raycastRadius = 1f;
        
        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

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
            if (InteractWithUI()) return;
            if (_health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }
            if (InteractWithComponent()) return;
            if (MovementInteraction()) return;

            SetCursor(CursorType.None);
        }

        private bool InteractWithComponent()
        {
            var hits = RaycastAllSorted();

            foreach (var hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (var raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                      
                }
            }
            return false;
        }

        private RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            float[] distances = new float[hits.Length];

            for (var i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);

            return hits;
        }

        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }
        private bool MovementInteraction()
        {
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {
                if (!_mover.CanMoveTo(target)) return false;
                
                if (Input.GetMouseButton(0))
                {
                    _mover.StartMoveAction(target, 1f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();

            RaycastHit hit;
            var hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit) return false;

            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(
                hit.point,
                out navMeshHit,
                maxNavMeshProjectionDistance,
                NavMesh.AllAreas
                );
            if (!hasCastToNavMesh) return false;

            target = navMeshHit.position;

            

            return true;
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (var mapping in cursorMappings)
            {
                if (mapping.type == type)
                    return mapping;
            }
            return cursorMappings[0];
        }

        private Ray GetMouseRay()
        {
            return camera.ScreenPointToRay(Input.mousePosition);
        }
    }
}