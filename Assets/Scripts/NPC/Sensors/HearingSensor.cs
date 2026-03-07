using UnityEngine;
using FPSDemo.Core;
using FPSDemo.Player;

namespace FPSDemo.NPC.Sensors
{
    [RequireComponent(typeof(NPC))]
    public class HearingSensor : MonoBehaviour, ISensor
    {
        // ========================================================= INSPECTOR FIELDS

        [Tooltip("Max distance at which footsteps can be heard.")]
        [SerializeField] private float _maxFootstepHearingDistance = 20f;

        [Tooltip("Awareness gained per second at max noise level and zero distance.")]
        [SerializeField] private float _awarenessGainPerSecondAtMaxNoise = 2f;

        [Tooltip("Awareness fraction of alert threshold before the NPC turns toward the sound (0-1).")]
        [Range(0f, 1f)]
        [SerializeField] private float _turnTowardSoundThreshold = 0.75f;

        [Header("Debug")]
        [SerializeField] private bool _debugMode = false;


        // ========================================================= PRIVATE FIELDS

        private AIContext _lastContext;


        // ========================================================= PROPERTIES

        public float TickRate => Game.AISettings != null ? Game.AISettings.HearingSensorTickRate : 0f;
        public float NextTickTime { get; set; }


        // ========================================================= TICK

        public void Tick(AIContext context)
        {
            _lastContext = context;

            if (PlayerNoiseMaker.Instance == null) return;

            var noise = PlayerNoiseMaker.Instance;
            var noisePosition = noise.NoisePosition;
            var distance = Vector3.Distance(noisePosition, transform.position);
            var dt = TickRate > 0f ? TickRate : Time.deltaTime;

            var totalGain = 0f;

            // --- Footstep noise ---
            if (noise.MovementNoiseLevel > 0f && distance <= _maxFootstepHearingDistance)
            {
                var distanceFactor = 1f - (distance / _maxFootstepHearingDistance);
                totalGain += noise.MovementNoiseLevel * distanceFactor * _awarenessGainPerSecondAtMaxNoise * dt;

                if (_debugMode)
                    Debug.Log($"[HearingSensor] {gameObject.name} | footstep noise={noise.MovementNoiseLevel:F2} dist={distance:F1}m");
            }

            // --- Weapon noise ---
            var weaponRange = noise.WeaponNoiseRange;
            if (weaponRange > 0f && distance <= weaponRange)
            {
                var distanceFactor = 1f - (distance / weaponRange);
                totalGain += distanceFactor * _awarenessGainPerSecondAtMaxNoise * dt;

                if (_debugMode)
                    Debug.Log($"[HearingSensor] {gameObject.name} | weapon noise range={weaponRange:F1}m dist={distance:F1}m");
            }

            if (totalGain <= 0f)
            {
                context.ThisController.SetLookAtPosition(null);
                return;
            }

            foreach (var kvp in context.EnemiesSpecificData)
            {
                if (!kvp.Key.IsPlayer) continue;

                context.SetAwarenessOfThisEnemy(kvp.Key, kvp.Value.awarenessOfThisTarget + totalGain);

                if (_debugMode)
                    Debug.Log($"[HearingSensor] {gameObject.name} | awareness={kvp.Value.awarenessOfThisTarget:F2}/{context.AlertAwarenessThreshold:F2} (+{totalGain:F3})");

                var turnThreshold = context.AlertAwarenessThreshold * _turnTowardSoundThreshold;
                if (kvp.Value.awarenessOfThisTarget >= turnThreshold &&
                    kvp.Value.awarenessOfThisTarget < context.AlertAwarenessThreshold)
                {
                    context.ThisController.SetLookAtPosition(noisePosition);
                }
            }
        }


        // ========================================================= GIZMOS

        private void OnDrawGizmos()
        {
            if (PlayerNoiseMaker.Instance == null) return;

            var noise = PlayerNoiseMaker.Instance;
            var noisePosition = noise.NoisePosition;
            var distance = Vector3.Distance(noisePosition, transform.position);

            var hearingFootstep = noise.MovementNoiseLevel > 0f && distance <= _maxFootstepHearingDistance;
            var hearingWeapon = noise.WeaponNoiseRange > 0f && distance <= noise.WeaponNoiseRange;
            var isHearing = hearingFootstep || hearingWeapon;

            // Footstep range
            Gizmos.color = hearingFootstep ? new Color(1f, 1f, 0f, 0.15f) : new Color(1f, 1f, 1f, 0.05f);
            Gizmos.DrawSphere(transform.position, _maxFootstepHearingDistance);
            Gizmos.color = hearingFootstep ? Color.yellow : new Color(1f, 1f, 1f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, _maxFootstepHearingDistance);

            // Weapon range (only when active)
            if (noise.WeaponNoiseRange > 0f)
            {
                Gizmos.color = hearingWeapon ? new Color(1f, 0.3f, 0f, 0.15f) : new Color(1f, 0.3f, 0f, 0.05f);
                Gizmos.DrawSphere(transform.position, noise.WeaponNoiseRange);
                Gizmos.color = hearingWeapon ? new Color(1f, 0.3f, 0f) : new Color(1f, 0.3f, 0f, 0.4f);
                Gizmos.DrawWireSphere(transform.position, noise.WeaponNoiseRange);
            }

            if (!isHearing) return;

            Gizmos.color = hearingWeapon ? Color.red : Color.yellow;
            Gizmos.DrawLine(transform.position + Vector3.up, noisePosition + Vector3.up);

            if (_lastContext == null) return;
            foreach (var kvp in _lastContext.EnemiesSpecificData)
            {
                if (!kvp.Key.IsPlayer) continue;
                var fraction = kvp.Value.awarenessOfThisTarget / _lastContext.AlertAwarenessThreshold;
                Gizmos.color = Color.Lerp(Color.green, Color.red, fraction);
                var barStart = transform.position + Vector3.up * 2.5f;
                Gizmos.DrawLine(barStart, barStart + Vector3.up * fraction);
            }
        }
    }
}
