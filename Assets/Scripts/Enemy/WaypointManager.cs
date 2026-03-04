using UnityEngine;

namespace Enemy
{
    public class WaypointManager : MonoBehaviour
    {
        [SerializeField] private Transform[] waypoints;

        public Transform[] GetWaypoints()
        {
            return waypoints;
        }

        private void OnDrawGizmos()
        {
            if (waypoints == null || waypoints.Length == 0) return;

            Gizmos.color = Color.cyan;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] == null) continue;

                // Draw sphere at waypoint
                Gizmos.DrawWireSphere(waypoints[i].position, 0.3f);

                // Draw line to next waypoint
                if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }

                // Draw waypoint number
                #if UNITY_EDITOR
                UnityEditor.Handles.Label(waypoints[i].position, "WP " + i);
                #endif
            }
        }
    }
}
