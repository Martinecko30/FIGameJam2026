using UnityEngine;
using FPSDemo.Target;

namespace FPSDemo.NPC.Sensors
{
    /// <summary>
    /// Instantly alerts an NPC when the player enters the trigger collider on this GameObject.
    /// Attach to a child GameObject that has a trigger collider (e.g. a sphere trigger on the NPC body).
    /// </summary>
    public class TouchSensor : MonoBehaviour
    {
        // ========================================================= INSPECTOR FIELDS

        [SerializeField] private NPC _npc;


        // ========================================================= UNITY METHODS

        private void OnValidate()
        {
            if (_npc == null)
            {
                _npc = GetComponentInParent<NPC>();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<HumanTarget>(out var target)) return;
            if (!target.IsPlayer) return;

            AlertToPlayer(target);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.TryGetComponent<HumanTarget>(out var target)) return;
            if (!target.IsPlayer) return;

            AlertToPlayer(target);
        }


        // ========================================================= HELPERS

        private void AlertToPlayer(HumanTarget target)
        {
            var context = _npc?.Context;
            if (context == null) return;

            if (context.EnemiesSpecificData.ContainsKey(target))
            {
                context.SetAwarenessOfThisEnemy(target, context.AlertAwarenessThreshold);
            }
        }
    }
}
