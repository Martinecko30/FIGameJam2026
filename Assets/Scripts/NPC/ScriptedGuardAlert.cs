using System;
using System.Collections.Generic;
using UnityEngine;

namespace FPSDemo.NPC
{
    public class ScriptedGuardAlert : MonoBehaviour
    {
        [SerializeField] private List<NPC> _guardsToAlert = new();

        private void Start()
        {
            TriggerInvestigation();
        }

        public void TriggerInvestigation()
        {
            foreach (var guard in _guardsToAlert)
            {
                if (guard != null)
                    guard.SendToInvestigate(transform.position);
            }
        }
    }
}
