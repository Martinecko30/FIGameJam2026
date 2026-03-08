using FluidHTN.Operators;
using FluidHTN;
using UnityEngine;
using UnityEngine.AI;

namespace FPSDemo.NPC.Operators
{
    public class InvestigateOperator : IOperator
    {
        public TaskStatus Start(IContext ctx)
        {
            if (!(ctx is AIContext c)) return TaskStatus.Failure;

            if (!NavMesh.SamplePosition(c.InvestigatePosition, out var hit, 2f, NavMesh.AllAreas))
            {
                c.ClearInvestigatePosition();
                return TaskStatus.Failure;
            }

            c.ThisController.ApplyWalkSpeed();
            c.ThisController.ClearAimAtPoint();
            c.ThisController.SetDestination(hit.position);
            c.ThisNPC.Bark(BarkType.Investigate);

            return TaskStatus.Continue;
        }

        public TaskStatus Update(IContext ctx)
        {
            if (!(ctx is AIContext c)) return TaskStatus.Failure;

            // Wait for NavMeshAgent to compute the path before checking distance
            if (!c.ThisController.HasPath) return TaskStatus.Continue;

            if (c.ThisController.DistanceToDestination <= c.ThisController.StoppingDistance)
            {
                c.ThisController.SetDestination(null);
                c.ClearInvestigatePosition();
                return TaskStatus.Success;
            }

            return TaskStatus.Continue;
        }

        public void Stop(IContext ctx)
        {
            if (ctx is AIContext c)
                c.ThisController.SetDestination(null);
        }

        public void Abort(IContext ctx) => Stop(ctx);
    }
}
