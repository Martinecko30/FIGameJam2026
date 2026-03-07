using System;
using FPSDemo.NPC;
using UnityEngine;

public class AnimatorRelay : MonoBehaviour
{
    private ThirdPersonController tps;

    private void Awake()
    {
        if (tps == null)
        {
            tps = GetComponentInParent<ThirdPersonController>();
        }
    }

    private void OnMeleeHit(AnimationEvent animationEvent)
    {
        tps.OnMeleeHit(animationEvent);
    }
}
