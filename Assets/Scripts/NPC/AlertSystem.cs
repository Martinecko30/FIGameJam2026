using System;
using UnityEngine;
using FPSDemo.Target;

namespace FPSDemo.NPC
{
    /// <summary>
    /// Broadcasts enemy spotted alerts to nearby NPCs.
    /// </summary>
    public static class AlertSystem
    {
        // ========================================================= EVENTS

        /// <summary>
        /// Fired when an NPC spots a target.
        /// Parameters: spottedTarget, reporterPosition, alertRadius
        /// </summary>
        public static event Action<HumanTarget, Vector3, float> OnEnemySpotted;


        // ========================================================= METHODS

        public static void ReportEnemySpotted(HumanTarget spottedTarget, Vector3 reporterPosition, float alertRadius)
        {
            OnEnemySpotted?.Invoke(spottedTarget, reporterPosition, alertRadius);
        }
    }
}
