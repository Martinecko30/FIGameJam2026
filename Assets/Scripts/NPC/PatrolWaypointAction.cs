using UnityEngine;
using UnityEngine.Events;

namespace FPSDemo.NPC
{
    /// <summary>
    /// Optional component on a waypoint Transform.
    /// Fires when an NPC arrives, and can override the default patrol wait time.
    /// </summary>
    public class PatrolWaypointAction : MonoBehaviour
    {
        [Tooltip("Leave at -1 to use the PatrolPath default wait time.")]
        [SerializeField] private float _waitTimeOverride = -1f;

        [SerializeField] private UnityEvent _onNPCArrived = new();


        public float GetWaitTime(float defaultWait) =>
            _waitTimeOverride >= 0f ? _waitTimeOverride : defaultWait;

        public void OnArrived()
        {
            _onNPCArrived.Invoke();
        }
    }
}
