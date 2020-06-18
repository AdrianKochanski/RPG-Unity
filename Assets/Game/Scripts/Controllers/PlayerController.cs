using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Resources;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health health;
        Fighter fighter;
        Mover mover;

        private void Awake() {
            health = GetComponent<Health>();
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
        }

        private void Start() { 
        }

        private void Update()
        {
            if(health.IsDead()) return;
            if(InteractWithCombat()) return;
            if(InteractWithMovement()) return;
        }

        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach(RaycastHit hit in hits){
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if(target == null) continue;
                GameObject targetGameObject = target.gameObject;
                if(!fighter.CanAttack(targetGameObject)) continue;
                if(Input.GetMouseButton(0)){
                    fighter.Attack(targetGameObject);
                }
                return true;
            }
            return false;
        }

        private bool InteractWithMovement()
        {
            Ray ray = GetMouseRay();
            RaycastHit hit;
            bool hasHit = Physics.Raycast(ray, out hit);
            if (hasHit)
            {
                if (Input.GetMouseButton(0))
                {
                    mover.StartMoveAction(hit.point, 1f);
                }
                return true;
            }
            return false;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}