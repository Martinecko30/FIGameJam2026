using FluidHTN.Operators;
using FluidHTN;
using UnityEngine;
using UnityEngine.AI;

namespace FPSDemo.NPC.Operators
{
    public class PatrolOperator : IOperator
    {
        private bool _isWaiting = false;
        private float _waitEndTime = 0f;

        private const float ArrivalDistance = 0.5f;

        public TaskStatus Start(IContext ctx)
        {
            if (!(ctx is AIContext c)) return TaskStatus.Failure;

            var patrol = c.PatrolPath;
            if (patrol == null || patrol.Count == 0) return TaskStatus.Failure;

            var waypoint = patrol.GetWaypoint(c.PatrolIndex);
            if (waypoint == null) return TaskStatus.Failure;

            _isWaiting = false;

            c.ThisController.ApplyWalkSpeed();
            c.ThisController.ClearAimAtPoint();

            if (!NavMesh.SamplePosition(waypoint.position, out var hit, 2f, NavMesh.AllAreas))
            {
                c.PatrolIndex = (c.PatrolIndex + 1) % patrol.Count;
                return TaskStatus.Failure;
            }

            c.ThisController.SetDestination(hit.position);
            return TaskStatus.Continue;
        }

        public TaskStatus Update(IContext ctx)
        {
            if (!(ctx is AIContext c)) return TaskStatus.Failure;

            if (_isWaiting)
            {
                return Time.time >= _waitEndTime ? TaskStatus.Success : TaskStatus.Continue;
            }

            var patrol = c.PatrolPath;
            var waypoint = patrol.GetWaypoint(c.PatrolIndex);
            var distance = Vector3.Distance(c.ThisNPC.transform.position, waypoint.position);

            if (distance > ArrivalDistance)
                return TaskStatus.Continue;

            // Arrived
            c.ThisController.SetDestination(null);

            waypoint.GetComponent<PatrolWaypointAction>()?.OnArrived();
            waypoint.GetComponent<PoisonableItem>()?.Consume(c.ThisNPC);

            var waypointAction = waypoint.GetComponent<PatrolWaypointAction>();
            var waitTime = waypointAction != null
                ? waypointAction.GetWaitTime(patrol.WaitTime)
                : patrol.WaitTime;

            if (patrol.IsCircular)
            {
                c.PatrolIndex = (c.PatrolIndex + 1) % patrol.Count;
            }
            else
            {
                int next = c.PatrolIndex + c.PatrolDirection;
                if (next >= patrol.Count) { c.PatrolDirection = -1; next = patrol.Count - 2; }
                else if (next < 0)        { c.PatrolDirection =  1; next = 1; }
                c.PatrolIndex = next;
            }
            _isWaiting = true;
            _waitEndTime = Time.time + waitTime;

            return TaskStatus.Continue;
        }

        public void Stop(IContext ctx)
        {
            if (ctx is AIContext c)
                c.ThisController.SetDestination(null);
            _isWaiting = false;
        }

        public void Abort(IContext ctx) => Stop(ctx);
    }
}
