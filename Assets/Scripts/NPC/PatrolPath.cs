using System.Collections.Generic;
using UnityEngine;

namespace FPSDemo.NPC
{
    public class PatrolPath : MonoBehaviour
    {
        [Tooltip("How long NPCs wait at each waypoint before moving on.")]
        [SerializeField] private float _waitTimeAtWaypoint = 1f;

        [Tooltip("Circular: loops back to start. End-to-end: reverses direction at each end.")]
        [SerializeField] private bool _isCircular = true;

        private List<Transform> _waypoints = new();

        public int Count => _waypoints.Count;
        public float WaitTime => _waitTimeAtWaypoint;
        public bool IsCircular => _isCircular;

        public Transform GetWaypoint(int index) =>
            _waypoints.Count > 0 ? _waypoints[index % _waypoints.Count] : null;

        private void Awake()
        {
            _waypoints = CollectLeafChildren(transform);
        }

        private static List<Transform> CollectLeafChildren(Transform root)
        {
            var results = new List<Transform>();
            foreach (Transform child in root)
            {
                if (child.childCount == 0)
                    results.Add(child);
                else
                    results.AddRange(CollectLeafChildren(child));
            }
            return results;
        }

        private void OnDrawGizmos()
        {
            var points = CollectLeafChildren(transform);
            if (points.Count == 0) return;

            Gizmos.color = Color.cyan;
            for (int i = 0; i < points.Count; i++)
            {
                Gizmos.DrawSphere(points[i].position, 0.15f);
                bool isLast = i == points.Count - 1;
                if (!isLast || _isCircular)
                    Gizmos.DrawLine(points[i].position, points[(i + 1) % points.Count].position);
            }
        }
    }
}
