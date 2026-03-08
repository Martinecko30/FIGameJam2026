using FluidHTN.Operators;
using FluidHTN;
using UnityEngine;
using UnityEngine.AI;
namespace FPSDemo.NPC.Operators
{
    public class EmergencyRepositionOperator : IOperator
    {
        private float _originalSpeed;

        public TaskStatus StartNavigation(AIContext c)
        {
                // Try to move away from current enemy
                if (c.CurrentEnemy != null)
                {
                    var diff = c.ThisNPC.transform.position - c.CurrentEnemy.transform.position;
                    diff.y = 0f;
                    var directionAwayFromEnemy = diff.normalized;
                    var fallbackDestination = c.ThisNPC.transform.position + directionAwayFromEnemy * 15f;
                    
                    if (NavMesh.SamplePosition(fallbackDestination, out var fallbackHit, 5.0f, NavMesh.AllAreas))
                    {
                        c.ThisController.SetDestination(fallbackHit.position);
                        // Store original speed and set higher movement speed for emergency repositioning
                        _originalSpeed = c.ThisController.Speed;
                        c.ThisController.Speed = _originalSpeed * 1.5f;
                        c.ThisController.ApplyPlayerAsAimAtPoint();
                        
                        return TaskStatus.Continue;
                    }
                }
                c.ThisController.ClearAimAtPoint();
                return TaskStatus.Failure;
        }

        public TaskStatus UpdateNavigation(AIContext c)
        {
            if (c.ThisController.DistanceToDestination <= c.ThisController.StoppingDistance)
            {
                c.ThisController.SetDestination(null);
                // Reset speed to normal
                ResetMovementSpeed(c);
                return TaskStatus.Success;
            }

            return TaskStatus.Continue;
        }

        public TaskStatus Start(IContext ctx)
        {
            return TaskStatus.Continue;
        }

        public TaskStatus Update(IContext ctx)
        {
            if (ctx is AIContext c)
            {
                if (!c.HasRecentDamage)
                {
                    return TaskStatus.Failure;
                }
                if (c.ThisController.IsStopped)
                {
                    return StartNavigation(c);
                }
                else
                {
                    return UpdateNavigation(c);
                }
            }

            return TaskStatus.Failure;
        }

        public void Stop(IContext ctx)
        {
            if (ctx is AIContext c)
            {
                c.ThisController.SetDestination(null);
                ResetMovementSpeed(c);
            }
        }

        public void Abort(IContext ctx)
        {
            Stop(ctx);
        }

        private void ResetMovementSpeed(AIContext c)
        {
            // Reset to original speed
            if (_originalSpeed > 0)
            {
                c.ThisController.Speed = _originalSpeed;
            }
        }
    }
}