using FluidHTN.Operators;
using FluidHTN;
using UnityEngine;
using UnityEngine.AI;

namespace FPSDemo.NPC.Operators
{
    public class MoveToPlayerOperator : IOperator
    {
        public TaskStatus StartNavigation(AIContext c)
        {
            if (c.CurrentEnemy == null || c.CurrentEnemy.IsPlayer == false)
            {
                c.ThisController.ClearAimAtPoint();
                return TaskStatus.Failure;
            }
            
            c.ThisController.ApplyPlayerAsAimAtPoint();
            var dir = (c.ThisNPC.transform.position - c.CurrentEnemy.transform.position).normalized;
            var destination = c.CurrentEnemy.transform.position + dir * c.IdealEnemyRange;
            if (NavMesh.SamplePosition(destination, out var hit, 1.0f, NavMesh.AllAreas))
            {
                c.ThisController.SetDestination(hit.position);
            }

            if (c.ThisController.DistanceToDestination > c.ThisController.StoppingDistance)
            {
                return TaskStatus.Continue;
            }


            return TaskStatus.Success;
        }

        public TaskStatus UpdateNavigation(AIContext c)
        {
            if (c.CurrentEnemy == null)
            {
                return TaskStatus.Failure;
            }

            if (c.ThisController.DistanceToDestination <= c.ThisController.StoppingDistance)
            {
                c.ThisController.SetDestination(null);
                return TaskStatus.Success;
            }

            var dir = (c.ThisNPC.transform.position - c.CurrentEnemy.transform.position).normalized;
            var destination = c.CurrentEnemy.transform.position + dir * c.IdealEnemyRange;
            if (NavMesh.SamplePosition(destination, out var hit, 1.0f, NavMesh.AllAreas))
            {
                c.ThisController.SetDestination(hit.position);
            }

            if (c.ThisController.DistanceToDestination > c.ThisController.StoppingDistance)
            {
                return TaskStatus.Continue;
            }

            return TaskStatus.Failure;
        }

        public TaskStatus Start(IContext ctx)
        {
            if (ctx is AIContext c)
            {
                return StartNavigation(c);
            }

            return TaskStatus.Failure;
        }

        public TaskStatus Update(IContext ctx)
        {
            if (ctx is AIContext c)
            {
                return UpdateNavigation(c);
            }

            return TaskStatus.Failure;
        }

        public void Stop(IContext ctx)
        {
            if (ctx is AIContext c)
            {
                c.ThisController.SetDestination(null);
            }
        }

        public void Abort(IContext ctx)
        {
            if (ctx is AIContext c)
            {
                Stop(c);
            }
        }
    }
}