using UnityEngine;

namespace FPSDemo.NPC.Domains.TaskDefinitions
{
    [CreateAssetMenu(fileName = "New AIPatrolTask", menuName = "FPSDemo/AI/PatrolTask")]
    public class PatrolTaskDefinition : AICompoundTaskDefinition
    {
        public override void Add(AIDomainBuilder builder)
        {
            builder.Patrol();
        }
    }
}
