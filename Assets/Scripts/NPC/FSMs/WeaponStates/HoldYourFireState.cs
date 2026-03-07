using FluidHTN;
using FPSDemo.FSM;

namespace FPSDemo.NPC.FSMs.WeaponStates
{
    public class HoldYourFireState : IState
    {
        public int Id => (int)WeaponStateType.HoldYourFire;

        public void Enter(IFiniteStateMachine mgr, IContext ctx)
        {
            //TODO: Lower weapon
        }

        public void Exit(IFiniteStateMachine mgr, IContext ctx)
        {

        }

        public void Tick(IFiniteStateMachine mgr, IContext ctx)
        {
            if (ctx is AIContext npcCtx)
            {
                if (npcCtx.HasWeaponState(WeaponStateType.HoldYourFire) == false)
                {
                    mgr.ChangeState((int)npcCtx.GetWeaponState(), ctx);
                    return;
                }

                if (npcCtx.CurrentEnemy != null)
                {
                    var distance = UnityEngine.Vector3.Distance(npcCtx.ThisNPC.transform.position, npcCtx.CurrentEnemy.transform.position);
                    if (distance <= 2f)
                    {
                        mgr.ChangeState((int)WeaponStateType.SingleShot, ctx);
                    }
                }
            }
        }
    }
}