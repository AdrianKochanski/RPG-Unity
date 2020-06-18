using System;
using UnityEngine;

namespace RPG.Control {
    
    public class PatrolPath : MonoBehaviour {

        const float waypointGizmoRadius = 0.2f;

        private void OnDrawGizmos() {
            for (int i = 0; i < transform.childCount; i++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(GetWaypoint(i), waypointGizmoRadius);
                Gizmos.color = Color.black;
                int j = GetNextIndexPoint(i);
                Gizmos.DrawLine(
                    GetWaypoint(i),
                    GetWaypoint(j)
                );
            }
        }

        public int GetNextIndexPoint(int i)
        {
            if(i == transform.childCount - 1) return 0;
            else return i+1;
        }

        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }

        public int GetClosestWaypoint(GameObject objectTestet){
            int closestWaypoint = 0;
            float closestdistance = Mathf.Infinity;
            for (int i = 0; i < transform.childCount; i++)
            {
                float nextdistance = Vector3.Distance(objectTestet.transform.position, GetWaypoint(i));
                if(nextdistance < closestdistance){
                    closestdistance = nextdistance;
                    closestWaypoint = i;
                }
            }
            return closestWaypoint;
        }
    }
}
