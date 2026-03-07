using UnityEngine;

namespace FPSDemo.NPC.Domains.TaskDefinitions
{
    [CreateAssetMenu(fileName = "New AIInvestigateTask", menuName = "FPSDemo/AI/InvestigateTask")]
    public class InvestigateTaskDefinition : AICompoundTaskDefinition
    {
        public override void Add(AIDomainBuilder builder)
        {
            builder.Investigate();
        }
    }
}
