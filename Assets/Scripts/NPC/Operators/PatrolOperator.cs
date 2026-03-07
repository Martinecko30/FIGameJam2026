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
        private Vector3 _currentDestination;
        private string _activeAnimationBool = "";

        private const float ArrivalDistance = 0.5f;

        private void ClearWaypointBehaviors(AIContext c)
        {
            c.ThisController.SetLookAtPosition(null);
            if (!string.IsNullOrEmpty(_activeAnimationBool))
            {
                c.ThisController.SetAnimatorBool(_activeAnimationBool, false);
                _activeAnimationBool = "";
            }
        }

        public TaskStatus Start(IContext ctx)
        {
            if (!(ctx is AIContext c)) return TaskStatus.Failure;

            var patrol = c.PatrolPath;
            if (patrol == null || patrol.Count == 0) return TaskStatus.Failure;

            var waypoint = patrol.GetWaypoint(c.PatrolIndex);
            if (waypoint == null) return TaskStatus.Failure;

            _isWaiting = false;
            ClearWaypointBehaviors(c);

            c.ThisController.ApplyWalkSpeed();
            c.ThisController.ClearAimAtPoint();

            if (!NavMesh.SamplePosition(waypoint.position, out var hit, 2f, NavMesh.AllAreas))
            {
                c.PatrolIndex = (c.PatrolIndex + 1) % patrol.Count;
                return TaskStatus.Failure;
            }

            _currentDestination = hit.position;
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
            var distance = Vector3.Distance(c.ThisNPC.transform.position, _currentDestination);
            var arrivalRadius = Mathf.Max(ArrivalDistance, c.ThisController.StoppingDistance + 0.1f);

            if (distance > arrivalRadius)
                return TaskStatus.Continue;

            // Arrived
            c.ThisController.SetDestination(null);

            waypoint.GetComponent<PatrolWaypointAction>()?.OnArrived();
            waypoint.GetComponent<PoisonableItem>()?.Consume(c.ThisNPC);

            var waypointAction = waypoint.GetComponent<PatrolWaypointAction>();
            var waitTime = waypointAction != null
                ? waypointAction.GetWaitTime(patrol.WaitTime)
                : patrol.WaitTime;

            if (waypointAction != null && waypointAction.UseWaypointFacing)
                c.ThisController.SetLookAtPosition(waypoint.position + waypoint.forward * 100f);

            if (waypointAction != null && !string.IsNullOrEmpty(waypointAction.WaitAnimationBool))
            {
                _activeAnimationBool = waypointAction.WaitAnimationBool;
                c.ThisController.SetAnimatorBool(_activeAnimationBool, true);
            }

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
            {
                c.ThisController.SetDestination(null);
                ClearWaypointBehaviors(c);
            }
            _isWaiting = false;
        }

        public void Abort(IContext ctx) => Stop(ctx);
    }
}
