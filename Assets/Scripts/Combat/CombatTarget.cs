using Control;
using Resources;
using UnityEngine;

namespace Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {

        public bool HandleRaycast(PlayerController callingController)
        {
                if (!callingController.GetComponent<Fighter>().CanAttack(gameObject))
                    return false;

                if (Input.GetMouseButtonDown(0))
                {
                    callingController.GetComponent<Fighter>().AttackTo(gameObject);
                }
                return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }
    }
}