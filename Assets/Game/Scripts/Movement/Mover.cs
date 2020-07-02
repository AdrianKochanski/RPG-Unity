using RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using GameDevTV.Saving;
using RPG.Attributes;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] Transform target;
        [SerializeField] float MaxSpeed = 7f;
        [SerializeField] float maxPathLength = 40f;
        NavMeshAgent navMeshAgent;
        Health health;
        ActionScheduler scheduler;
        Animator animator;

        private void Awake() {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
            animator = GetComponent<Animator>();
            scheduler = GetComponent<ActionScheduler>();
        }

        void Update()
        {
            navMeshAgent.enabled = !health.IsDead();
            UpdateAnimator();
        }
        
        public void StartMoveAction(Vector3 destination, float speedFraction){
            scheduler.StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public bool CanMoveTo(Vector3 destination) {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if(!hasPath) {
                print("Not available");
                return false;
            }
            if(path.status != NavMeshPathStatus.PathComplete) return false;
            if(GetPathLength(path) > maxPathLength) return false;
            return true;
        }

        private float GetPathLength(NavMeshPath path) {
            float total = 0;
            if (path.corners.Length < 2) return total;
            for (int i = 0; i < path.corners.Length - 1; i++) {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return total;
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.speed = MaxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }

        public void Cancel(){
            navMeshAgent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            animator.SetFloat("forwardSpeed", speed);
        }

        [System.Serializable]
        struct MoverSaveData {
            public SerializableVector3 position;
            public SerializableVector3 eulerAngles;
        }

        public object CaptureState()
        {
            MoverSaveData data = new MoverSaveData();
            data.position = new SerializableVector3(transform.position);
            data.eulerAngles = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        public void RestoreState(object state)
        {
            MoverSaveData data = (MoverSaveData)state;
            navMeshAgent.enabled = false;
            transform.position = data.position.ToVector();
            transform.eulerAngles = data.eulerAngles.ToVector();
            navMeshAgent.enabled = true;
        }
    }
}
