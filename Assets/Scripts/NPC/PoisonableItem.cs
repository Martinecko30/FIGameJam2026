using UnityEngine;

namespace FPSDemo.NPC
{
    /// <summary>
    /// Attach to a waypoint the commander visits (e.g. wine glass).
    /// Call Poison() from player interaction code.
    /// The PatrolOperator calls Consume() when the NPC arrives at this waypoint.
    /// </summary>
    public class PoisonableItem : MonoBehaviour
    {
        public bool IsPoisoned = false;

        /// <summary>Called by player interaction to poison this item.</summary>
        public void Poison()
        {
            IsPoisoned = true;
        }

        /// <summary>Called by the patrol system when an NPC arrives at this waypoint.</summary>
        public void Consume(NPC consumer)
        {
            if (!IsPoisoned) return;

            var health = consumer.GetComponent<Target.HealthSystem>();
            health?.ForceKill();
        }
    }
}
