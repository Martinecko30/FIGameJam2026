using System;
using UnityEngine;

namespace FPSDemo.Target
{
    public class HealthSystem : MonoBehaviour
    {
        [SerializeField] bool godMode = false;
        [SerializeField] int hitsToKill = 3;

        public string ActionDescription
        {
            get { return "Perform takedown"; }
        }

        bool isDead = false;
        int _hitCount = 0;
        public event Action OnDeath;
        public event Action<Vector3, HumanTarget> OnDamageTaken;
        public HumanTarget ThisTarget { get; private set; }
        CapsuleCollider characterCollider;

        void Awake()
        {
            ThisTarget = GetComponent<HumanTarget>();
            characterCollider = GetComponent<CapsuleCollider>();
        }

        public void ForceKill()
        {
            if (!isDead && !godMode)
            {
                KillThisEntity();
            }
        }

        public void WasShot(HumanTarget shotBy)
        {
            if (!isDead && shotBy != ThisTarget && !godMode)
            {
                // Notify about damage taken at this position
                OnDamageTaken?.Invoke(transform.position, shotBy);

                _hitCount++;
                if (_hitCount >= hitsToKill)
                {
                    KillThisEntity();
                }
            }
        }

        void KillThisEntity()
        {
            if (!ThisTarget.IsPlayer)
            {
                characterCollider.enabled = false;
            }

            isDead = true;
            OnDeath?.Invoke();
        }
    }
}
