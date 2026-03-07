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

        [Tooltip("Rotate the NPC to face this waypoint's forward direction while waiting.")]
        [SerializeField] private bool _useWaypointFacing = false;

        [Tooltip("Animator bool parameter to set true while the NPC waits here (cleared on departure).")]
        [SerializeField] private string _waitAnimationBool = "";

        [SerializeField] private UnityEvent _onNPCArrived = new();

        public float GetWaitTime(float defaultWait) =>
            _waitTimeOverride >= 0f ? _waitTimeOverride : defaultWait;

        public bool UseWaypointFacing => _useWaypointFacing;
        public string WaitAnimationBool => _waitAnimationBool;

        public void OnArrived() => _onNPCArrived.Invoke();
    }
}
