using UnityEngine;

namespace FPSDemo.NPC
{
    /// <summary>
    /// Global static coordinator that prevents multiple NPCs from barking simultaneously.
    /// Before playing a bark, call TryReserve — it returns false if another NPC barked recently.
    /// </summary>
    public static class BarkCoordinator
    {
        private static float _globalSilenceUntil;

        public static bool TryReserve(float clipLength)
        {
            if (Time.time < _globalSilenceUntil)
                return false;

            _globalSilenceUntil = Time.time + clipLength;
            return true;
        }
    }
}
