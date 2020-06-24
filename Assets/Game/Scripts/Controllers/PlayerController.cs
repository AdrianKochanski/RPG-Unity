using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using RPG.Core;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health health;
        Fighter fighter;
        Mover mover;
        ActionScheduler scheduler;

        [Serializable]
        struct CursorMapping {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] float raycastRadius = 1f;

        private void Awake() {
            health = GetComponent<Health>();
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
            scheduler = GetComponent<ActionScheduler>();
        }

        private void Update() {
            if(InteractWithUI()) {
                SetCursor(CursorType.UI);
                return;
            }
            if(health.IsDead()) {
                SetCursor(CursorType.None);
                return;
            }
            if(InteractWithComponent()) return;
            if(InteractWithMovement()) return;
            SetCursor(CursorType.None);
        }

        private bool InteractWithComponent() {
            RaycastHit[] hits = GetSortedRaycastAll();
            foreach (RaycastHit hit in hits) {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables) {
                    if (raycastable.HandleRaycast(this)) {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        private static RaycastHit[] GetSortedRaycastAll() {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), 1f);
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++) {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }

        private bool InteractWithUI() {
            return EventSystem.current.IsPointerOverGameObject();
        }

        private void SetCursor(CursorType type) {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type) {
            foreach (CursorMapping mapping in cursorMappings) {
                if(mapping.type != type) continue;
                return mapping;
            }
            return cursorMappings[0];
        }

        private bool InteractWithMovement() {
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit) {
                if(!mover.CanMoveTo(target)) return false;
                if (Input.GetMouseButton(0)) {
                    mover.StartMoveAction(target, 1f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 result) {
            RaycastHit rayHit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out rayHit);
            result = new Vector3();
            if(!hasHit) return false;
            NavMeshHit hit;
            if(!NavMesh.SamplePosition(
                rayHit.point, out hit, maxNavMeshProjectionDistance, NavMesh.AllAreas)){
                return false;
            }
            result = hit.position;
            return true;
        }

        private static Ray GetMouseRay() {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        public void DisablePlayer() {
            scheduler.CancellCurrentAction();
            this.enabled = false;
        }

        public void EnablePlayer() {
            this.enabled = true;
        }
    }
}