using System.Collections.Generic;

namespace FPSDemo.NPC
{
    public static class GuardRegistry
    {
        private static readonly List<NPC> _all = new();

        public static void Register(NPC npc) => _all.Add(npc);
        public static void Unregister(NPC npc) => _all.Remove(npc);

        public static List<NPC> GetAlertedGuards()
        {
            var alerted = new List<NPC>();
            foreach (var npc in _all)
            {
                if (npc.Context.HasState(AIWorldState.AwareOfEnemy))
                    alerted.Add(npc);
            }
            return alerted;
        }
    }
}
