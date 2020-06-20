using RPG.Control;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController controller)
        {
            if (!controller.GetComponent<Fighter>().CanAttack(gameObject)) return false;
            if (Input.GetMouseButton(0)) {
                controller.GetComponent<Fighter>().Attack(gameObject);
            }
            return true;
        }
    }
}