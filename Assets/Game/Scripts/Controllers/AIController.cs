using GameDevTV.Utils;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using UnityEngine;
using System;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float timeSuspicion = 3f;
        [SerializeField] float aggroCooldownTime = 5f;
        [SerializeField] float waypointDwelltime = 3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTollerance = 0.7f;
        [Range(0, 1)]
        [SerializeField] float patrolSpeedFraction = 0.4f;
        [Range(0, 1)]
        [SerializeField] float chasingSpeedFraction = 0.66f;
        [SerializeField] float shoutDistance = 5f;

        Fighter fighter;
        Health health;
        GameObject player;
        Mover mover;
        ActionScheduler scheduler;

        LazyValue<Vector3> guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeStayedBeyondWaypoint = Mathf.Infinity;
        float timeSinceAggrevated = Mathf.Infinity;
        int currentWaypointIndex = 0;
        bool wasAggrevated = false;

        private void Awake() {
            fighter = GetComponent<Fighter>();
            player = GameObject.FindWithTag("Player");
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            scheduler = GetComponent<ActionScheduler>();
            guardPosition = new LazyValue<Vector3>(GetInitPosition);
        }

        private Vector3 GetInitPosition() {
            return transform.position;
        }

        private void Start() {
            guardPosition.ForceInit();
        }

        private void Update() {
            if(health.IsDead()) return;
            if (InteractWithCombat()) return;
            if (InteractWithSuspicion()) return;
            if (InteractWithPatrol()) return;
            scheduler.CancellCurrentAction();
        }

        public void Aggrevate() {
            if(!wasAggrevated) {
                wasAggrevated = true;
                timeSinceAggrevated = 0;
                return;
            }      
        }

        private bool InteractWithSuspicion() {
            if(timeSinceLastSawPlayer < timeSuspicion) {
                timeSinceLastSawPlayer += Time.deltaTime;
                SetClosestWaypoint();
                scheduler.CancellCurrentAction();
                return true;
            } else {
                wasAggrevated = false;
            }
            return false;
        }

        private void SetClosestWaypoint()
        {
            if(patrolPath == null) return;
            currentWaypointIndex = patrolPath.GetClosestWaypoint(transform.gameObject);
        }

        private bool InteractWithPatrol() {   
            Vector3 nextPosition = guardPosition.value;
            if(patrolPath != null){
                if(AtWaypoint()) {
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }
            if((transform.position != nextPosition) && (timeStayedBeyondWaypoint > waypointDwelltime)){
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
                return true;
            }
            return false;
        }

        private bool AtWaypoint() {
            timeStayedBeyondWaypoint += Time.deltaTime;
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTollerance;
        }

        private void CycleWaypoint() {
            timeStayedBeyondWaypoint = 0;
            currentWaypointIndex = patrolPath.GetNextIndexPoint(currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint() {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private bool InteractWithCombat() {
            timeSinceAggrevated += Time.deltaTime;
            if ((CanChase() || IsAggrevated()) && fighter.CanAttack(player)) {
                timeSinceLastSawPlayer = 0f;
                StartAttack();
                return true;
            }
            return false;
        }

        private void StartAttack() {
            fighter.SetChasignSpped(chasingSpeedFraction);
            fighter.Attack(player);
            AggregateNearbyEnemies(); 
        }

        private void AggregateNearbyEnemies() {
            RaycastHit[] hits=  Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach(RaycastHit hit in hits) {
                hit.transform.GetComponent<AIController>()?.Aggrevate();
            }
        }

        private bool CanChase() {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            return distanceToPlayer < chaseDistance;
        }

        private bool IsAggrevated() {
            return timeSinceAggrevated < aggroCooldownTime;
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}

